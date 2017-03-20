using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    public interface IPresenterBase
    {
    }

    /// <summary>
    /// Interface of an MVP Presenter, as seen by other Presenters.
    /// </summary>
    public interface IPresenter : IPresenterBase
    {
        /// <summary>
        /// Show the Presenter.
        /// May be modal or non-modal (derived interfaces may specify which).
        /// </summary>
        /// <returns>Depends on the Presenter (may be defined on the derived interface). May be null.</returns>
        object Show();
        //| TODO: Consider moving this to IWindowPresenter  (with derived interfaces having their own way to show them).
        //| TODO: Consider using a different name, to avoid conflict with a 'Show' method of classes that may be used as base classes of implementations of this.
        //| This could use a generic type for the return value.
    }

}
