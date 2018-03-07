using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public abstract class MvpContextBase
    {
        public MvpContextBase(ModelBinderWrapper modelBinder, PresenterBinderWrapperBase presenterBinder, IViewBinder viewBinder)
        {
            this.ModelBinder = modelBinder;
            this.PresenterBinder = presenterBinder;
            this.ViewBinder = viewBinder;
        }

        public virtual ModelBinderWrapper ModelBinder { get; }

        public virtual PresenterBinderWrapperBase PresenterBinder { get; }

        public virtual IViewBinder ViewBinder { get; }
    }

    /// <summary>
    /// Context provided to Presenters and Views for binding.
    /// </summary>
    public class MvpContext : MvpContextBase
    {
        public MvpContext(ModelBinderWrapper modelBinder, PresenterBinderWrapperBase presenterBinder, ControlBinderFactory controlBinderFactory, IViewBinder viewBinder = null)
            : base(modelBinder, presenterBinder, viewBinder)
        {
            ControlBinderFactory = controlBinderFactory;
        }

        public virtual IControlBinderFactory ControlBinderFactory { get; }
    }

    public class MvpControlBindingContext : MvpContextBase
    {
        public MvpControlBindingContext(ModelBinderWrapper modelBinder, PresenterBinderWrapperBase presenterBinder, IViewBinder viewBinder)
            : base(modelBinder, presenterBinder, viewBinder)
        {
        }
    }
}
