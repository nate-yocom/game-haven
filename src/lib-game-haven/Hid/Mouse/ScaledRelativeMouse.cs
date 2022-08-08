using System.Drawing;
using Microsoft.Extensions.Logging;

using GameHaven.Diagnostics;

namespace GameHaven.Hid.Mouse {

    public class ScaledRelativeMouse {
        public const int MAX_X = 32767;
        public const int MAX_Y = 32767;

        private readonly string _devicePath;
        private byte[] _buffer = new byte[7];
        
        private Point _currentPosition = new Point(0, 0); 
        private ILogger _logger;
        private bool _leftButtonToggle = false;

        public Point CurrentPosition {
            get { return _currentPosition; }
        }

        public ScaledRelativeMouse() : this("/dev/hidg1") {
        }

        public ScaledRelativeMouse(string devicePath) {
            _devicePath = devicePath;
            _logger = Logging.GetLogger($"ScaledRelativeMouse[{_devicePath}]");
        }

        public void Center() {
            MoveRelativeToScreen(new PointF(0.50f, 0.50f));
        }

        public void LeftClick() {
            // Click is a push AND release
            WriteMouseMessage(true, false, _currentPosition.X, _currentPosition.Y, 0, 0);
            WriteMouseMessage(false, false, _currentPosition.X, _currentPosition.Y, 0, 0);
        }

        public void RightClick() {
            // Click is a push AND release
            WriteMouseMessage(false, true, _currentPosition.X, _currentPosition.Y, 0, 0);
            WriteMouseMessage(false, false, _currentPosition.X, _currentPosition.Y, 0, 0);
        }

        public void ButtonClick(byte buttons) {
            WriteMouseMessageInternal(buttons, _currentPosition.X, _currentPosition.Y, 0, 0);
            WriteMouseMessageInternal((byte)0x00, _currentPosition.X, _currentPosition.Y, 0, 0);
        }

        public void MoveRelativeToScreen(PointF scaledPosition) {
            Point targetLocation = ToScaledLocation(scaledPosition);            
            // Later, may need to track state of mouse buttons, so we can do click->drag->release?
            WriteMouseMessage(_leftButtonToggle, false, targetLocation.X, targetLocation.Y, 0, 0);
        }

        // Move relative to the current position based on scale - i.e. X = 0.50 means 'move half again to right on X'
        public void MoveScaledRelativeToCurrent(PointF relativeScale) {
            int x = (int) (_currentPosition.X * relativeScale.X);
            int y = (int) (_currentPosition.Y * relativeScale.Y);
            WriteMouseMessage(_leftButtonToggle, false, x, y, 0, 0);
        }

        public void MoveRelativeToCurrent(Point delta) {
            int x = _currentPosition.X + delta.X;
            int y = _currentPosition.Y + delta.Y;
            WriteMouseMessage(_leftButtonToggle, false, x, y, 0, 0);
        }

        public void ScrollVertical(byte delta) {
            WriteMouseMessage(_leftButtonToggle, false, _currentPosition.X, _currentPosition.Y, delta, 0);
        }

        public void ScrollHorizontal(byte delta) {
            WriteMouseMessage(_leftButtonToggle, false, _currentPosition.X, _currentPosition.Y, 0, delta);
        }

        public void ToggleLeftButton() {
            _logger.LogDebug($"Toggling left button Previous: {_leftButtonToggle}");
            _leftButtonToggle = !_leftButtonToggle;
            WriteMouseMessage(_leftButtonToggle, false, _currentPosition.X, _currentPosition.Y, 0, 0);
            _logger.LogDebug($"Toggling left button Now: {_leftButtonToggle}");
        }
        
        private Point ToScaledLocation(PointF scaledPosition) {
            return new Point(
                (int)(MAX_X * scaledPosition.X),
                (int)(MAX_Y * scaledPosition.Y)
            );
        }

        private int BoxVal(int proposed, int max) {
            return Math.Max(Math.Min(proposed, max), 1);
        }

        private void WriteMouseMessage(bool button1, int actualX, int actualY, int vWheelDelta, int hWheelDelta) {
            WriteMouseMessageInternal((byte) (button1 ? 0x01 : 0x00), actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessage(bool button1, bool button2, int actualX, int actualY, int vWheelDelta, int hWheelDelta) { 
            byte buttons = 0x00;
            buttons = (byte)(0x00 | (button1 ? 0x01 : 0x00));
            buttons = (byte)(buttons | (button2 ? 0x02 : 0x00));
            WriteMouseMessageInternal(buttons, actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessage(bool button1, bool button2, bool button3, int actualX, int actualY, int vWheelDelta, int hWheelDelta) { 
            byte buttons = 0x00;
            buttons = (byte)(0x00 | (button1 ? 0x01 : 0x00));
            buttons = (byte)(buttons | (button2 ? 0x02 : 0x00));
            buttons = (byte)(buttons | (button3 ? 0x04 : 0x00));
            WriteMouseMessageInternal(buttons, actualX, actualY, hWheelDelta, hWheelDelta);
        }

        private void WriteMouseMessageInternal(byte buttons, int actualX, int actualY, int vWheelDelta, int hWheelDelta) {
            actualX = BoxVal(actualX, MAX_X);
            actualY = BoxVal(actualY, MAX_Y);

            // Caller disabled left button explicitly
            byte button1on = (byte) (buttons & (byte )0x01);
            if (button1on == 0x00 && _leftButtonToggle) {
                _logger.LogDebug($"Auto-de-flagging left toggle");
                _leftButtonToggle = false;
            }

            _logger.LogDebug($"From: {_currentPosition.X},{_currentPosition.Y} => {actualX},{actualY}, Buttons[{buttons}] LeftToggle[{_leftButtonToggle}]");
            
            _currentPosition.X = actualX;
            _currentPosition.Y = actualY;            

            Array.Clear(_buffer, 0, _buffer.Length);
            _buffer[0] = buttons;            
            _buffer[1] = (byte) (actualX & 0xff);
            _buffer[2] = (byte) ((actualX >> 8) & 0xff);        
            _buffer[3] = (byte) (actualY & 0xff);
            _buffer[4] = (byte) ((actualY >> 8) & 0xff);        
            _buffer[5] = (byte) vWheelDelta;
            _buffer[6] = (byte) hWheelDelta;

            try {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(_devicePath))) {
                    writer.Write(_buffer, 0, _buffer.Length);
                    writer.Flush();
                }
            } catch (Exception ex) {
                _logger.LogWarning($"{ex.Message} while writing");
            }
        }        
    }
}