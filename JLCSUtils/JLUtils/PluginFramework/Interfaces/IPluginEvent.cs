using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    /// <summary>
    /// Base interface for events.
    /// </summary>
    public interface IPluginEvent
    {
//        /// <summary>
//        /// Hierarchical ID of the event type.
//        /// </summary>
//        string EventTypeId { get; }

        string Name { get; }
    }


}
