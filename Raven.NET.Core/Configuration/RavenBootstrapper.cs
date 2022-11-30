using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;

namespace Raven.NET.Core.Configuration
{
    public static class RavenBootstrapper
    {
        private const string RavenSectionName = "Raven";
        
        public static void ConfigureRaven(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Dictionary<string, RavenSettings>>(configuration.GetSection(RavenSectionName));
            services.AddTransient<IRavenSettingsProvider, RavenSettingsProvider>();
            
            services.AddTransient<IRavenWatcher, RavenWatcher>();
            services.AddTransient<IRavenProvider, RavenProvider>();
            services.AddTransient<IRavenTypeWatcher, RavenTypeWatcher>();

            services.AddSingleton<IRavenStorage, RavenStorage>();
            services.AddSingleton(services);
        }
    }
}