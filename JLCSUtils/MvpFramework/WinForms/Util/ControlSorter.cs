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
