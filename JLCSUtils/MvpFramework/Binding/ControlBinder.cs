using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using JohnLambe.Util;
using DiExtension.AutoFactory;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Interface implemented by a control or by a helper for the control,
    /// to bind the model to it.
    /// </summary>
    public interface IControlBinder
    {
        /// <summary>
        /// Bind the given model to the control.
        /// </summary>
        /// <param name="modelBinder"></param>
        void BindModel(ModelBinderWrapper modelBinder);
    }


    public interface IControlBinderFactory : IFactory<IControlBinder, Control, IPresenter>
    {
    }


    /// <summary>
    /// 
    /// </summary>
    public class ControlBinderFactory : IControlBinderFactory
    {
        public virtual IControlBinder Create(Control control, IPresenter presenter)
        {
            if (control is IControlBinder)                   // if the control implements the interface itself
                return control as IControlBinder;            // use it
            else
                return new GeneralControlBinder(control, presenter);
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
            // - by a naming convention.
            // - by type mapping in DI container (get implementation of IControlBinder<T> where T is the control type).
            // - scanning for an IControlBinder implementation with an attribute referencing the control type.
        }
    }

}
