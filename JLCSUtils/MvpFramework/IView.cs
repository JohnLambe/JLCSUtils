using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using MvpFramework.Binding;

namespace MvpFramework
{
    /// <summary>
    /// Base interface for Views - the interface to the View from the Presenter.
    /// </summary>
    // Note: Presenters reference only their own View. Any reference to other forms is through the Presenters of those forms.
    //  Models cannot reference Views.
    public interface IView
    {
        void Show();
        //* return value?
        //* should Show() be on base interface at all? A View could be for a non-form (panel, etc.).

        void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory);
    }
    /* OR
    public interface IView<TResult>
    {
        TResult Show();
            // return value?
    }
    */

}
