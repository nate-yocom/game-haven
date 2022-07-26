using GameHaven.Engine.Renderer;
using GameHaven.Engine.Diagnostics;

namespace GameHaven.Engine {
    public class StateEngine : IDisposable {
        public const string DEFAULT_SETTINGS_FILE_PATH = "data/settings.json";        

        private readonly ILogger _logger;
        private readonly string _settingsFilePath;
        private bool _disposed;

        private AsyncFrameBufferDisplay? _display;

        public StateEngine() : this(DEFAULT_SETTINGS_FILE_PATH) {
        }

        public StateEngine(string settingsFilePath) {
            _logger = Logging.GetLogger("StateEngine");
            _settingsFilePath = settingsFilePath;
        }

        public void Initialize() {   
            _display = new AsyncFrameBufferDisplay();
            _display.Clear();

            // Test image from: https://commons.wikimedia.org/wiki/File:Test.svg
            _display.DisplayImage("data/test-image.png");
        }
        
        
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
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