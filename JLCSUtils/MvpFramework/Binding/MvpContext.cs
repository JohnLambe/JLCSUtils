using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public abstract class MvpContextBase
    {
        public MvpContextBase(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
        {
            ModelBinder = modelBinder;
            PresenterBinder = presenterBinder;
        }

        public virtual ModelBinderWrapper ModelBinder { get; }

        public virtual PresenterBinderWrapper PresenterBinder { get; }
    }

    /// <summary>
    /// Context provided to Presenters and Views for binding.
    /// </summary>
    public class MvpContext : MvpContextBase
    {
        public MvpContext(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder, ControlBinderFactory controlBinderFactory)
            : base(modelBinder, presenterBinder)
        {
            ControlBinderFactory = controlBinderFactory;
        }

        public virtual IControlBinderFactory ControlBinderFactory { get; }
    }

    public class MvpControlBindingContext : MvpContextBase
    {
        public MvpControlBindingContext(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
            : base(modelBinder, presenterBinder)
        {
        }
    }
}
