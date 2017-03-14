using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    public class EmbeddableView : ViewBase, IEmbeddedView, IControlBinderExt
    {
        /// <summary><see cref="IEmbeddedView.ViewId"/></summary>
        public virtual string ViewId { get; set; }

        public virtual object BoundControl => this;

        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {

        }
    }
}
