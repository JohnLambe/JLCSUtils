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
    // Note: Presenters reference only their own View. Any reference to other Views is through the Presenters of those Views.
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

        //event EventHandler ViewOpened;  // or 'Shown' ?  The arguments can indicate whether it is modal or not.

        /// <summary>
        /// Fired when the view is closing.
        /// <para>It can be fired before closing (<see cref="ViewClosingEventArgs.Closed"/> is false), giving handlers the option of preventing it from closing.
        /// Views are not required to support that.
        /// If it is fired with <see cref="ViewClosingEventArgs.Closed"/>==false, it must still be fired again with <see cref="ViewClosingEventArgs.Closed"/>==true (when it is actually closed).
        /// </para>
        /// </summary>
        event ViewClosingDelegate ViewClosing;
    }
    /* OR
    public interface IView<TResult>
    {
        TResult Show();
            // return value?
    }
    */

}
