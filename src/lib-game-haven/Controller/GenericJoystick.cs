using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;

using GameHaven.Diagnostics;


// Many nods and thanks to https://github.com/nahueltaibo/gamepad
namespace GameHaven.Controller
{    
    public class GenericJoystick : IDisposable {
        public class AxisEventArgs {
            public byte Axis { get; set; }
            public short Value { get; set; }
        }

        public class ButtonEventArgs {
            public byte Button { get; set; }
            public bool Pressed { get; set; }
        }

        [Flags]
        private enum MessageFlags {
            Configuration   = 0x80,
            Button          = 0x01,
            Axis            = 0x02
        }

        private const int MESSAGE_SIZE = 8;
        private const int MESSAGE_FLAG_INDEX = 6;
        private const int PRESSED_FLAG_INDEX = 4;
        private const int ADDRESS_INDEX = 7;
        private const int AXIS_INDEX_START = 4;        

        private bool _disposedValue;
        private readonly string _deviceFile;
        private string? _identifier;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Dictionary<byte, bool> _buttons = new Dictionary<byte, bool>();
        private Dictionary<byte, short> _axis = new Dictionary<byte, short>();

        private ILogger _logger;

        public TimeSpan RetryDeviceFileInterval { get; set; } = TimeSpan.FromSeconds(1);

        public bool IsPressed(byte button) {
            return _buttons[button];
        }

        public short LastAxisValue(byte axis) {
            return _axis[axis];
        }

        public event EventHandler<ButtonEventArgs>? ButtonChanged;

        public event EventHandler<AxisEventArgs>? AxisChanged;
        
        public bool CallbacksForAllEvents { get; set; } = true;

        public bool Available { get { return File.Exists(_deviceFile); } }

        public string Identifier { get { return _identifier != null ? _identifier : _deviceFile; } }

        public string Device { get { return _deviceFile; } }

        public bool LogUnplugged { get; set; } = false;

        public GenericJoystick() : this("/dev/input/js0") {}

        public GenericJoystick(string deviceFile)
        {
            _logger = Logging.GetLogger($"GenericJoystick[{deviceFile}]");
            _deviceFile = deviceFile;

            // Get device name (if able)
            ProbeJoystick();

            // Create the Task that will constantly read the device file, process its bytes and fire events accordingly
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ProcessMessages(_cancellationTokenSource.Token));
        }

        private bool HasFlag(byte value, MessageFlags flag) {
            return (value & (byte)flag) == (byte)flag;
        }
        
        private void ProcessMessages(CancellationToken token) {            
            while (!token.IsCancellationRequested) {
                try {
                    if (_identifier == null) {
                        ProbeJoystick();
                    }

                    ProcessDeviceFile(token);
                } catch (Exception ex) {
                    if (!File.Exists(_deviceFile) && LogUnplugged) {
                        _logger.LogWarning($"{ex.Message} while trying to read messages from {_deviceFile} - unplugged?");
                    }
                    Thread.Sleep(RetryDeviceFileInterval);
                }
            }                        
        }

        private void ProcessDeviceFile(CancellationToken token) {
            using (FileStream fs = new FileStream(_deviceFile, FileMode.Open)) {                
                byte[] message = new byte[MESSAGE_SIZE];
                while (!token.IsCancellationRequested) {                    
                    fs.Read(message, 0, MESSAGE_SIZE);

                    if (HasFlag(message[MESSAGE_FLAG_INDEX], MessageFlags.Configuration)) {                    
                        ProcessConfiguration(message);
                    }

                    ProcessValues(message);
                }
            }
        }

        private void ProcessConfiguration(byte[] message) {
            if(HasFlag(message[MESSAGE_FLAG_INDEX], MessageFlags.Button)) {
                byte key = message[ADDRESS_INDEX];
                if (!_buttons.ContainsKey(key)) {
                    _buttons.Add(key, false);                    
                }
            } else if (HasFlag(message[MESSAGE_FLAG_INDEX], MessageFlags.Axis)) {
                byte key = message[ADDRESS_INDEX];
                if (!_axis.ContainsKey(key)) {
                    _axis.Add(key, 0);
                }
            }
        }

        private void ProcessValues(byte[] message) {
            if(HasFlag(message[MESSAGE_FLAG_INDEX], MessageFlags.Button)) {
                var oldValue = _buttons[message[ADDRESS_INDEX]];
                var newValue = message[PRESSED_FLAG_INDEX] == 0x01;

                if (CallbacksForAllEvents || oldValue != newValue) {
                    // Note, important to update the _buttons value *AFTER* the callback, so callback can compare against current state if useful
                    ButtonChanged?.Invoke(this, new ButtonEventArgs { Button = message[ADDRESS_INDEX], Pressed = newValue });
                    _buttons[message[ADDRESS_INDEX]] = newValue;
                }
            } else if (HasFlag(message[MESSAGE_FLAG_INDEX], MessageFlags.Axis)) {
                var oldValue = _axis[message[ADDRESS_INDEX]];
                var newValue = BitConverter.ToInt16(message, AXIS_INDEX_START);

                if (CallbacksForAllEvents || oldValue != newValue) {
                    // Note, important to update the _buttons value *AFTER* the callback, so callback can compare against current state if useful
                    AxisChanged?.Invoke(this, new AxisEventArgs { Axis = message[ADDRESS_INDEX], Value = newValue });
                    _axis[message[ADDRESS_INDEX]] = newValue;
                }
            }
        }

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int handle, uint request, byte[] output);

        [DllImport("libc", EntryPoint = "__errno_location")]
        private static extern System.IntPtr __errno_location();
        private const uint JSIOCGNAME_128 = 0x80806A13;  // JSIOCGNAME(len = 128)

        private void ProbeJoystick() {
            try {
                if (File.Exists(_deviceFile)) {
                    using(var fileHandle = File.OpenHandle(_deviceFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None, FileOptions.None, 0)) {
                        byte[] name = new byte[128];
                        if(ioctl(fileHandle.DangerousGetHandle().ToInt32(), JSIOCGNAME_128, name) < 0) {
                            _logger.LogError($"ProbeJoystick ioctl({JSIOCGNAME_128}) error: {System.Runtime.InteropServices.Marshal.ReadInt32(__errno_location())}");
                        } else {
                            _identifier = System.Text.ASCIIEncoding.ASCII.GetString(name).TrimEnd(new char[] { '\r', '\n', ' ', '\0' });
                            _logger.LogInformation($"Joystick at {_deviceFile} => {_identifier}");
                        }
                    }
                }
            } catch(Exception ex) {
                _logger.LogError($"Unable to probe {_deviceFile}: {ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    _cancellationTokenSource.Cancel();
                }
                
                _disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}