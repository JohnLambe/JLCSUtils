using DiExtension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConsoleApp
{
    // From early version of ConfigInject:

    public abstract class ConsoleInjectAttribute : InjectAttribute
    {
        public ConsoleInjectAttribute()
        {
            Enabled = true;
        }


        //        public virtual int Priority { get; set; }

        //        public virtual string Source { get; set; }

        /*
    /// <summary>
    /// 
    /// e.g. Base Registry key, XPath of parent node, INI file section.
    /// </summary>
    public virtual string Path { get; set; }

    public virtual string PathRelative { get; set; }

    /// <summary>
    /// Value to assign to the property on loading the configuration
    /// if no configured value is available.
    /// </summary>
    public virtual object DefaultValue { get; set; }
*/


        public virtual int ParameterPosition
        {
            get
            {
                if (ParameterPositionMin == ParameterPositionMax)
                    return ParameterPositionMin;
                else
                    return -1;
            }
            set
            {
                ParameterPositionMin = value;
                ParameterPositionMax = value;
            }
        }
        public virtual int ParameterPositionMin { get; set; }
        public virtual int ParameterPositionMax { get; set; }

    }


    public abstract class ConsoleAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates a method or class that can be invoked by the command/operation parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ConsoleCommandAttribute : ConsoleAttribute
    {
    }

}
