using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace WorldsMergerGui.Models {
    public class ReactiveLogger : ILogger {
        public ISubject<string> OnLog { get; } = new Subject<string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            OnLog.OnNext($"{DateTime.Now} | [{logLevel}] | {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return Disposable.Empty;
        }
    }
}