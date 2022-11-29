using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Raven.NET.Analytics
{
    public static class HostedServiceBootstrapper
    {
        public static void AddRavenAnalytics(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<RavenAnalyticsApi>();
        }
        
        public static void UseRavenAnalytics(this IServiceProvider serviceProvider)
        {
            var hostedService = serviceProvider.GetRequiredService<IHostedService>();
            Task.Run(() => hostedService.StartAsync(CancellationToken.None));
        }
    }
}