using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Context provided to Presenters and Views for binding.
    /// </summary>
    public class MvpContext
    {
        public virtual ModelBinderWrapper ModelBinder { get; }

        public virtual PresenterBinderWrapper PresenterBinder { get; }

        public virtual IControlBinderFactory ControlBinderFactory { get; }

        // IconRepository
    }
}
