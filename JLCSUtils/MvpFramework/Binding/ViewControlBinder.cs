using JohnLambe.Util;
using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// A control binder for the View itself (binds events and properties of the View class).
    /// <para>The bound control is typically a View (usually <see cref="IView"/>) but does not have to be.</para>
    /// </summary>
    public class ViewControlBinder : IControlBinderExt
    {
        /// <summary>
        /// </summary>
        /// <param name="view">The view to be bound.</param>
        public ViewControlBinder(object view)
        {
            BoundControl = view;
        }

        /// <summary>
        /// Returns a binder for a given view.
        /// </summary>
        /// <param name="view">The view to be bound.</param>
        /// <returns>A binder or null, if there is none, or nothing to bind.</returns>
        public static IControlBinder GetBinderForView(object view)
        {
            if (view is IControlBinder)
            {
                return view as IControlBinder;
            }
            else
            {
                var binder = new ViewControlBinder(view);
                /*
                if (binder.HasBindng)
                    return binder;
                else
                    return null;
                */
                return binder;
            }
        }

        /// <summary>
        /// The bound view.
        /// </summary>
        public virtual object BoundControl
        {
            get;
            protected set;
        }

        /// <summary>
        /// True iff anything is bound by this instance.
        /// <para>If false, it does nothing (because it is bound yet, or because there was nothing to bind).</para>
        /// </summary>
        public virtual bool HasBindng { get; protected set; }

        public void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            MvpBind(new MvpContext(modelBinder, new PresenterBinderWrapper(presenter), null));
        }

        public virtual void MvpBind(MvpContext context)  //ModelBinderWrapper modelBinder, PresenterBinderWrapperBase presenterBinder)
        {
            if (BindEvents(BoundControl, context.PresenterBinder))
                HasBindng = true;
            if (BindProperties(BoundControl, context.ModelBinder))
                HasBindng = true;

            if(HasBindng)
                MvpRefresh();    // to populate the property values bound in BindProperties(...)
        }

        public virtual void MvpRefresh()
        {
            _propertyBinding?.Invoke();         // Run the delegate created on binding
        }

        /// <summary>
        /// Binds events on a given object (usually a View) to a given presenter.
        /// </summary>
        /// <param name="target">The object to bind events on.</param>
        /// <param name="presenterBinder">Wrapper of the presenter to bind to.</param>
        /// <returns>True iff anything was bound.</returns>
        public virtual bool BindEvents(object target, PresenterBinderWrapperBase presenterBinder)
        {
            bool anyBinding = false;
            foreach (var eventInfo in target.GetType().GetEvents())
            {
                var attribute = eventInfo.GetCustomAttribute<MvpEventAttribute>();
                if (attribute?.Enabled ?? false)     // if present and Enabled
                {
                    var handler = presenterBinder.GetHandler(attribute.Id ?? eventInfo.Name, null, !attribute.EmptyHandler);
                    if (handler != null)    // if there is a handler
                    {
                        eventInfo.AddEventHandler(target, handler);
                    }
                    else
                    {
                        if (attribute.Required)
                            throw new MvpResolutionException("There are no handlers for " + target.GetType().FullName + "." + eventInfo.Name + " and a handler is required");
                    }
                    anyBinding = true;
                }
            }
            return anyBinding;
        }

        /// <summary>
        /// Builds <see cref="_propertyBinding"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="modelBinder"></param>
        /// <returns>True iff anything was bound.</returns>
        public virtual bool BindProperties(object target, ModelBinderWrapper modelBinder)
        {
            bool anyBinding = false;
            foreach (var targetPropertyInfo in target.GetType().GetProperties())
            {
                var attribute = targetPropertyInfo.GetCustomAttribute<MvpBindAttribute>();
                if (attribute?.Enabled ?? false)     // if present and Enabled
                {
                    if (attribute.Key == MvpBindAttribute.Model)  // if binding the model object itself
                    {
                        if (modelBinder.AsObject != null)   // if the model 
                        {
                            _propertyBinding += () =>
                                targetPropertyInfo.SetValueConverted(target, modelBinder.AsObject);
                        }
                        else if(attribute.Required)   // required and not available
                        {
                            throw new MvpBindingException("Binding Model to " + BoundControl.GetType().Name + "." + targetPropertyInfo.Name
                                + " failed; Target: " + BoundControl + "\nThe Model is not available as an object");
                        }
                    }
                    else   // binding a property of the Model
                    {
                        var property = modelBinder.GetProperty(attribute.Key);
                        _propertyBinding += () =>
                            targetPropertyInfo.SetValueConverted(target, property.Value);
                    }
                    anyBinding = true;
                }
            }
            return anyBinding;
        }

        /// <summary>
        /// Delegate to populate the view properties from the model properties.
        /// </summary>
        protected event VoidDelegate _propertyBinding;
    }
}
