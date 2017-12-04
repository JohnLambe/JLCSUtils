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
using MvpFramework.Dialog;
using MvpFramework.Dialog.Dialogs;
using DiExtension.AutoFactory;
using JohnLambe.Util.Misc;
using DiExtension.Attributes;

namespace MvpFramework.Binding
{
    // Quick-and-dirty binder to demonstrate the concept:

    //TODO: NOTE: This references WinForms - move to MvpFramework.Binding.WinForms.

    //TODO: Bind the property specified by DefaultBindingPropertyAttribute.
    //  And event specified by DefaultEventAttribute (the one created on double-clicking in the designer).
    // Note: CheckBox has DefaultBindingPropertyAttribute:CheckState but DefaultEventAttribute:CheckChanged.

    /// <summary>
    /// Binds some common WinForms controls.
    /// </summary>
    //| Could be called "TagControlBinder", "GeneralWinFormsControlBinder", or "WinFormsTagControlBinder".
    public class GeneralControlBinder : IControlBinderExt, IKeyboardKeyHandler
    {
        public const char TagPrefix = '[';
        public const char TagSuffix = ']';

        /// <summary>
        /// Creates a <see cref="GeneralControlBinder"/> for the given control if it can be bound,
        /// otherwise, returns null.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="messageDialogServiceFactory">Providers the message dialog service to be used by the binder.
        /// See the corresponding parameter of <see cref="GeneralControlBinder.GeneralControlBinder(Control, Func{IMessageDialogService})"/>.
        /// </param>
        /// <returns>Binder or null.</returns>
        public static IControlBinderExt TryCreateBinder(Control control, Func<IMessageDialogService> messageDialogServiceFactory)
        {
            if (GetBinderString(control) != null)
                return new GeneralControlBinder(control, messageDialogServiceFactory);
            else
                return null;        // no binder
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control">The control to be bound.</param>
        /// <param name="messageDialogServiceFactory">Delegate to create a message dialog service to be used for showing validation errors.
        /// If null, validation errors are raised as exceptions.</param>
        public GeneralControlBinder(Control control, Func<IMessageDialogService> messageDialogServiceFactory)
        {
            this.DialogServiceFactory = messageDialogServiceFactory;

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

        /// <summary>
        /// Returns the binder string for the given control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
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
                return tag.ExtractEnclosed(TagPrefix, TagSuffix);
                //ControlToBind.GetType().GetProperty(_propertyName);
            }
            return null;
        }

        /// <summary>
        /// Gets the binder string from the bound control.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetBinderString()
        {
            return GetBinderString(_boundControl);
        }

        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            MvpBind(modelBinder, new PresenterBinderWrapper(presenter));
        }

        /// <summary>
        /// Bind this control to the given model.
        /// </summary>
        /// <param name="modelBinder"></param>
        /// <param name="presenterBinder"></param>
        public virtual void MvpBind(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
        {
            Presenter = presenterBinder.Presenter;

            try
            {

                if (_controlProperty != null)
                {
                    Model = modelBinder;

                    MvpRefresh();

                    if (modelBinder.GetProperty(_modelPropertyName).CanWrite)
                    {
                        _boundControl.Validating += BoundControl_Validating;
                        _boundControl.Validated += BoundControl_Validated;
                        //                        _boundControl.TextChanged += BoundControl_ValueChanged;
                        //                        modelBinder.BindProperty(_propertyName);
                    }

                    if (_boundControl is TextBoxBase)
                    {
                        var property = modelBinder.GetProperty(_modelPropertyName).Property;
                        var attrib = property.GetCustomAttribute<MaxLengthAttribute>();
                        if (attrib != null)
                        {
                            ((TextBoxBase)_boundControl).MaxLength = attrib.Length;
                        }

                        //TODO: Attach a handler for key press events (for TextBoxBase and possibly some other controls) that undoes any change on pressing ESCAPE.

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
                string controlName = (BoundControl is Control) ? ((Control)BoundControl).Name : BoundControl.ToString();
                throw new MvpBindingException("Error on binding control " + controlName + ": " + ex.Message, ex);
            }
        }

        protected virtual void BoundControl_Validated(object sender, EventArgs e)
        {
            if (EventEnabled)
            {
                PropertyBinder.Validated(sender, e, _controlProperty.GetValue(_boundControl));
                /*
                Model.GetProperty(_modelPropertyName).Value = _controlProperty.GetValue(_boundControl);
                //| We could set _boundControl.'Modified' (if it exists) to false:
//                ReflectionUtil.TrySetPropertyValue(_boundControl, "Modified", false);  // control value is the same as the model
                */
            }
        }

        protected virtual void BoundControl_Validating(object sender, CancelEventArgs e)
        {
            if (EventEnabled)
            {
                object value = _controlProperty.GetValue(_boundControl);
                if(PropertyBinder.Validating(sender, e, ref value, DialogService))
                {
                    _controlProperty.SetValue(_boundControl, value);
                }
                /*
                var value = _controlProperty.GetValue(_boundControl);
                var results = new ValidationResults();
                Model.GetProperty(_modelPropertyName).TryValidateValue(value, results);
                if ( !results.IsValid )
                {
                    e.Cancel = true;   // validation fails. This usually means that the control stays focussed.

                    // We show the dialog here, if we have the service to do so,
                    // because raising an exception would cause WinForms not to handle the cancelling of the event
                    // (it would allow the focus to leave the control).
                    if (DialogService != null)
                        DialogService.ShowMessage(UserErrorDialog.CreateDialogModelForValidationResult(results));
                    else
                        results.ThrowIfInvalid();
                }

                //TODO: Warnings

                if (results.Modified)
                {
                    _controlProperty.SetValue(_boundControl, results.NewValue);
//                    ReflectionUtil.TrySetPropertyValue(_boundControl, "Modified", true);  // leave it 'modified' until the property is assigned to the model
                }
                */
            }
        }

        /// <summary>
        /// Update the control from the model.
        /// </summary>
        public virtual void MvpRefresh()
        {
            if (_modelPropertyName != null)
            {
                EventEnabled = false;
                try
                {
                    if(Model != null)
                        if (Model.GetProperty(_modelPropertyName).CanRead && _controlProperty.CanWrite)
                            _controlProperty.SetValue(_boundControl, Model.GetProperty(_modelPropertyName).Value);
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
                Model.GetProperty(_modelPropertyName).Value = _controlProperty.GetValue(_boundControl);
            }
        }

        public void NotifyKeyDown(KeyboardKeyEventArgs args)
        {
            //TODO: Invoke certain controls, e.g. buttons, if the key matches their hotkey.
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
        /// Service for showing a dialog on validation errors.
        /// </summary>
        protected virtual IMessageDialogService DialogService => LazyInitialize.GetValue(ref _dialogService, DialogServiceFactory);
        [Inject]
        private Func<IMessageDialogService> DialogServiceFactory { get; set; }
        private IMessageDialogService _dialogService;

        /// <summary>
        /// The model of the View that the control is placed in.
        /// </summary>
        protected ModelBinderWrapper Model;

        protected ModelPropertyBinder PropertyBinder => Model?.GetProperty(_modelPropertyName);

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
