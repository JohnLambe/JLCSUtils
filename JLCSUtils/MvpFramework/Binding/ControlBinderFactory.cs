using DiExtension.AutoFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Returns a control binder for a given control and presenter.
    /// </summary>
    public class ControlBinderFactory : IControlBinderFactory
    {
        public virtual IControlBinder Create(object control)
        {
            if (control is IControlBinder)                      // if the control implements the interface itself
                return control as IControlBinder;               // use it
            else
                return new GeneralControlBinder((Control)control);
            //TODO:
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
            // - by a naming convention;
            // - by type mapping in DI container (get implementation of IControlBinder<T> where T is the control type); or
            // - by scanning for an IControlBinder implementation with an attribute referencing the control type.
        }


        /*
        public virtual IControlBinder GetBinderForControl(object control, IPresenter presenter)
        {
            var binderClass = BinderClassMappings.TryGetValue(control.GetType());
            //TODO: if not found this way, search mappings for closest superclass of control, or an interface implemented by it,
            //  and add mapping for control.GetType() (to speed up future searches).

            if(binderClass != null)
                return DiResolver.GetInstance(binderClass);
            else
                return null;
        }

        public virtual void Scan(Assembly assm)
        {
            foreach(var t in assm.GetTypes().Where(t => t.IsDefined(typeof(ControlBinderAttribute))))
            {

            }
        }

        /// <summary>
        /// Mappings of control types to binder class types.
        /// </summary>
        public virtual IDictionary<Type,Type> BinderClassMappings { get; set; }
        */
    }
}
