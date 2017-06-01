using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public abstract class MvpContextBase
    {
        public virtual ModelBinderWrapper ModelBinder { get; }

        public virtual PresenterBinderWrapper PresenterBinder { get; }
    }

    /// <summary>
    /// Context provided to Presenters and Views for binding.
    /// </summary>
    public class MvpContext : MvpContextBase
    {

        public virtual IControlBinderFactory ControlBinderFactory { get; }

    }

    public class MvpControlBindingContext : MvpContextBase
    {
    }
}
