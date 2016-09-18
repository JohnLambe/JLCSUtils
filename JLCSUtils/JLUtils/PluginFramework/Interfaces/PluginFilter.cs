using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.FilterDelegates;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    /// <summary>
    /// Delegate for filtering plugins.
    /// </summary>
    /// <param name="p"></param>
    /// <returns>True to accept the plugin.</returns>
    //    public delegate bool PluginFilterDelegate(IPluginDetails p);

    public class PluginFilter : BooleanExpression<IPluginDetails>
    {
        public PluginFilter(FilterDelegate<IPluginDetails> del)
            : base(del)
        { }
    }

    public class PluginCategoryFilter : PluginFilter
    {
        public PluginCategoryFilter(IPluginCategory category)
            : base(p => p.IsInCategory(category))
        {
        }
    }

    public class PluginClassIdFilter : PluginFilter
    {
        public PluginClassIdFilter(string classId)
            : base(p => p.ClassId.Equals(classId))
        {
        }
    }
}
