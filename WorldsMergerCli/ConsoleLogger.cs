using System;
using Microsoft.Extensions.Logging;

namespace WorldsMergerCli {
    public class ConsoleLogger : ILogger {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            Console.WriteLine($"{DateTime.Now} | [{logLevel}] | {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return new DisposableFake();
        }
    }
    
    public class DisposableFake : IDisposable {
        public void Dispose() { }
    }
}