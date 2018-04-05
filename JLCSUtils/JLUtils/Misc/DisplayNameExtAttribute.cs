using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Attribute for any item that provides a name for displaying in a user interface.
    /// For use when there is code to use it for this purpose, e.g. this can be used on enum values, and a user interface
    /// for displaying or inputting a value of that type could use them.
    /// <para>This is allowed on items that <see cref="DisplayNameAttribute"/> (the base class) is not.
    /// When testing for the presence of this in code, testing for <see cref="DisplayNameAttribute"/> is recommended.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DisplayNameExtAttribute : DisplayNameAttribute
    {
        public DisplayNameExtAttribute() : base(null)
        {
        }
        /// <summary>
        /// </summary>
        /// <param name="displayName"><see cref="DisplayNameAttribute.DisplayName"/></param>
        public DisplayNameExtAttribute(string displayName) : base(displayName)
        {
        }

        /// <summary>
        /// A shorter version of the display name, for use when space is more limited.
        /// <para>
        /// If this is present, and <see cref="DisplayNameAttribute.DisplayName"/> is null, this should NOT be considered
        /// a default for it (if there is another method of getting a default name (for example, using the code name of the attributed item),
        /// it should be used).
        /// </para>
        /// </summary>
        [Nullable]
        public virtual string ShortName { get; set; }

        /*TODO:
         * Or use MvpDisplayNameAny
        /// <summary>
        /// Accelerator character for this item in a user interface.
        /// </summary>
        public virtual char AcceleratorChar { get; set; }

        /// <summary>
        /// Value to display when the value is null.
        /// </summary>
        public virtual string NullText { get; set; }  // or object NullDisplay ?
        */

        /// <summary>
        /// The plural of the display name.
        /// </summary>
        [Nullable]
        public virtual string PluralName { get; set; }
    }
}
