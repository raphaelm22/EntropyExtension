using Microsoft.Extensions.Logging;
using System;

namespace EntropyExtension.Core
{
    public class EntropyLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EntropyLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new EntropyLogger(categoryName, _serviceProvider);
        }

        public void Dispose()
        {}
    }
}
