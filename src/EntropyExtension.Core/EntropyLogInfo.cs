﻿using Microsoft.AspNetCore.Diagnostics.Elm;
using Microsoft.Extensions.Logging;
using System;

namespace EntropyExtension.Core
{
    public class EntropyLogInfo
    {
        public string ApplicationName { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public LogLevel LogLevel { get; set; }

        public HttpInfo HttpInfo { get; set; }

        public Exception Exception { get; set; }
        public string State { get; set; }

        public string MachineName { get; set; }
    }
}
