using System.Windows.Forms;

using DiExtension.AutoFactory;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Interface implemented by a control or by a helper for the control,
    /// to bind the model and/or presenter to it.
    /// </summary>
    public interface IControlBinder
    {
        /// <summary>
        /// Bind the given model to the control.
        /// </summary>
        /// <param name="modelBinder"></param>
        void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter);
        //| `presenter` could also use a wrapper, similar to ModelBinderWrapper.

        /// <summary>
        /// Update the control from the model.
        /// </summary>
        void Refresh();
    }

    public interface IControlBinderExt : IControlBinder
    {
        /// <summary>
        /// The control bound by this binder.
        /// </summary>
        Control Control { get; }
    }

    /// <summary>
    /// Interface to return a binder for a given controls and presenter.
    /// May return null if the control does not bind to anything.
    /// </summary>
    public interface IControlBinderFactory : IFactory<IControlBinderExt, Control, IPresenter>
    {
    }


    /// <summary>
    /// Returns a control binder for a given control and presenter.
    /// </summary>
    public class ControlBinderFactory : IControlBinderFactory
    {
        public virtual IControlBinderExt Create(Control control, IPresenter presenter)
        {
            if (control is IControlBinderExt)                   // if the control implements the interface itself
                return control as IControlBinderExt;            // use it
            else
                return new GeneralControlBinder(control, presenter);
            //TODO:
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
            // - by a naming convention;
            // - by type mapping in DI container (get implementation of IControlBinder<T> where T is the control type); or
            // - by scanning for an IControlBinder implementation with an attribute referencing the control type.
        }
    }

}
