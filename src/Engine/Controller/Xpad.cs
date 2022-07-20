using GameHaven.Engine.Diagnostics;

using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace GameHaven.Engine.Controller {

    // Wraps a Gamepad with constants for buttons/sticks as well as support for 
    //  short/long click etc.
    public class Xpad : IDisposable
    {        
        public bool Available { get { return _gamepad.Available; } }
                            
        public enum Button {
            A       = 0x00,
            B       = 0x01,
            X       = 0x02,
            Y       = 0x03,
            LB      = 0x04,
            RB      = 0x05,
            View    = 0x06,
            Menu    = 0x07,
            L       = 0x09,
            R       = 0x0A,

            StartSyntheticButtons = 0xA0,

            // Only if TreatAxisAsButtons is used (these numbers are simulated)
            X1          = StartSyntheticButtons + 1,
            X2          = StartSyntheticButtons + 1,  // X2 == X1
            LT          = StartSyntheticButtons + 3,
            RT          = StartSyntheticButtons + 4,
            DPadLeft    = StartSyntheticButtons + 5,
            DPadRight   = StartSyntheticButtons + 6,
            DPadUp      = StartSyntheticButtons + 7,
            DPadDown    = StartSyntheticButtons + 8
        }

        public enum Axis {
            X1              = 0x01, 
            X2              = 0x01, // Same as X1?
            LT              = 0x02,
            RT              = 0x05,
            DPadLeftRight   = 0x06, // 0x06 (negative = Left, positive = Right)            
            DPadUpDown       = 0x07, // 0x07 (negative = Down, positive = Up)            
        }

        public ButtonSettings DefaultButtonSettings { get; set; } = new ButtonSettings();       
        private Dictionary<Button, ButtonSettings> _buttonSettings = new Dictionary<Button, ButtonSettings>();        

        private const short AXIS_NEGATIVE_MAX = -32767;
        private const short AXIS_POSITIVE_MAX = 32767;

        private bool _disposedValue;        
        private GenericJoystick _gamepad;
        
        // Args: Button, Event, and Duration
        private readonly Action<Button, ButtonEventTypes, double>? _buttonAction;
        
        // TBD: For now we don't need it, so we dont have an Action<Axis, short> --- but could... someday, if useful...

        private readonly ButtonEventTypes _subscribedEvents;
        private readonly string _deviceFile;
                        
        private Stopwatch _stopwatch = new Stopwatch();        

        private class ButtonState {
            public TimeSpan LastEventElapsedTime;
            public bool     Pressed;
        }

        private Dictionary<Button, ButtonState> _buttonState = new Dictionary<Button, ButtonState>();

        private ILogger _logger;

        public const string DEFAULT_DEVICE_FILE = "/dev/input/js0";        

        public Xpad(string deviceFile, Action<Button, ButtonEventTypes, double> callback) {
            _logger = Logging.GetLogger($"Xpad[{deviceFile}]");
            _deviceFile = deviceFile;
            _buttonAction = callback;
            _subscribedEvents = ButtonEventTypes.All;
            _stopwatch.Start();
            _gamepad = new GenericJoystick(_deviceFile);
            _gamepad.CallbacksForAllEvents = true;
            _gamepad.ButtonChanged += ButtonChangeCallback;
            _gamepad.AxisChanged += AxisChangeCallback;
        }

        public Xpad(string deviceFile, ButtonEventTypes subscribeTypes, Action<Button, ButtonEventTypes, double> callback) : this(deviceFile, callback) {
            _subscribedEvents = subscribeTypes;
        }      

        public void SetButtonSettings(Button button, ButtonSettings settings) {
            lock (_buttonSettings) {
                _buttonSettings[button] = settings;
            }
        }

        public void ClearButtonSettings() {
            lock (_buttonSettings) {
                _buttonSettings.Clear();
            }
        }

        private ButtonSettings GetButtonSettings(Button button) {
            lock(_buttonSettings) {
                return _buttonSettings.ContainsKey(button) ? _buttonSettings[button] : DefaultButtonSettings;
            }
        }
        
        private bool IsSubscribedTo(ButtonEventTypes buttonEvent) {
            return (_subscribedEvents & buttonEvent) != ButtonEventTypes.None;
        }
  
        private void ButtonChangeCallback(object? sender, GenericJoystick.ButtonEventArgs args) {            
            ButtonChangeHandler((Button) args.Button, args.Pressed);
        }

        private void ButtonChangeHandler(Button button, bool pressed) {            
            // If we dont have a callback, no need to think harder
            if (_buttonAction == null)
                return;            

            // If the button isnt known yet, we add it - and ignore the event (always ignore init basically)
            if (!_buttonState.ContainsKey(button)) {                
                _buttonState[button] = new ButtonState() { LastEventElapsedTime = _stopwatch.Elapsed, Pressed = false };
                return;
            }            

            ButtonState currentState = _buttonState[button];              
            ButtonSettings settings = GetButtonSettings(button);
            
            // Every event we (re)start the clock, as we only ever care about delta between events            
            TimeSpan timeSinceLastEvent = _stopwatch.Elapsed - currentState.LastEventElapsedTime;
            currentState.LastEventElapsedTime = _stopwatch.Elapsed;            

            // Ignore dupes
            if (currentState.Pressed == pressed) {                
                return;
            }

            // Update state
            currentState.Pressed = pressed;

            // Short click is any transition from press->release that is at least short click duration
            //  but not more than long click duration
            if (IsSubscribedTo(ButtonEventTypes.ShortClick) && pressed == false &&
                    timeSinceLastEvent.TotalMilliseconds >= settings.GetShortClickMinimumDurationMilliseconds() &&
                    timeSinceLastEvent.TotalMilliseconds < settings.GetLongClickMinimumDurationMilliseconds()) {
                        _buttonAction(button, ButtonEventTypes.ShortClick, timeSinceLastEvent.TotalMilliseconds);
            }                            
            
            // If we are releasing, and it has been long enough, then generate a long click
            if (IsSubscribedTo(ButtonEventTypes.LongClick) && pressed == false && 
                timeSinceLastEvent.TotalMilliseconds >= settings.GetLongClickMinimumDurationMilliseconds()) {
                _buttonAction(button, ButtonEventTypes.LongClick, timeSinceLastEvent.TotalMilliseconds);
            } 
            
            if (IsSubscribedTo(ButtonEventTypes.Press) && pressed) {
                _buttonAction(button, ButtonEventTypes.Press, timeSinceLastEvent.TotalMilliseconds);
            }

            if (IsSubscribedTo(ButtonEventTypes.Release) && !pressed) {
                _buttonAction(button, ButtonEventTypes.Release, timeSinceLastEvent.TotalMilliseconds);
            }
        }

        private Button ButtonFromAxisAndDirection(Axis axis, short direction) {
            switch(axis) {
                case Axis.X1:                
                    return Button.X1;
                case Axis.LT:
                    return Button.LT;
                case Axis.RT:
                    return Button.RT;
                case Axis.DPadUpDown:
                    return direction < 0 ? Button.DPadUp : Button.DPadDown;
                case Axis.DPadLeftRight:
                    return direction < 0 ? Button.DPadLeft : Button.DPadRight;
            }

            // Unknown?
            return Button.StartSyntheticButtons;
        } 

        private void AxisChangeCallback(object? sender, GenericJoystick.AxisEventArgs args) {
            // For now, we just translate based on which axis it is.  Dpad we use < 0 for Left and Up, every other axis is either 
            //  -32k for Release, and 32k for press
            Axis axis = (Axis) args.Axis;            
            switch(axis) {
                case Axis.X1:   // and X2, same value
                case Axis.LT:
                case Axis.RT:
                    {
                        Button button = ButtonFromAxisAndDirection(axis, args.Value);
                        if (args.Value == AXIS_NEGATIVE_MAX) {
                            ButtonChangeHandler(button, false);
                        } else if (args.Value == AXIS_POSITIVE_MAX) { 
                            ButtonChangeHandler(button, true);                            
                        }
                    }
                    break;
                case Axis.DPadLeftRight:
                    if (args.Value == 0) {
                        // This is a release of the Left AND Right, so generate both, handler must remove dupes if useful
                        ButtonChangeHandler(Button.DPadLeft, false);
                        ButtonChangeHandler(Button.DPadRight, false);                        
                    } else if (args.Value == AXIS_NEGATIVE_MAX) {
                        // This is a press of the Left
                        ButtonChangeHandler(Button.DPadLeft, true);
                    } else if (args.Value == AXIS_POSITIVE_MAX) {
                        // This is a press of the Right
                        ButtonChangeHandler(Button.DPadRight, true);
                    }
                    break;
                case Axis.DPadUpDown:                    
                    if (args.Value == 0) {
                        // This is a release of the Up AND Down, so generate both, handler must remove dupes if useful
                        ButtonChangeHandler(Button.DPadUp, false);
                        ButtonChangeHandler(Button.DPadDown, false);                        
                    } else if (args.Value == AXIS_NEGATIVE_MAX) {
                        // This is a press of the Up
                        ButtonChangeHandler(Button.DPadUp, true);
                    } else if (args.Value == AXIS_POSITIVE_MAX) {
                        // This is a press of the Down
                        ButtonChangeHandler(Button.DPadDown, true);
                    }
                    break;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    _gamepad.Dispose();
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