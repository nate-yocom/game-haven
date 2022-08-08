using Microsoft.Extensions.Logging;

using GameHaven.Diagnostics;

namespace GameHaven.Hid.Keyboard {

    public class StandardKeyboard {        
        private readonly string _devicePath;

        private const int KEYBOARD_BUFFER_SIZE = 8;
        private const int MODIFIER_KEY_INDEX = 0;
        private const int FIRST_KEY_INDEX = 2;
        private byte[] _buffer = new byte[KEYBOARD_BUFFER_SIZE];
        
        private ILogger _logger;
        
        public StandardKeyboard() : this("/dev/hidg0") { 
        }

        public StandardKeyboard(string devicePath) {
            _devicePath = devicePath;
            _logger = Logging.GetLogger($"StandardKeyboard[{_devicePath}]");
        }

        // Sends requested key DOWN only
        public void SendSingleEvent(Keys key, Modifiers modifiers) {
            WriteKeyboardMessage(key, modifiers);            
        }

        public void SendSingleEvent(IEnumerable<Keys> keys, Modifiers modifiers) {
            WriteKeyboardMessage(keys, modifiers);            
        }

        private void WriteKeyboardMessage(Keys key, Modifiers modifiers) {
            WriteKeyboardMessage(new Keys[] { key }, modifiers);
        }        

        private void WriteKeyboardMessage(IEnumerable<Keys> key, Modifiers modifiers) {
            Array.Clear(_buffer, 0, _buffer.Length);
            try {
                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(_devicePath))) {                
                    _buffer[MODIFIER_KEY_INDEX] = (byte) modifiers;
                    for(int x = 0; x < key.Count() && (FIRST_KEY_INDEX + x < _buffer.Length); x++)
                        _buffer[FIRST_KEY_INDEX + x] = (byte) key.ElementAt(x);
                    writer.Write(_buffer, 0, _buffer.Length);
                    writer.Flush();        
                }
            } catch (Exception ex) {
                _logger.LogWarning($"{ex.Message} while writing");
            }
        }     
    }
}