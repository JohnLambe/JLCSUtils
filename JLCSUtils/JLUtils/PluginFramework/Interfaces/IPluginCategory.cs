using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    public interface IPluginCategory
    {
        /// <summary>
        /// The human-readable name of the category.
        /// </summary>
        string Name { get; }
    }
}
