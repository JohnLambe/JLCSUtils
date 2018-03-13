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
        /// <param name="presenter"></param>
        void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter);

        /// <summary>
        /// Update the control from the model.
        /// </summary>
        void MvpRefresh();
    }

    public interface IControlBinderV2 : IControlBinder
    {
        /// <summary>
        /// Bind the model and presenter to this control.
        /// </summary>
        /// <param name="context"></param>
        void MvpBind(MvpControlBindingContext context);

        /// <summary>
        /// Whether the value in the control can be modified.
        /// Setting this has no effect if the control does not have a data value (e.g. a button).
        /// </summary>
        bool ReadOnly { get; set; }
    }

    public interface IValidateableControl
    {
        /// <summary>
        /// Validate the value in the model bound to this control.
        /// </summary>
        /// <returns></returns>
        bool Validate(ControlValidationOptions options);

        /// <summary>
        /// When controls on a form are validated, reporting of errors or focussing of controls with invalid values,
        /// is done in this order (ascending order of this value).
        /// </summary>
        int ValidationOrder { get; }
    }

    public enum ControlValidationOptions
    {
        /// <summary>
        /// Update the UI to indicate whether the control is valid. This may display the validation error modelessly (such as beside the control),
        /// or just highlight the control (icon, change in style, etc.).
        /// </summary>
        Highlight = 1,
        /// <summary>
        /// If the value is invalid, focus the control, show a modal error, or otherwise get the user to correct the value.
        /// </summary>
        Enter
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
    /// <para>
    /// The <see cref="IFactory{IControlBinder, T}.Create(T)"/> method takes
    /// a parameter of a control, and returns the control binder (or null).
    /// </para>
    /// <para>
    /// May return null if the control does not bind to anything.
    /// If not null, the returned instance must not be shared with any other controls (if shared, it would not be possible to refresh just one of them, for example).
    /// </para>
    /// </summary>
    //| TODO?: Could add a generic parameter for the type of the control (the control base class for the UI framework).
    public interface IControlBinderFactory : IFactory<IControlBinder, object>
    {
    }


    public static class ControlBinderExtension
    {
        public static int GetValidationOrder(this IControlBinder binder)
        {
            return (binder as IValidateableControl)?.ValidationOrder ?? 0;
        }

        /*
        public static void BindModel(this IControlBinderV2 binder, ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            binder.MvpBind(new MvpControlBindingContext(modelBinder, new PresenterBinderWrapper(presenter), null));
        }
        */
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
