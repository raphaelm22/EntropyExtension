using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EntropyExtension.Core
{
    public static class EntropyExtensions
    {

        public static IServiceCollection AddEntropyCore(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddElm();

            serviceCollection.TryAddSingleton<EntropyOptions>();

            return serviceCollection;
        }

        public static IApplicationBuilder UseEntropyCore(this IApplicationBuilder app,
             Action<EntropyOptions> setupAction = null)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.UseElmCapture();

            UseEntropyCore(app.ApplicationServices, setupAction);

            return app;
        }

        public static IServiceProvider UseEntropyCore(this IServiceProvider serviceProvider,
             Action<EntropyOptions> setupAction = null)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                var options = serviceProvider.GetService<EntropyOptions>();
                setupAction?.Invoke(options);

                loggerFactory.AddProvider(new EntropyLoggerProvider(serviceProvider));
            }

            return serviceProvider;
        }

    }
}
