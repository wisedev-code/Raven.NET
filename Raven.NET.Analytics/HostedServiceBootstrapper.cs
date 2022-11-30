
using Microsoft.Extensions.DependencyInjection;

namespace Raven.NET.Analytics
{
    public static class HostedServiceBootstrapper
    {
        public static void AddRavenAnalytics(this IServiceCollection serviceCollection, bool populateRandomData = false)
        {
            RavenAnalyticsApi.PopulateRandomData = populateRandomData;
            serviceCollection.AddHostedService<RavenAnalyticsApi>();
        }
    }
}