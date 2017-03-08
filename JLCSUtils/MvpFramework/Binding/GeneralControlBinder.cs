using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;

using JohnLambe.Util;
using JohnLambe.Util.Reflection;

namespace MvpFramework.Binding
{
    // Quick-and-dirty binder to demonstrate the concept:

    /// <summary>
    /// Binds some common WinForms controls.
    /// </summary>
    public class GeneralControlBinder : IControlBinderExt
    {
        public GeneralControlBinder(Control control, IPresenter presenter)
        {
            BoundControl = control;
            this.Presenter = presenter;

            string tag = BoundControl.Tag?.ToString();
            if (tag != null)
            {
                _modelPropertyName = tag.ExtractEnclosed('[', ']');
                //ControlToBind.GetType().GetProperty(_propertyName);
                _controlProperty = BoundControl.GetType().GetProperty("Text");  //TODO 
            }

        }

        /// <summary>
        /// Bind this control to the given model.
        /// </summary>
        /// <param name="modelBinder"></param>
        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            if (_controlProperty != null)
            {
                Model = modelBinder;

                Refresh();

                if (modelBinder.GetProp(_modelPropertyName).CanWrite)
                    BoundControl.TextChanged += BoundControl_ValueChanged;
                //modelBinder.BindProperty(_propertyName);

                if (BoundControl is TextBox)
                {
                    var property = modelBinder.GetProp(_modelPropertyName).Property;
                    var attrib = property.GetCustomAttribute<MaxLengthAttribute>();
                    if(attrib != null)
                        ((TextBox)BoundControl).MaxLength = attrib.Length;
                }
            }

            // 'Click' event handler:
            var method = Presenter?.GetType().GetMethods().Where(
                p => p.GetCustomAttributes<MvpHandlerAttribute>().Where(a => a.Id?.Equals(_modelPropertyName) ?? false).Any())
                ?.FirstOrDefault();
            if (method != null)
            {
                _eventHandlerMethod = method;
                BoundControl.Click += BoundControl_Click;
            }
        }

        /// <summary>
        /// Update the control from the model.
        /// </summary>
        public virtual void Refresh()
        {
            if (_modelPropertyName != null)
            {
                EventEnabled = false;
                try
                {
                    if (Model.GetProp(_modelPropertyName).CanRead)
                        _controlProperty.SetValue(BoundControl, Model.GetProp(_modelPropertyName).Value);
                }
                finally
                {
                    EventEnabled = true;
                }
            }
        }

        protected virtual void BoundControl_Click(object sender, EventArgs e)
        {
            _eventHandlerMethod?.Invoke(Presenter, new object[] { });     //TODO populate any arguments of event handler
        }

        /// <summary>
        /// Update the model when a value in the control changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void BoundControl_ValueChanged(object sender, EventArgs e)
        {
            if(EventEnabled)
                Model.GetProp(_modelPropertyName).Value = _controlProperty.GetValue(BoundControl);
        }

        /// <summary>
        /// Iff false, the event to update the model is not fired.
        /// </summary>
        protected virtual bool EventEnabled { get; set; } = true;

        /// <summary>
        /// The control bound by this <see cref="IControlBinder"/>.
        /// </summary>
        public virtual Control Control => BoundControl;

        protected MethodInfo _eventHandlerMethod;

        protected readonly IPresenter Presenter;

        protected ModelBinderWrapper Model;
        /// <summary>
        /// The control bound by this class.
        /// </summary>
        protected readonly Control BoundControl;
        /// <summary>
        /// Name of the bound property on the model.
        /// </summary>
        protected readonly string _modelPropertyName;
        /// <summary>
        /// The bound property on the control.
        /// </summary>
        protected readonly PropertyInfo _controlProperty;
    }
}
