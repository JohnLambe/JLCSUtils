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
using System.ComponentModel;
using JohnLambe.Util.Validation;

namespace MvpFramework.Binding
{
    // Quick-and-dirty binder to demonstrate the concept:

    //TODO: Bind the property specified by DefaultBindingPropertyAttribute.
    //  And event specified by DefaultEventAttribute (the one created on double-clicking in the designer).
    // Note: CheckBox has DefaultBindingPropertyAttribute:CheckState but DefaultEventAttribute:CheckChanged.

    /// <summary>
    /// Binds some common WinForms controls.
    /// </summary>
    //| Could be called "TagControlBinder".
    public class GeneralControlBinder : IControlBinderExt
    {
        /// <summary>
        /// Creates a <see cref="GeneralControlBinder"/> for the given control if it can be bound,
        /// otherwise, returns null.
        /// </summary>
        /// <param name="control"></param>
        /// <returns>Binder or null.</returns>
        public static IControlBinderExt TryCreateBinder(Control control)
        {
            if (GetBinderString(control) != null)
                return new GeneralControlBinder(control);
            else
                return null;        // no binder
        }

        public GeneralControlBinder(Control control)
        {
            var defaultBindingAttrib = control.GetType().GetCustomAttribute<DefaultBindingPropertyAttribute>();
            var propertyName = defaultBindingAttrib?.Name;

            _boundControl = control;
            //            this.Presenter = presenter;

            _modelPropertyName = GetBinderString();

            if (_modelPropertyName != null && propertyName != null)
            {
                _controlProperty = _boundControl.GetType().GetProperty(propertyName);
            }
        }

        protected static string GetBinderString(Control control)
        {
            /* Use AttributedControlBinder for this:
            // Look for an attributed property:
            var properties = _boundControl.GetType().GetProperties().Where(p => p.IsDefined<MvpModelPropertyAttribute>());
            if (properties.Count() > 1)
            {
                throw new MvpBindingException("Can't bind " + _boundControl.GetType().Name + " because it has " + properties.Count()
                    + " properties with " + typeof(MvpModelPropertyAttribute).Name);
            }

            if(properties.Any())
            {
                return properties.First().GetValue(_boundControl).ToString();
            }
            */

            string tag = control.Tag?.ToString();
            if (tag != null)
            {
                return tag.ExtractEnclosed('[', ']');
                //ControlToBind.GetType().GetProperty(_propertyName);
            }
            return null;
        }

        protected virtual string GetBinderString()
        {
            return GetBinderString(_boundControl);
        }

        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            BindModel(modelBinder, new PresenterBinderWrapper(presenter));
        }

        /// <summary>
        /// Bind this control to the given model.
        /// </summary>
        /// <param name="modelBinder"></param>
        public virtual void BindModel(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
        {
            Presenter = presenterBinder.Presenter;

            try
            {

                if (_controlProperty != null)
                {
                    Model = modelBinder;

                    Refresh();

                    if (modelBinder.GetProp(_modelPropertyName).CanWrite)
                    {
                        _boundControl.Validating += _boundControl_Validating;
                        _boundControl.Validated += _boundControl_Validated; ;
                        //                        _boundControl.TextChanged += BoundControl_ValueChanged;
                        //                        modelBinder.BindProperty(_propertyName);
                    }

                    if (_boundControl is TextBoxBase)
                    {
                        var property = modelBinder.GetProp(_modelPropertyName).Property;
                        var attrib = property.GetCustomAttribute<MaxLengthAttribute>();
                        if (attrib != null)
                        {
                            ((TextBoxBase)_boundControl).MaxLength = attrib.Length;
                        }
                    }
                }

                // 'Click' event handler:
                var method = //presenterBinder.GetHandler(_modelPropertyName)
                    Presenter?.GetType().GetMethods().Where(
                    p => p.GetCustomAttributes<MvpHandlerAttribute>().Where(a => a.Id?.Equals(_modelPropertyName) ?? false).Any())
                    ?.FirstOrDefault();
                if (method != null)
                {
                    _eventHandlerMethod = method;
                    _boundControl.Click += BoundControl_Click;
                }

                //TODO: Other events. Map to handler name: <Name>_<Event>
            }
            catch(Exception ex)
            {
                throw new MvpBindingException("Error on binding control " + BoundControl + ": " + ex.Message, ex);
                //TODO: Get control name?
            }
        }

        private void _boundControl_Validated(object sender, EventArgs e)
        {
            if (EventEnabled)
            {
                Model.GetProp(_modelPropertyName).Value = _controlProperty.GetValue(_boundControl);
                //| We could set _boundControl.'Modified' (if it exists) to false.         
            }
        }

        protected void _boundControl_Validating(object sender, CancelEventArgs e)
        {
            if (EventEnabled)
            {
                var value = _controlProperty.GetValue(_boundControl);
                Model.GetProp(_modelPropertyName).ValidateValue(ref value);
//                _controlProperty.SetValue(_boundControl, value);
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
                    if(Model != null)
                        if (Model.GetProp(_modelPropertyName).CanRead && _controlProperty.CanWrite)
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
            if (EventEnabled)
            {
                Model.GetProp(_modelPropertyName).Value = _controlProperty.GetValue(_boundControl);
            }
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
        protected readonly string _modelPropertyName;  //TODO: Replace with ModelPropertyBinder (for efficiency)
        /// <summary>
        /// The bound property on the control - the property of the control whose value is bound to the model.
        /// </summary>
        protected readonly PropertyInfo _controlProperty;
    }
}
