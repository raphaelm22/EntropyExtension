using EntropyExtension.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;


namespace EntropyExtension.SqlServer
{
    public static class EntropySqlServerExtensions
    {
        public static IServiceCollection AddEntropySqlServer(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<IEntropyStore, SqlServerEntropyStore>();
            return services;
        }

        public static IServiceProvider UseEntropySqlServer(this IServiceProvider serviceProvider,
             Action<SqlServerEntropyOptions> setupAction = null)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            setupAction?.Invoke(SqlServerEntropyStore.Options);

            return serviceProvider;
        }

        public static IApplicationBuilder UseEntropySqlServer(this IApplicationBuilder app,
             Action<SqlServerEntropyOptions> setupAction = null)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.ApplicationServices.UseEntropySqlServer(setupAction);

            return app;
        }

    }
}
