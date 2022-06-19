using System.Collections.Generic;
using Raven.NET.Core.Configuration;

namespace Raven.NET.Core.Providers.Interfaces
{
    public interface IRavenSettingsProvider
    {
        Dictionary<string, RavenSettings> GetRavens();
        RavenSettings GetRaven(string name);
    }
}