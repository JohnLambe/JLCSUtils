using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public static class EfUtil
    {
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if the given object is either an Entity Framework proxy object or the Type of one.</returns>
        public static bool IsEfClass(object value)
        {
            Type clas = value is Type ? (Type)value : value.GetType();
            return clas?.Namespace?.StartsWith(EfProxyNamespace) ?? false;
        }

        /// <summary>
        /// The namespace of Entity Framework proxies.
        /// (This may change with future versions of Entity Framework.)
        /// </summary>
        private const string EfProxyNamespace = "System.Data.Entity.DynamicProxies";
    }
}
