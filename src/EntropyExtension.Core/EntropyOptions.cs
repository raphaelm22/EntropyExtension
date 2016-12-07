using Microsoft.Extensions.Logging;
using System;

namespace EntropyExtension.Core
{
    public class EntropyOptions
    {
        public string ApplicationName { get; set; }
        public Func<string, LogLevel, bool> Filter { get; set; } = (name, level) => true;
        public Func<EntropyLogInfo, bool> FilterEntropyLogInfo { get; set; } = (info) => true;
    }
}
