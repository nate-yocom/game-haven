using GameHaven.Engine;
using GameHaven.Engine.Diagnostics;

namespace GameHaven {    
    public class GameHavenWorker : BackgroundService
    {
        private readonly ILogger<GameHavenWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GameHavenWorker(ILogger<GameHavenWorker> logger, IServiceScopeFactory scopeFactory) {
            _logger = logger;
            _serviceScopeFactory = scopeFactory;
        }

        private CancellationTokenSource _internalStopToken = new CancellationTokenSource();
        private CancellationTokenSource _shutdownComplete = new CancellationTokenSource();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {        
            using(IServiceScope scope = _serviceScopeFactory.CreateScope()) {
                
                Logging.LoggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();            
                            
                using(GameEngine engine = new GameEngine()) {
                    // Initialize display, controller, etc, enters default mode
                    engine.Initialize();
                    
                    // Endless loop until stopped...
                    _logger.LogInformation("ExecuteAsync() running"); 
                    while (!stoppingToken.IsCancellationRequested && !_internalStopToken.Token.IsCancellationRequested)
                    {                                           
                        await Task.Delay(1000, stoppingToken);
                    }            
                                
                    _logger.LogInformation("ExecuteAsync() signaled to stop");
                }
                _shutdownComplete.Cancel();     
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            // Tell ourselves to stop
            _internalStopToken.Cancel();
            _logger.LogInformation("stopping");

            // Now wait for ack from ExecutAsync()
            while (!_shutdownComplete.IsCancellationRequested) {
                _logger.LogInformation("Waiting for ExecutAsync to cleanup");
                await Task.Delay(1000);
            }
        }
    }
}