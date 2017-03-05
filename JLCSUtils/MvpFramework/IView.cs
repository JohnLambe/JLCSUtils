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
        void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory);

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        void RefreshView();

    }

}
