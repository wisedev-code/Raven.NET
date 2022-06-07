using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.NET.Core.Providers;
using Raven.NET.Core.Providers.Interfaces;

namespace Raven.NET.Core.Configuration
{
    public static class RavenBootstrapper
    {
        private const string RavenSectionName = "Raven";
        
        public static void ConfigureRaven(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Dictionary<string, RavenSettings>>(configuration.GetSection(RavenSectionName));
            services.AddTransient<IRavenSettingsProvider, RavenSettingsProvider>();
        }
    }
}