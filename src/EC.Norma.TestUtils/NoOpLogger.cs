using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EC.Norma.TestUtils
{
    public class NoOpLogger : ILogger, IDisposable
    {
        private SortedList<long, LogEvent> logEvents;

        private Stack<(string, string)> States { get; } = new Stack<(string, string)>();

        public IReadOnlyCollection<LogEvent> LogEvents => (IReadOnlyCollection<LogEvent>) this.logEvents.Select<KeyValuePair<long, LogEvent>, LogEvent>((Func<KeyValuePair<long, LogEvent>, LogEvent>) (s => s.Value)).ToList<LogEvent>().AsReadOnly();

        public static NoOpLogger Instance { get; } = new NoOpLogger();

        private NoOpLogger() => this.logEvents = new SortedList<long, LogEvent>();

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state is Dictionary<string, string> source)
                source.Select<KeyValuePair<string, string>, (string, string)>((Func<KeyValuePair<string, string>, (string, string)>) (d => (d.Key, d.Value))).ToList<(string, string)>().ForEach((Action<(string, string)>) (t => this.States.Push(t)));
            return (IDisposable) NoOpLogger.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            LogEvent logEvent = new LogEvent()
            {
                Exception = exception,
                Message = state.ToString(),
                Level = logLevel,
                Properties = this.States.ToList<(string, string)>()
            };
            this.logEvents.Add(logEvent.Moment, logEvent);
        }

        public void Dispose()
        {
            try
            {
                this.States.Pop();
            }
            catch (InvalidOperationException ex)
            {
                this.Log<string>(LogLevel.Error, new EventId(), "No more states to be popped", (Exception) ex, (Func<string, Exception, string>) null);
            }
        }
    }

    public class LogEvent
    {
        public LogEvent()
        {
            Moment = DateTime.UtcNow.Ticks;
        }

        public long Moment { get; }

        public Exception Exception { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public List<(string Property, string Value)> Properties { get; set; }
    }
}
