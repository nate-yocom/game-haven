using Microsoft.Extensions.Logging;

namespace GameHaven.Engine.Diagnostics {
    public static class Logging {
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();

        public static ILogger<T> GetLogger<T>() { return LoggerFactory.CreateLogger<T>(); }
        public static ILogger GetLogger(string name) { return LoggerFactory.CreateLogger(name); }        
    }
}