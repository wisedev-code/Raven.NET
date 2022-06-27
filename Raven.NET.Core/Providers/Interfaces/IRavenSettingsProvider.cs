using System.Collections.Generic;
using Raven.NET.Core.Configuration;

namespace Raven.NET.Core.Providers.Interfaces
{
    /// <summary>
    /// Interface that provides ravens configuration for application settings providers
    /// </summary>
    public interface IRavenSettingsProvider
    {
        /// <summary>
        /// Get configuration for all ravens
        /// </summary>
        /// <returns></returns>
        Dictionary<string, RavenSettings> GetRavens();
        
        /// <summary>
        /// Get raven configuration by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        RavenSettings GetRaven(string name);
    }
}