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
    /// <summary>
    /// Dependency injection utilities for use with WinForms and DiExtension.
    /// </summary>
    public static class WinFormsDiUtil
    {
        /// <summary>
        /// Run property injection on the given control, and optionally, its children.
        /// <para>
        /// Only types attributed with <see cref="SupportsInjectionAttribute"/> are injected, but all <see cref="Control"/>s have their properties scanned (if <paramref name="injectChildren"/> is true).
        /// The classes must be injectable by the dependency injection implementation.
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="control">The control to have its properties injected.</param>
        /// <param name="injectChildren">Iff true, the child controls of <paramref name="control"/> (recursively) are also injected.</param>
        public static void RunPropertyInjection(this IDiResolver context, Control control, bool injectChildren = true)
        {
            if(control.GetType().GetCustomAttribute<SupportsInjectionAttribute>()?.Enabled ?? false)
                context.BuildUp(control);

            RunPropertyInjectionOnChildren(context, control);
        }

        /// <summary>
        /// Run property injection on the children of the given control.
        /// <para>
        /// This has the same restrictions as <see cref="RunPropertyInjection"/> regarding what is injected.
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="containerControl"></param>
        public static void RunPropertyInjectionOnChildren(IDiResolver context, Control containerControl)
        {
            if (containerControl.HasChildren)
            {
                foreach (var control in containerControl.Controls)
                {
                    RunPropertyInjection(context, control as Control);
                }
            }
        }

        //TODO: Inject Component and IContainer (not WinForms-specific) ?
    }
}
