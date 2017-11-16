using DiExtension;
using DiExtension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Util
{
    public static class WinFormsDiUtil
    {
        /// <summary>
        /// Run property injection on the given control, and optionally, its children.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="control"></param>
        /// <param name="injectChildren">Iff true, the child controls are also injected.</param>
        public static void RunPropertyInjection(IDiContext context, Control control, bool injectChildren = true)
        {
            if(control.GetType().GetCustomAttribute<SupportsInjectionAttribute>()?.Enabled ?? false)
                context.BuildUp(control);

            RunPropertyInjectionOnChildren(context, control);
        }

        /// <summary>
        /// Run property injection on the children of the given control.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="control"></param>
        public static void RunPropertyInjectionOnChildren(IDiContext context, Control control)
        {
            if (control.HasChildren)
            {
                foreach (var c in control.Controls)
                {
                    RunPropertyInjection(context, control);
                }
            }
        }

        //TODO: Inject 
    }
}
