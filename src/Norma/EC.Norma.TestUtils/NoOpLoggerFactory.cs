using System;
using Microsoft.Extensions.Logging;

namespace EC.Norma.TestUtils
{
    public class NoOpLoggerFactory : ILoggerFactory, IDisposable
    {
        public static readonly NoOpLoggerFactory Instance = new NoOpLoggerFactory();

        /// <inheritdoc />
        /// <remarks>
        /// This returns a <see cref="T:Microsoft.Extensions.Logging.Abstractions.NullLogger" /> instance which logs nothing.
        /// </remarks>
        public ILogger CreateLogger(string name)
        {
            return (ILogger) NoOpLogger.Instance;
        }

        /// <inheritdoc />
        /// <remarks>This method ignores the parameter and does nothing.</remarks>
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}