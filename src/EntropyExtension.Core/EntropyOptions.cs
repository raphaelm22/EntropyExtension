using Microsoft.Extensions.Logging;
using System;

namespace EntropyExtension.Core
{
    public class EntropyOptions
    {
        public string ApplicationName { get; set; }
        public Func<string, LogLevel, bool> Filter { get; set; } = (name, level) => true;
        public Func<Exception, bool> FilterException { get; set; } = (exception) => true;
    }
}
