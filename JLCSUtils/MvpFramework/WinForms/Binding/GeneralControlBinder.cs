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
using System.Drawing;

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
    public class GeneralControlBinder : IControlBinderExt, IControlBinderV2, IValidateableControl, IKeyboardKeyHandler
    {
        /// <summary>
        /// Prefix of string in the Tag property that specifies how the control is bound.
        /// </summary>
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
            _backColor = control.BackColor;

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

        public virtual void MvpBind(MvpControlBindingContext context)
        {
            Presenter = (context.PresenterBinder as PresenterBinderWrapper).Presenter;

            try
            {
                if (_controlProperty != null)
                {
                    Model = context.ModelBinder;

                    MvpRefresh();

                    if (PropertyBinder?.CanWrite ?? false)
                    {
                        _boundControl.Validating += BoundControl_Validating;
                        _boundControl.Validated += BoundControl_Validated;
                        //                        _boundControl.TextChanged += BoundControl_ValueChanged;
                        //                        modelBinder.BindProperty(_propertyName);
                    }

                    if (_boundControl is TextBoxBase)
                    {
                        var property = PropertyBinder?.Property;
                        if (property != null)
                        {
                            var attrib = property.GetCustomAttribute<MaxLengthAttribute>();
                            if (attrib != null)
                            {
                                ((TextBoxBase)_boundControl).MaxLength = attrib.Length;
                            }
                        }

                        //TODO: Attach a handler for key press events (for TextBoxBase and possibly some other controls) that undoes any change on pressing ESCAPE.

                    }
                }

                // 'Click' event handler:
                var handlerInfo = context.PresenterBinder.GetHandlerInfo(_modelPropertyName, null);
                //var handler = presenterBinder.GetHandler(_modelPropertyName);
                if (handlerInfo != null)
                {
                    //                    _eventHandlerMethod = handler.Method;
                    _eventHandlerDelegate = handlerInfo.HandlerDelegate;
                    _boundControl.Click += BoundControl_Click;  //TODO: Could add `handler` directly to Click. 
                    _hotKeys = handlerInfo.Attribute?.HotKeys;
                }

                //TODO: Other events. Map to handler name: <Name>_<Event>
            }
            catch (Exception ex)
            {
                string controlName = (BoundControl is Control) ? ((Control)BoundControl).Name : BoundControl.ToString();
                throw new MvpBindingException("Error on binding control " + controlName + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Bind this control to the given model.
        /// </summary>
        /// <param name="modelBinder"></param>
        /// <param name="presenterBinder"></param>
        public virtual void MvpBind(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
        {
            MvpBind(new MvpControlBindingContext(modelBinder, presenterBinder));
        }

        protected virtual void BoundControl_Validated(object sender, EventArgs e)
        {
            if (EventEnabled)
            {
                PropertyBinder?.Validated(sender, e, _controlProperty.GetValue(_boundControl));
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
                        if ((PropertyBinder?.CanRead ?? false) && _controlProperty.CanWrite)
                            _controlProperty.SetValue(_boundControl, PropertyBinder.Value);
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
            Invoke();
        }

        /// <summary>
        /// Invoke the behaviour (handler) of the bound control (e.g. for clicking a button or equivalent).
        /// </summary>
        protected virtual void Invoke()
        {
            _eventHandlerDelegate?.Invoke(this, EventArgs.Empty);     //TODO populate any arguments of event handler
            //_eventHandlerMethod?.Invoke(Presenter, new object[] { this, EventArgs.Empty });     //TODO populate any arguments of event handler
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
                PropertyBinder.Value = _controlProperty.GetValue(_boundControl);
            }
        }

        public void NotifyKeyDown(KeyboardKeyEventArgs args)
        {
            // Invoke certain controls, e.g. buttons, if the key matches their hotkey.
            if (!args.Cancel && (_hotKeys?.Contains(args.Key) ?? false))  // if not already cancelled and the key matches any of the keys for this item
            {
                Invoke();
                args.Cancel = true;
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
//        protected MethodInfo _eventHandlerMethod;
        protected EventHandler _eventHandlerDelegate;

        /// <summary>
        /// Hotkeys to invoke the bound handler.
        /// </summary>
        protected KeyboardKey[] _hotKeys;

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

        /// <summary>
        /// The binder of the bound property.
        /// </summary>
        protected ModelPropertyBinder PropertyBinder => Model?.GetProperty(_modelPropertyName);

        public virtual bool ReadOnly
        {
            get
            {
                return ReflectionUtil.TryGetPropertyValue<bool>(BoundControl, "ReadOnly");
            }
            set
            {
                ReflectionUtil.TrySetPropertyValue(BoundControl, "ReadOnly", value);
            }
        }

        public virtual bool Validate(ControlValidationOptions options)
        {
            var isValid = IsValid();
            if (HasValue())
            {
                _boundControl.BackColor = !isValid ? Color.Yellow : _backColor;

                if (!isValid)
                {
                    if (options.Flags.HasFlag(ControlValidationFlags.Enter))
                    {
                        _boundControl.Focus();
                    }
                }
            }
            return isValid;
        }

        protected Color _backColor;

        /// <summary>
        /// true iff the bound control has a data value that can be bound.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasValue()
        {
            return _controlProperty != null;
        }

        /// <summary>
        /// true iff the valid in this control is valid. If it is not bound or does not do any validation, it is considered valid.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsValid()
        {
            var value = _controlProperty?.GetValue(_boundControl) ?? true;

            try
            {
                PropertyBinder?.ValidateValue(ref value);
            }
            catch (ValidationException)
            {
                return false;
            }
            return true;
        }

        public virtual int ValidationOrder => PropertyBinder?.Order ?? 0;

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
