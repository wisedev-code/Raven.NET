using Microsoft.Extensions.Logging;

namespace Raven.NET.Core.Configuration
{
    public class RavenSettings
    {
        public bool AutoDestroy { get; set; }
        
        public bool? BackgroundWorker { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}