using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Util
{
    public class ControlSorter
    {
        public static IQueryable<Control> SortControls(IQueryable<Control> controls, bool descending = false)
        {
            if(!descending)
                return controls.OrderBy(c => GetControlFullTabOrder(c));
            else
                return controls.OrderByDescending(c => GetControlFullTabOrder(c));
        }

        /// <summary>
        /// Returns a value such that sorting all controls on a form by this value
        /// will give the order the order in which focus moves when pressing TAB in WinForms
        /// (based on the <see cref="Control.TabIndex"/> of this and all of its ancestors).
        /// <para>
        /// The retuned string for comparison/sorting with <see cref="StringComparison.InvariantCulture"/>.
        /// The format or values returned cannot be depended on - later versions may return different values for the same inputs.
        /// So the returned values are not suitable for storing persistently nor transmission to another application/process instance.
        /// </para>
        /// </summary>
        /// <param name="control"></param>
        /// <returns>String for sorting to give the tab order. If <paramref name="control"/> is null, "" is returned.</returns>
        [return: NotNull]
        public static string GetControlFullTabOrder([Nullable] Control control)
        {
            if (control == null)
                return "";

            string key = "";

            do
            {
                key = control.TabIndex.ToString("X8") + "." + key;  // sub-optimal: reallocates the string
                control = control.Parent;
            } while (control != null);

            return key;
        }

    }
}
