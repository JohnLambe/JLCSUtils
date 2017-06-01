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
    //  Models cannot reference Views or Presenters.
    public interface IView
    {
        /// <summary>
        /// Bind the View to the Presenter and Model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory);
        //TODO: Replace with void Bind(MvpContext mvpContext);

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        void RefreshView();
    }

    /// <summary>
    /// View which can be embedded in another View.
    /// </summary>
    public interface IEmbeddedView : IView
    {
        /// <summary>
        /// ID of embedded View.
        /// null or "" to not treat this as an embedded view (handled as an ordinary control).
        /// <para>Mapped to Presenter and Model.</para>
        /// </summary>
        //TODO?: For later version: "/" to map this view to the main Presenter and Model.
        // String beginning with "/": "/" is removed and the rest of the string is mapped as if it was specified in a view embedded in the outer one (for views nested in nested ones).
        string ViewId { get; }
    }
}
