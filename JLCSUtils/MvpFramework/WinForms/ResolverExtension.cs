using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Extension;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// <see cref="IResolverExtension"/> implementation for WinForms, that opens each view in a new window.
    /// </summary>
    public class ResolverExtension : ResolverExtensionForExplicitScopeContext
    {

        public override ResolverExtensionStatus AfterCreateView<TView>(ResolverExtensionContext context, ref TView view)
        {
            if (!context.Nested)                  
            {                                       // don't do this for embedded views, because they either already exist within another view or are placed in it.
                new WrapperForm(view as Control);   // wrap in a window
            }

            return base.AfterCreateView(context, ref view);
        }

    }
}
