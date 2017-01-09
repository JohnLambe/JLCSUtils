using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.Attributes
{
    /// <summary>
    /// Tags a class that can have auto-wired events.
    /// </summary>
    public interface IHasAutoWiredEvent
    {
    }

    /// <summary>
    /// Enables handling of auto-wired events in the attributed class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HasAutoWiredEventHandlerAttribute : Attribute
    {
    }
}
