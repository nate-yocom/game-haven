using Microsoft.Extensions.Logging;

using Nfw.Linux.Joystick.Smart;

namespace Nfw.Linux.Joystick.Xpad {
    public class XboxGamepad : Nfw.Linux.Joystick.Smart.Joystick {
        // this, ID, EventType, Value, Duration Since Last
        public Action<XboxGamepad, Button, ButtonEventTypes, bool, TimeSpan>? XboxButtonCallback;
        // this, ID, Value, Duration Since Last
        public Action<XboxGamepad, Axis, short, TimeSpan>? XboxAxisCallback;

        public bool TreatAxisAsButtons { get; set; } = false;          


        public XboxGamepad(string deviceFile, ILogger? logger, ButtonEventTypes subscribeTo) : base(deviceFile, logger, subscribeTo) {            
        }

        public XboxGamepad(string deviceFile, ButtonEventTypes subscribeTo) : this(deviceFile, null, subscribeTo) {            
        }

        public XboxGamepad(ILogger logger, ButtonEventTypes subscribeTo) : this(DEFAULT_DEVICE, logger, subscribeTo) {
        }

        public XboxGamepad(ButtonEventTypes subscribeTo) : this(DEFAULT_DEVICE, null, subscribeTo) {            
        }
        
        public XboxGamepad() : this(DEFAULT_DEVICE, null, ButtonEventTypes.All) {
        }        

        public void SetButtonSettings(Button button, ButtonSettings settings) {
            base.SetButtonSettings((byte) button, settings);
        }
        
        public void ClearButtonSettings(Button button) {
            base.ClearButtonSettings((byte) button);
        }

        protected override void AxisChangeCallback(byte axis, short value) {
            // If we are treating axis normally, just call base class
            if (!TreatAxisAsButtons) {
                base.AxisChangeCallback(axis, value);
                return;
            }

            // Map to button, then fire that instead
        }

        protected override void InvokeSmartButtonCallback(byte key, ButtonEventTypes eventType, bool pressed, TimeSpan elapsed) {
            XboxButtonCallback?.Invoke(this, (Button) key, eventType, pressed, elapsed);
            base.InvokeSmartButtonCallback(key, eventType, pressed, elapsed);
        }

        protected override void InvokeSmartAxisCallback(byte axis, short value, TimeSpan elapsed) {
            XboxAxisCallback?.Invoke(this, (Axis) axis, value, elapsed);
            base.InvokeSmartAxisCallback(axis, value, elapsed);
        }
    }
}