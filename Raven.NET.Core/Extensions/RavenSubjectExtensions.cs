using System.Collections.Generic;
using System.Text;

namespace Raven.NET.Core.Extensions
{
    public static class RavenSubjectExtensions
    {
        
        /// <summary>
        /// Create string based on object reflection to store current value in memory cache
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string CreateCacheValue(this object obj, string propName = null)
        {
            var sb = new StringBuilder();
            if (obj.GetType().IsValueType || obj is string)
                sb.AppendFormat("{0}_{1}|", propName, obj);
            else
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if(prop.Name == "Observers") continue;
                    if (typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType))
                    {
                        var get = prop.GetGetMethod();
                        if (!get.IsStatic && get.GetParameters().Length == 0)
                        {
                            var collection = (IEnumerable<object>)get.Invoke(obj, null);
                            if (collection != null)
                                foreach (var o in collection)
                                    sb.Append(o.CreateCacheValue(prop.Name));
                        }
                    }
                    else
                        sb.AppendFormat("{0}{1}_{2}|", propName, prop.Name, prop.GetValue(obj, null));
                }
            return sb.ToString();
        }
    }
}