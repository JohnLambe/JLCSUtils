using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;

namespace MvpFramework.Binding
{
    //TODO:

    /// <summary>
    /// Binds a control based on attributes on it.
    /// <para><see cref="ControlBinderFactory"/> uses this when the control has the attribute <see cref="MvpBoundControlAttribute"/>.
    /// This can also be used as a base class for custom control binders, or explicitly provided as the binder for a control.
    /// </para>
    /// <para>This has no dependencies on any UI framework.</para>
    /// <seealso cref="MvpBoundControlAttribute"/>
    /// </summary>
    public class AttributedControlBinder : IControlBinderExt
    {
        /// <summary>
        /// </summary>
        /// <param name="control">The control to bind.</param>
        public AttributedControlBinder(object control)
        {
            BoundControl = control;
            var attribute = BoundControl.GetType().GetCustomAttribute<MvpBoundControlAttribute>();   // get the attribute that probably caused this class to be used
            UseOwnHandler = attribute?.UseOwnHandler ?? false && (BoundControl is IControlBinder);   // if the control implements the interface, and the attribute is present and indicates that this should be used.
        }

        /// <summary>
        /// The control bound by this binder.
        /// </summary>
        public virtual object BoundControl { get; protected set; }

        /// <summary>
        /// True iff the control's own <see cref="IControlBinder"/> implementation is also used.
        /// </summary>
        protected virtual bool UseOwnHandler { get; set; }

        public void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            BindModel(modelBinder, new PresenterBinderWrapper(presenter));
            if (UseOwnHandler)
                ((IControlBinder)BoundControl).BindModel(modelBinder, presenter);
        }

        public virtual void BindModel(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenter)
        {
//            BoundControl.GetType().GetProperties().Where(p => p.IsDefined<MvpMappingPropertyBaseAttribute>())
//TODO

        }

        public virtual void Refresh()
        {
            if (UseOwnHandler)
                ((IControlBinder)BoundControl).Refresh();
        }


        protected virtual string GetModelPropertyName()
        {
            /*
            // Look for an attributed property:
            var properties = BoundControl.GetType().GetProperties().Where(p => p.IsDefined<MvpModelPropertyAttribute>());
            if (properties.Count() > 1)
            {
                throw new MvpBindingException("Can't bind " + BoundControl.GetType().Name + " because it has " + properties.Count()
                    + " properties with " + typeof(MvpModelPropertyAttribute).Name);
            }

            if (properties.Any())
            {
                return properties.First().GetValue(BoundControl).ToString();
            }
            */

            return null;
        }
    }
}
