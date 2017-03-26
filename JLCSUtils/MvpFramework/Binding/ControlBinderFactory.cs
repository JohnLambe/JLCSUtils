using DiExtension.AutoFactory;
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

        public virtual IControlBinder Create(object control)
        {
            if (DiResolver != null)
            {
                var binder = GetBinderForControl(control);
                if (binder != null)
                    return binder;
            }

            if (control is IControlBinder)                      // if the control implements the interface itself
                return control as IControlBinder;               // use it
            else if (control.GetType().IsDefined<MvpBoundControlAttribute>())
                return new AttributedControlBinder(control);
            else
                return FallbackBinder(control);
            //TODO:
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
            // - by a naming convention;
            // - by type mapping in DI container (get implementation of IControlBinder<T> where T is the control type); or
            // - by scanning for an IControlBinder implementation with an attribute referencing the control type.
            // Do this mapping first.
            // Cache the mappings.
        }


        public virtual IControlBinder GetBinderForControl(object control)
        {
            var binderClass = BinderClassMappings.TryGetValue(control.GetType());
            //TODO?: if not found this way, search mappings for closest superclass of control, or an interface implemented by it,
            //  and add mapping for control.GetType() (to speed up future searches).

            if(binderClass != null)
                return DiResolver.GetInstance<IControlBinder>(binderClass);
            else
                return null;
        }

        protected virtual IControlBinder FallbackBinder(object control)
        {
            return GeneralControlBinder.TryCreateBinder(control as Control);
        }

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
