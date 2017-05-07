using DiExtension.AutoFactory;
using System.Collections.Generic;
using System;
using JohnLambe.Util.Collections;
using System.Reflection;
using System.Linq;
using MvpFramework.Menu;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Interface implemented by a control or by a helper for the control,
    /// to bind the model and/or presenter to it.
    /// </summary>
    public interface IControlBinder
    {
        /// <summary>
        /// Bind the given model and presenter to the control.
        /// </summary>
        /// <param name="modelBinder"></param>
        void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter);
        //TODO: Change to void MvpBind(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenter);

        /// <summary>
        /// Update the control from the model.
        /// </summary>
        void MvpRefresh();
    }

    public interface IControlBinderExt : IControlBinder
    {
        /// <summary>
        /// The control bound by this binder.
        /// </summary>
        object BoundControl { get; }
        //| Type `object` since each UI framework will have its own type(s) for controls.
        //TOOD?: Use type parameter for control class, or control base class.

        //| Could add `void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)` for nested Views
    }

    /// <summary>
    /// Interface to return a binder for a given control.
    /// May return null if the control does not bind to anything.
    /// If not null, the returned instance must not be shared with any other controls (if shared, it would not be possible to refresh just one of them, for example).
    /// <para>
    /// The <see cref="IFactory{IControlBinder, object}.Create(object)"/> method takes
    /// a parameter of a control, and returns the control binder (or null).
    /// </para>
    /// </summary>
    //| TODO?: Could add a generic parameter for the type of the control (the control base class for the UI framework).
    public interface IControlBinderFactory : IFactory<IControlBinder, object>
    {
    }


/*
    public interface IMvpBinding
    {
        IControlBinderFactory ControlBinderFactory { get; }
        IPresenterBinderInterface PresenterBinderInterface { get; }
    }

    public interface IPresenterBinderInterface
    {
        MenuItemModel GetOption(string id);
    }
*/
}
