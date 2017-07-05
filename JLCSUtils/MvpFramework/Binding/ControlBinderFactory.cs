using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Collections;
using System.Reflection;
using DiExtension;
using DiExtension.AutoFactory;
using MvpFramework.Dialog;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Returns a control binder for a given control.
    /// </summary>
    public class ControlBinderFactory : IControlBinderFactory
    {
        public ControlBinderFactory(IDiResolver diResolver = null)
        {
            this.DiResolver = diResolver;
        }

        /// <summary>
        /// Create a binder for the given control.
        /// </summary>
        /// <param name="control">The control to get a binder for.</param>
        /// <returns>The binder for the given control, or null if the control is not to be bound.</returns>
        public virtual IControlBinder Create(object control)
        {
            if (DiResolver != null)
            {   // Look for a registered binder:
                var binder = GetBinderForControl(control);
                if (binder != null)
                    return binder;
            }

            if (control is IControlBinder)                      // if the control implements the interface itself
                return control as IControlBinder;               // use it
            var boundControlAttribute = control.GetType().GetCustomAttribute<MvpBoundControlAttribute>();
            if (boundControlAttribute != null)                    // if it has this attribute
                return boundControlAttribute.BindControl(control);
            else
                return FallbackBinder(control);
        }

        /// <summary>
        /// Create a binder for a given control based using its mapped binder type.
        /// </summary>
        /// <param name="control">The control to get a binder for.</param>
        /// <returns>The binder for the given control, or null if none is found.</returns>
        public virtual IControlBinder GetBinderForControl(object control)
        {
            var binderClass = BinderClassMappings.TryGetValue(control.GetType());
            //TODO?: if not found this way, search mappings for closest superclass of control, or an interface implemented by it,
            //  and add mapping for control.GetType() (to speed up future searches).

            if(binderClass != null)
                return DiResolver.GetInstance<IControlBinder>(binderClass);
            else
                return null;

            //TODO:
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
            // - by a naming convention;
            // - by type mapping in DI container (get implementation of IControlBinder<T> where T is the control type); or
            // - by scanning for an IControlBinder implementation with an attribute referencing the control type. *DONE*
            // Do this mapping first.
            // Cache the mappings.
        }

        /// <summary>
        /// Return a default binder for a control. To be used when other methods have failed.
        /// </summary>
        /// <param name="control">The control to get a binder for.</param>
        /// <returns>The binder for the given control, or null if the control is not to be bound.</returns>
        protected virtual IControlBinder FallbackBinder(object control)
        {
            return GeneralControlBinder.TryCreateBinder(control as Control, () => DiResolver.GetInstance<IMessageDialogService>(typeof(IMessageDialogService)));
            //TODO: Avoid the reference to IMessageDialogService (by populating method arguments from DI by reflection).
        }

        /// <summary>
        /// Populate the mappings of control types to binder types, based on attributes on types in a specified assembly.
        /// </summary>
        /// <param name="assm">The assembly to scan.</param>
        public virtual void Scan(Assembly assm)
        {
            foreach(var t in assm.GetTypes().Where(t => t.IsDefined<ControlBinderAttribute>(false)))
            {
                BinderClassMappings[t] = t.GetCustomAttribute<ControlBinderAttribute>().ForControl;
            }
        }

        /// <summary>
        /// Mappings of control types to binder class types.
        /// </summary>
        protected readonly IDictionary<Type, Type> BinderClassMappings = new Dictionary<Type, Type>();

        protected readonly IDiResolver DiResolver;
    }
}
