using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.Attributes
{
    /// <summary>
    /// Tags a class that can have auto-wired events (not to be confused with <seealso cref="HasAutoWiredEventHandlerAttribute"/>,
    /// which is placed on the class of the <i>handlers</i>).
    /// <para>
    /// Events to be injected are attributed with <seealso cref="InjectAttribute"/>.
    /// </para>
    /// </summary>
    public interface IHasAutoWiredEvent
    {
    }

    /// <summary>
    /// Enables handling of auto-wired events in the attributed class.
    /// </summary>
    /// <seealso cref="AutoEventHandlerAttribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HasAutoWiredEventHandlerAttribute : DiAttribute
    {
    }
}
