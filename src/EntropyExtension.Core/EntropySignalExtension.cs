using Microsoft.Extensions.Logging;
using System;

namespace EntropyExtension.Core
{
    public static class EntropySignalExtension
    {
        public static void EntropySignal(this IServiceProvider serviceProvider, Exception exception)
        {
            var entropyLogger = new EntropyLogger("Manual", serviceProvider);
            entropyLogger.Log<object>(LogLevel.Error, 0, null, exception, null);
        }
    }
}
