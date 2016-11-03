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
        //|TODO: return value?
        //|  Rename to avoid name conflicts in implementing classes.
        //|  Change to:  object ShowView()  ?
        //| should Show() be on base interface at all? A View could be for a non-form (panel, etc.).

        void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory);

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        void Refresh();
    }
    /* OR
    public interface IView<TResult>
    {
        TResult Show();
            // return value?
    }
    */


}
