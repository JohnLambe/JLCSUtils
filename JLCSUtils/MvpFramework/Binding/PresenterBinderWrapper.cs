using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public class PresenterBinderWrapper
    {
        public PresenterBinderWrapper(IPresenter presenter)
        {
            this.Presenter = presenter;
        }

        protected readonly IPresenter Presenter;

        /*
        public virtual EventHandler GetHandler(string key)
        {
            return Presenter.GetType()
                .GetMethods(System.Reflection.BindingFlags.Public)
                // get attributes
                .Where(m => key.Equals(GetMethodKey(m)));
        }

        public virtual GetHandlers(string filter)
        {

        }

        protected virtual string GetMethodKey()
        {

        }
        */
    }
}
