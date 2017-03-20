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
        public GeneralControlBinder(Control control)
        {
            _boundControl = control;
//            this.Presenter = presenter;

            string tag = _boundControl.Tag?.ToString();
            if (tag != null)
            {
                _modelPropertyName = tag.ExtractEnclosed('[', ']');
                //ControlToBind.GetType().GetProperty(_propertyName);
                _controlProperty = _boundControl.GetType().GetProperty("Text");  //TODO 
            }

        }

        /// <summary>
        /// Bind this control to the given model.
        /// </summary>
        /// <param name="modelBinder"></param>
        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            Presenter = presenter;

            if (_controlProperty != null)
            {
                Model = modelBinder;

                Refresh();

                if (modelBinder.GetProp(_modelPropertyName).CanWrite)
                    _boundControl.TextChanged += BoundControl_ValueChanged;
                //modelBinder.BindProperty(_propertyName);

                if (_boundControl is TextBox)
                {
                    var property = modelBinder.GetProp(_modelPropertyName).Property;
                    var attrib = property.GetCustomAttribute<MaxLengthAttribute>();
                    if(attrib != null)
                        ((TextBox)_boundControl).MaxLength = attrib.Length;
                }
            }

            // 'Click' event handler:
            var method = Presenter?.GetType().GetMethods().Where(
                p => p.GetCustomAttributes<MvpHandlerAttribute>().Where(a => a.Id?.Equals(_modelPropertyName) ?? false).Any())
                ?.FirstOrDefault();
            if (method != null)
            {
                _eventHandlerMethod = method;
                _boundControl.Click += BoundControl_Click;
            }

            //TODO: Other events. Map to handler name: <Name>_<Event>
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
                        _controlProperty.SetValue(_boundControl, Model.GetProp(_modelPropertyName).Value);
                }
                finally
                {
                    EventEnabled = true;
                }
            }
        }

        /// <summary>
        /// Handles a Click event on the bound control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                Model.GetProp(_modelPropertyName).Value = _controlProperty.GetValue(_boundControl);
        }

        /// <summary>
        /// Iff false, the event to update the model is not fired.
        /// </summary>
        protected virtual bool EventEnabled { get; set; } = true;

        /// <summary>
        /// The control bound by this <see cref="IControlBinder"/>.
        /// </summary>
        public virtual object BoundControl => _boundControl;

        //TODO: Use PresenterBinder, and use an abstraction of the handler (rather than directly referencing a method).
        /// <summary>
        /// Method of the presenter to handler Click event.
        /// </summary>
        protected MethodInfo _eventHandlerMethod;

        /// <summary>
        /// The bound Presenter.
        /// </summary>
        protected virtual IPresenter Presenter { get; private set; }

        /// <summary>
        /// The model of the View that the control is placed in.
        /// </summary>
        protected ModelBinderWrapper Model;
        /// <summary>
        /// The control bound by this <see cref="IControlBinder"/>.
        /// </summary>
        protected readonly Control _boundControl;
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
