using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    public class ResolverExtension : ResolverExtensionBase
    {

        public override ResolverExtensionStatus AfterCreateView<TView>(Type presenterType, object[] param, ref TView view)
        {
            new WrapperForm(view as Control);

            return base.AfterCreateView(presenterType, param, ref view);
        }

    }
}
