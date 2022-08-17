using Microsoft.Extensions.Logging;

using GameHaven.Renderer;
using GameHaven.Diagnostics;

using Nfw.Linux.Joystick.Simple;

namespace GameHaven.States {

    /**
        Our job is to:
            - Load overall settings, and initiate the starting state
            - Funnel input events to the current state
            - Provide means for current state, or external events, to cause a move to <new state>
     **/
    public class StateEngine : IDisposable {
        public const string DEFAULT_SETTINGS_FILE_PATH = "data/settings.json";        

        private readonly ILogger _logger;
        private readonly string _settingsFilePath;
        private bool _disposed;
        private List<Joystick> _joysticks = new List<Joystick>();
        
        public AsyncFrameBufferDisplay Display { get; private set; }

        public StateEngine() : this(DEFAULT_SETTINGS_FILE_PATH) {
        }

        public StateEngine(string settingsFilePath) {
            _logger = Logging.GetLogger("StateEngine");
            _settingsFilePath = settingsFilePath;
            
            Display = AsyncFrameBufferDisplay.CreateDisplay();
            Display.Clear();
        }

        public void Initialize() {               
            // Test image from: https://commons.wikimedia.org/wiki/File:Test.svg
            Display.DisplayImage("data/test-image.png");

            _joysticks.Add(new Joystick("/dev/input/js0"));
            _joysticks.Add(new Joystick("/dev/input/js1"));
            _joysticks.Add(new Joystick("/dev/input/js2"));

            foreach(Joystick joystick in _joysticks) {
                joystick.AxisCallback = (j, e, d) => {
                    _logger.LogDebug($"{j.DeviceName} => Axis[{e}, {d}]");
                };

                joystick.ButtonCallback = (j, e, d) => {
                    _logger.LogDebug($"{j.DeviceName} => Button[{e}, {d}]");
                };

                joystick.ConnectedCallback = (j, c) => {
                    _logger.LogDebug($"{j.DeviceName} => Connected: {c}");
                };
            }
        }
        
        
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    foreach(Joystick joystick in _joysticks) {
                        joystick.Dispose();
                    }
                }
                
                _disposed = true;
            }
        }

        public void Dispose() {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}