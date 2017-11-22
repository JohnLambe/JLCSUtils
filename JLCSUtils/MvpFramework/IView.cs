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
    /// <remarks>
    /// Note: Presenters should reference only their own View. Any reference to other Views is through the Presenters of those Views.
    ///  Models cannot reference Views or Presenters.
    /// </remarks>
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
    /// Control which either is a view nested within another one, or is a parent control into which a nested view can be inserted 
    /// (by assigning <see cref="INestableView.ViewParent"/> to this).
    /// </summary>
    public interface INestedView
    {
        /// <summary>
        /// ID of nested/embedded View.
        /// null or "" to not treat this as a nested/embedded view (handled as an ordinary control).
        /// <para>Mapped to Presenter and Model.</para>
        /// </summary>
        //TODO?: For later version: "/" to map this view to the main Presenter and Model.
        // String beginning with "/": "/" is removed and the rest of the string is mapped as if it was specified in a view embedded in the outer one (for views nested in nested ones).
        string ViewId { get; }
    }

    public interface INestedViewPlaceholder : INestedView
    {
        /// <summary>
        /// Set the nested view to be contained in this control.
        /// </summary>
        /// <param name="nestedView">The nested view.</param>
        void SetNestedView(INestableView nestedView);
    }

    /// <summary>
    /// View which can be embedded in another View.
    /// </summary>
    public interface IEmbeddedView : INestedView, IView
    {
    }

    /// <summary>
    /// A View that be placed in another View.
    /// </summary>
    /// <seealso cref="INestedView"/>
    public interface INestableView : IView
    {
        object ViewParent { get; set; }
    }
    
    /// <summary>
    /// View which can contain nested Views.
    /// </summary>
    public interface IContainerView : IView
    {
        /// <summary>
        /// Find a nested view in this one.
        /// <para>
        /// If this view is an <see cref="IEmbeddedView"/>, it is returned.
        /// </para>
        /// <para>
        /// If this view contains a placeholder (<see cref="INestedViewPlaceholder"/>) for this view, null is returned,
        /// and <paramref name="viewParent"/> is the control into which the view should be placed.
        /// </para>
        /// <para>
        /// If no nested view with the given ID is defined, both the return value and <paramref name="viewParent"/> are null.
        /// </para>
        /// </summary>
        /// <param name="nestedViewId">The ID of the view to locate within this one.</param>
        /// <param name="viewParent">The control which contains or can contain the specified view.</param>
        /// <returns>The requested view.</returns>
        IView GetNestedView(string nestedViewId, out INestedViewPlaceholder viewParent);
    }

    public interface IValidatableView
    {
        /// <inheritdoc cref="ViewBinderBase{TControl}.ValidateModel"/>
        bool ValidateModel();
    }
}
