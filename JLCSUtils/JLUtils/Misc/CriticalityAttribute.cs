using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Indicates the criticality of an item.
    /// <para>This could be used by code analysis tools, for example to report unit test coverage
    /// by criticality level, or to enforce rules (e.g. requiring documentation comments) based on it.</para>
    /// </summary>
    public class CriticalityAttribute : Attribute
    {
        public CriticalityAttribute(int level)
        {
            CriticalityLevel = level;
        }

        /// <summary>
        /// Criticality level. Higher values are more critical.
        /// The value should be one of the constants defined in this class,
        /// or a constant defined by the consumer of this library, to use a different scale.
        /// </summary>
        public virtual int CriticalityLevel { get; set; }
        //| An integer, rather than an enumeration, is used, to enable consumers to use their own values/scale.


        // Constants for levels of criticality (values for CriticalityLevel):
        // Those beginning with 'Cockburn_' refer to the Cockburn Scale criticality levels
        // (https://en.wikipedia.org/wiki/Cockburn_Scale ; http://alistair.cockburn.us/Cockburn+Scale ; http://wiki.c2.com/?CockburnScale).

        // Cockburn Scale:
        // "Criticality" is defined by the sentence "A defect could cause loss of ___".
        // Process criticality is defined as the worst probable effect of an unremedied defect.

        /// <summary>
        /// Loss of Life (L)
        /// </summary>
        public const int Cockburn_L   = 90000;
        public const int LifeCritical = Cockburn_L;
        /// <summary>
        /// Loss of Essential Money (E)
        /// </summary>
        public const int Cockburn_E   = 70000;
        /// <summary>
        /// Loss of Discretionary Money (D)
        /// </summary>
        public const int Cockburn_D   = 30000;
        public const int MonetaryLoss = Cockburn_D;
        /// <summary>
        /// Loss of Comfort (C)
        /// </summary>
        public const int Cockburn_C   = 10000;
        public const int NonCritical  =  5000;
    }
}
