using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Providers.Interfaces;

namespace Raven.NET.Core.Providers
{
    public class RavenSettingsProvider : IRavenSettingsProvider
    {
        private readonly IOptionsMonitor<Dictionary<string, RavenSettings>> _ravenMonitor;

        public RavenSettingsProvider(IOptionsMonitor<Dictionary<string, RavenSettings>> ravenMonitor)
        {
            _ravenMonitor = ravenMonitor;
        }

        public Dictionary<string, RavenSettings> GetRavens()
            => _ravenMonitor.CurrentValue;

    }
}