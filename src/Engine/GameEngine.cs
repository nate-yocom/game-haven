using GameHaven.Engine.Diagnostics;

namespace GameHaven.Engine {
    public class GameEngine : IDisposable {
        public const string DEFAULT_SETTINGS_FILE_PATH = "data/game_settings.json";        

        private readonly ILogger _logger;
        private readonly string _settingsFilePath;
        private bool disposedValue;

        // Cheater: Should register with injection controller as a singleton, but ... eh.      
        public static GameEngine? ThereCanBeOnlyOne { get; private set; }

        public GameEngine() : this(DEFAULT_SETTINGS_FILE_PATH) {
        }

        public GameEngine(string settingsFilePath) {
            _logger = Logging.GetLogger("GameEngine");
            _settingsFilePath = settingsFilePath;
            ThereCanBeOnlyOne = this;
        }

        public void Initialize() {           
        }
        
        
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                }
                
                disposedValue = true;
            }
        }

        public void Dispose() {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}