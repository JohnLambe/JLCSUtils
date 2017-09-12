using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    /// <inheritdoc cref="IEmbeddedView" />
    public class EmbeddableView : ViewBase, IEmbeddedView, IControlBinderExt
    {
        /// <summary><see cref="INestedView.ViewId"/></summary>
        public virtual string ViewId { get; set; }

        /// <inheritdoc cref="IControlBinderExt.BoundControl" />
        public virtual object BoundControl => this;

        /// <inheritdoc cref="IControlBinder.BindModel" />
        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
        }

        /// <inheritdoc cref="IControlBinder.MvpRefresh" />
        public virtual void MvpRefresh()
        {
            Refresh();
        }
    }
}
