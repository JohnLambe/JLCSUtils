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
    /// </summary>
    /// <seealso cref="MvpBoundControlAttribute"/>
    public class AttributedControlBinder : IControlBinderExt
    {
        /// <summary>
        /// </summary>
        /// <param name="control">The control to bind.</param>
        /// <param name="attribute">The attribute on the control that caused this binder class to be used.</param>
        public AttributedControlBinder(object control, MvpBoundControlAttribute attribute)
        {
            BoundControl = control;            
//            var attribute = BoundControl.GetType().GetCustomAttribute<MvpBoundControlAttribute>();   // get the attribute that probably caused this class to be used
            UseOwnHandler = (attribute?.UseOwnHandler ?? false) && (BoundControl is IControlBinder);   // if the control implements the interface, and the attribute is present and indicates that this should be used.
            Attribute = attribute;
        }

        /// <summary>
        /// The control bound by this binder.
        /// </summary>
        public virtual object BoundControl { get; }

        /// <summary>
        /// The attribute on the bound control, that flags it is a control to be bound by this class.
        /// </summary>
        protected virtual MvpBoundControlAttribute Attribute { get; }

        /// <summary>
        /// True iff the control's own <see cref="IControlBinder"/> implementation is also used.
        /// </summary>
        protected virtual bool UseOwnHandler { get; }

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
/*
            if(Attribute?.IconIdProperty != null)
            {
                ReflectionUtil.TrySetPropertyValue(BoundControl, Attribute?.IconIdProperty, );
            }
*/

        }

        public virtual void MvpRefresh()
        {
            if (UseOwnHandler)
                ((IControlBinder)BoundControl).MvpRefresh();
        }


        //protected virtual string GetModelPropertyName()
        //{
        //    /*
        //    // Look for an attributed property:
        //    var properties = BoundControl.GetType().GetProperties().Where(p => p.IsDefined<MvpModelPropertyAttribute>());
        //    if (properties.Count() > 1)
        //    {
        //        throw new MvpBindingException("Can't bind " + BoundControl.GetType().Name + " because it has " + properties.Count()
        //            + " properties with " + typeof(MvpModelPropertyAttribute).Name);
        //    }

        //    if (properties.Any())
        //    {
        //        return properties.First().GetValue(BoundControl).ToString();
        //    }
        //    */

        //    return null;
        //}
    }
}
