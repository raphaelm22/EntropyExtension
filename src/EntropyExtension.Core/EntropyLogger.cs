using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics.Elm;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace EntropyExtension.Core
{
    public class EntropyLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly EntropyOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public EntropyLogger(string categoryName, IServiceProvider serviceProvider)
        {
            _categoryName = categoryName;
            _serviceProvider = serviceProvider;
            _options = serviceProvider.GetService<EntropyOptions>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _options.Filter(_categoryName, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            EntropyLogInfo info;
            try
            {
                info = new EntropyLogInfo
                {
                    LogLevel = logLevel,
                    ApplicationName = _options.ApplicationName,
                    Name = _categoryName,
                    HttpInfo = ElmScope.Current?.Context.HttpInfo,
                    Exception = exception,
                    Time = DateTime.Now,
                    State = formatter?.Invoke(state, exception),
                    MachineName = Environment.MachineName
                };

                if (!_options.FilterEntropyLogInfo(info))
                    return;
            }
            catch (Exception ex)
            {
                EntropyInternalError.Save(ex);
                return;
            }

            var stores = _serviceProvider.GetServices<IEntropyStore>();
            foreach (var store in stores)
            {
                Task.Run(delegate
                {
                    try
                    {
                        store.Log(info);
                    }
                    catch (Exception ex)
                    {
                        EntropyInternalError.Save(ex);
                    }
                });
            }
        }
    }
}
