using GameHaven.Engine.Renderer;
using GameHaven.Engine.Diagnostics;
using GameHaven.Engine.Controller;

namespace GameHaven.Engine {

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

        private Xpad _xpad;
        
        public AsyncFrameBufferDisplay Display { get; private set; }

        public StateEngine() : this(DEFAULT_SETTINGS_FILE_PATH) {
        }

        public StateEngine(string settingsFilePath) {
            _logger = Logging.GetLogger("StateEngine");
            _settingsFilePath = settingsFilePath;
            
            Display = new AsyncFrameBufferDisplay();
            Display.Clear();
        }

        public void Initialize() {               
            // Test image from: https://commons.wikimedia.org/wiki/File:Test.svg
            Display.DisplayImage("data/test-image.png");
            _xpad = new Xpad(Xpad.DEFAULT_DEVICE_FILE, (b, e, d) => {
                _logger.LogDebug($"Button: {b} => {e} [{d}]");
            });
        }
        
        
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _xpad.Dispose();                    
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