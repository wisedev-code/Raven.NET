using System.IO;
using System.Linq;
using System.Reflection;

namespace Raven.NET.Analytics.Misc
{
    public static class Resources
    {
        public static string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            if (!name.StartsWith(nameof(Analytics)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}