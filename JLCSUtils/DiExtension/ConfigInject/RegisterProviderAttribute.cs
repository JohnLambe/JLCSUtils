using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject
{
    /// <summary>
    /// To flag a provider to be registered automatically on scanning an assembly for providers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,Inherited=false,AllowMultiple=true)]
    public class RegisterProviderAttribute : Attribute
    {
        public virtual int Priority { get; set; }
    }

    /*
     *     /// <para>
    /// Providers can define their own interfaces derived from this, with their own registration parameters.
    /// </para>

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class RegisterRegistryProviderAttribute : RegisterProviderAttribute
    {
        string BaseKey { get; set; }
    }
*/

}
