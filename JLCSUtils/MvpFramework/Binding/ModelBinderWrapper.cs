using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;
using JohnLambe.Util.Reflection;
using System.ComponentModel.DataAnnotations;
using JohnLambe.Util.Validation;
using MvpFramework.Dialog.Dialogs;
using System.ComponentModel;
using MvpFramework.Dialog;
using JohnLambe.Util.Types;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Wrapper for a model, used by control binders (<see cref="IControlBinderExt"/>).
    /// </summary>
    //| A model could be a map, XmlNode, IConfigValueProvider, etc., and this class or a subclass (maybe refactored as an interface,
    //| with a factory that gets an implementation for a given model) could return properties in a standard way.
    //| Currently, it supports only accessing properties of an object.
    public class ModelBinderWrapper
    {
        public ModelBinderWrapper(object model)
        {
            this.Model = model;
        }

        #region obsolete
        // Methods obsoleted. Use GetProperty instead:

        [Obsolete]
        public virtual object GetValue(string propertyName)
        {
            //            return GetProperty(propertyName)?.GetValue(Model);
            return ReflectionUtil.TryGetPropertyValue<object>(Model, propertyName);
        }

        [Obsolete]
        public virtual void SetValue(string propertyName, object value)
            //            => GetProperty(propertyName)?.SetValue(Model, value);
            => ReflectionUtil.TrySetPropertyValue<object>(Model, propertyName, value);

        [Obsolete]
        public virtual bool CanRead(string propertyName)
            => GetPropertyInternal(propertyName)?.CanRead ?? false;

        [Obsolete]
        public virtual bool CanWrite(string propertyName)
            => GetPropertyInternal(propertyName)?.CanWrite ?? false;

        /// <summary>
        /// Get a property of the model.
        /// Returns null if this model does not support it.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [Obsolete]
        private PropertyInfo GetPropertyInternal(string propertyName)
        {
            var model = Model; 
            return ReflectionUtil.GetProperty(ref model, propertyName);
        }

        [Obsolete]
        public virtual string GetCaptionForProperty(string propertyName)
        {
            return CaptionUtil.GetDisplayName(GetPropertyInternal(propertyName));
        }

        /*
        [Obsolete("Use GetProperty")]
        public ModelPropertyBinder GetProp(string propertName)
        {
            return GetProperty(propertName);
        }
        */
        /*
               /// <summary>
               /// Provides attributes of the property, which may include details
               /// related to binding or validation.
               /// This may (in subclasses or future versions) return attributes even if 
               /// <see cref="GetProperty(string)"/> returns null, so it is recommended to use this instead of using it
               /// as <see cref="ICustomAttributeProvider"/>.
               /// </summary>
               /// <param name="propertyName"></param>
               /// <returns></returns>
               public virtual ICustomAttributeProvider GetAttributes(string propertyName)
               {
                   return GetProperty(propertyName);
               }
       */
        #endregion


        /// <summary>
        /// The collection of groups (of properties), sorted.
        /// </summary>
        public virtual IEnumerable<IUiGroupModel> Groups
            => Model.GetType().GetCustomAttributes<GroupDefinitionAttribute>(true)
                .OrderBy(a => a.Order);

        /// <summary>
        /// Returns all properties in the specifed group.
        /// </summary>
        /// <param name="groupId">The group to get.
        /// null for all groups.
        /// "" for all properties with no Group.
        /// </param>
        /// <returns></returns>
        public virtual IEnumerable<ModelPropertyBinder> GetPropertiesByGroup(string groupId)
            => Model.GetType().GetProperties()
                .Select(p => new ModelPropertyBinder(Model, p))
                .Where(p => groupId == null || (p.Group ?? "") == groupId)
                .OrderBy(p => p.Order);     // same order as Properties
        //TODO: Could exclude properties based on a naming convention, e.g. ending with "Id",
        //  or exclude properties referenced by an Entity Framework ForeignKeyAttribute
        //  (unless overridden in an attribute).

        /// <summary>
        /// All bindable properties of the model.
        /// (All objects that could be returned from <see cref="GetProperty"/>.)
        /// </summary>
        public virtual IEnumerable<ModelPropertyBinder> Properties
            => Model.GetType().GetProperties()
                    .Select(p => new ModelPropertyBinder(Model, p))
                    .OrderBy(p => p.Order);

        /// <summary>
        /// Get an object representing the binding of a requested property, and allowing reading and writing the property value.
        /// </summary>
        /// <param name="propertName">The name of the property in the model.
        /// (This is usually case-sensitive, but that depends on the type of model (the subclass of this)).</param>
        /// <returns></returns>
        public virtual ModelPropertyBinder GetProperty(string propertName)
        {
            return new ModelPropertyBinder(Model, propertName);
        }

        //TODO: Lazily populate property binders (Map) ?
        //  OR populate all property binders initially, but individual binder objects lazily populate their contents.

        #region Validation

        /// <summary>
        /// Validate the model.
        /// </summary>
        /// <param name="dialogService"></param>
        /// <returns>true iff valid.</returns>
        /// <remarks>
        /// This is for validating the whole model as an object.
        /// See <see cref="ModelPropertyBinder.Validating(object, CancelEventArgs, ref object, IMessageDialogService)"/> (and similar) for validating individual properties.
        /// </remarks>
        public virtual bool Validate([Nullable] IMessageDialogService dialogService)
        {
            ValidationResults results;
            Validate(Model, dialogService, out results);
            return results.IsValid;
        }

        /// <summary>
        /// Validate an object in the model.
        /// </summary>
        /// <param name="instance">The object to be validated. The model or an object within it. NOT a primitive value.</param>
        /// <param name="dialogService"></param>
        /// <param name="results"></param>
        public virtual void Validate([TypeValidation(IsPrimitive = false)] object instance, [Nullable] IMessageDialogService dialogService, out ValidationResults results)
        {
            results = new ValidationResults();

            Validator.TryValidateObject(instance, results);

            if (!results.IsValid)
            {
                if (dialogService != null)
                    dialogService.ShowMessage(UserErrorDialog.CreateDialogModelForValidationResult(results));
                else
                    results.ThrowIfInvalid();
            }

            //TODO: Warnings

            /*
            if (results.Modified)
            {
                value = results.NewValue;
                ////                _controlProperty.SetValue(_boundControl, results.NewValue);
                //                    ReflectionUtil.TrySetPropertyValue(_boundControl, "Modified", true);  // leave it 'modified' until the property is assigned to the model
            }

            return results.Modified;
            */
        }

        /// <summary>
        /// Validator used to validate the model.
        /// </summary>
        protected virtual ValidatorEx Validator { get; set; } = new ValidatorEx(); //TODO: Make injectable.  OR lazy populate.

        #endregion

        /// <summary>
        /// Returns the model as an object, if supported.
        /// Returns null if this type of model does not support it.
        /// </summary>
        public virtual object AsObject => Model;

        /// <summary>
        /// The underlying Model object.
        /// </summary>
        protected readonly object Model;
    }


    /// <summary>
    /// Metadata of a property on an instance.
    /// Provides access to the property value.
    /// </summary>
    public class ModelPropertyBinder : BoundProperty<object, object>
    {
        public ModelPropertyBinder(object target, PropertyInfo property) : base(target, property)
        {
            Init();
        }

        public ModelPropertyBinder(object target, string propertyName) : base(target, propertyName)
        {
            Init();
        }

        protected void Init()
        {
            _displayAttribute = Property?.GetCustomAttribute<DisplayAttribute>();
            _mvpDisplayAttribute = Property?.GetCustomAttribute<MvpDisplayAttribute>();
        }

        /// <summary>
        /// The data type of the property.
        /// May be null if the provider of the data does not support it (it may not be a real object property).
        /// </summary>
        public virtual Type PropertyType
            => Property?.PropertyType;

        /// <summary>
        /// Sorting order weight.
        /// </summary>
        public virtual int Order
            => _displayAttribute?.GetOrder() ?? 0;

        /// <summary>
        /// true if this property is visible in user interfaces.
        /// </summary>
        public virtual bool IsVisible
            => _mvpDisplayAttribute?.IsVisible ?? true;

        /// <summary>
        /// True if a control should be generated for this property (when using UI generation).
        /// </summary>
        public virtual bool AutoGenerate
            => _displayAttribute?.GetAutoGenerateField() ??
            _mvpDisplayAttribute?.IsVisible ??
            true;

        /// <summary>
        /// Maximum string length in characters.
        /// <see cref="StringValidationAttribute.Na"/> if unknown or unlimitied.
        /// This can be used to configure the maximum length of a user interface control.
        /// <para>
        /// This uses the first of <see cref="StringValidationAttribute"/>, <see cref="MaxLengthAttribute"/> and <see cref="StringLengthAttribute"/>
        /// that provides a value.
        /// </para>
        /// </summary>
        /// <remarks>
        /// To set a maximum length for Entity Framework (and probably other ORM frameworks), use <see cref="MaxLengthAttribute"/>, and to have the same length applied in user interfaces,
        /// don't specify one in <see cref="StringValidationAttribute"/>.
        /// To use a different length in user interfaces to Entity Framework, use <see cref="MaxLengthAttribute"/> for Entity Framework, and <see cref="StringValidationAttribute"/> for user interfaces.
        /// </remarks>
        public virtual int MaxLength
        {
            get
            {
                int length = Property.GetCustomAttribute<StringValidationAttribute>()?.MaximumLength ?? StringValidationAttribute.Na;
                if(length == StringValidationAttribute.Na)
                    length = Property.GetCustomAttribute<MaxLengthAttribute>()?.Length
                        ?? Property.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? StringValidationAttribute.Na;
                return length;
            }
        }

        /// <summary>
        /// true iff the property is allowed to be null (when validating).
        /// This is always false (for bound properties) if the type does not support null.
        /// </summary>
        public virtual bool Nullable
            => TypeUtil.IsNullable(Property.PropertyType)           // if the type supports null
                    && (!IsDefined(typeof(RequiredAttribute), true)             // and it is not flagged as not-nullable by one of these attributes
                    || (this.GetCustomAttribute<NullabilityAttribute>()?.IsNullable ?? false));

        /// <summary>
        /// The user interface group (<see cref="DisplayAttribute.GroupName"/>) of this item.
        /// </summary>
        public virtual string Group
            => _displayAttribute?.GetGroupName() ?? "";

        #region Validation


        public virtual bool Validating([Nullable] object sender, CancelEventArgs evt, ref object value, [Nullable] IMessageDialogService dialogService = null)
        {
            if (Property == null)
                return false;
            ValidationResults results;
            return Validating(sender,evt,ref value,dialogService, out results);
        }

        //TODO: Returning validation results, with no exception or dialog.
        /// <summary>
        /// Called when a value is modified in the UI, to validate the new value.
        /// In WinForms, this should be called on the <see cref="System.Windows.Forms.Control.Validating"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        /// <param name="value">The value in the UI, to be updated to the model.</param>
        /// <param name="dialogService">If not null, a dialog is shown if invalid.</param>
        /// <param name="exception">Iff true, and <paramref name="dialogService"/> is null, a <see cref="ValidationException"/> is thrown on failure.</param>
        /// <param name="results"></param>
        /// <returns>true iff <paramref name="value"/> is modified.</returns>
        public virtual bool Validating([Nullable] object sender, CancelEventArgs evt, ref object value,
            [Nullable] IMessageDialogService dialogService, out ValidationResults results, bool exception = false)
        {
            results = new ValidationResults();
            if (Property == null)   // if no bound property
                return false;

            TryValidateValue(value, results);
            if ( !results.IsValid )
            {
                evt.Cancel = true;   // validation fails. This usually means that the control stays focussed.

                // We show the dialog here, if we have the service to do so,
                // because raising an exception would cause WinForms not to handle the cancelling of the event
                // (it would allow the focus to leave the control).
                if (dialogService != null)
                    dialogService.ShowMessage(UserErrorDialog.CreateDialogModelForValidationResult(results));
                else if(exception)
                    results.ThrowIfInvalid();
            }

            //TODO: Warnings

            if (results.Modified)
            {
                value = results.NewValue;
//                    ReflectionUtil.TrySetPropertyValue(_boundControl, "Modified", true);  // leave it 'modified' until the property is assigned to the model
            }

            return results.Modified;
        }

        /// <summary>
        /// Called when a value is modified and validated in the user interface. In WinForms, this should be called on the <see cref="System.Windows.Forms.Control.Validated"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="value">The value in the UI, to be updated to the model.</param>
        public virtual void Validated(object sender, EventArgs e, object value)
        {
            if(CanWrite)
                ValueConverted = value;
            //| We could set _boundControl.'Modified' (if it exists) to false:
            //                ReflectionUtil.TrySetPropertyValue(_boundControl, "Modified", false);  // control value is the same as the model
        }

        #endregion

        //TODO: public virtual bool Modified { get; set; }
        //        ValueChanged event ?
        //        Revert


        //TODO: Lazily initialize:
        protected DisplayAttribute _displayAttribute;
        protected MvpDisplayAttribute _mvpDisplayAttribute;  
    }

    //TODO: raise EventHandler<ValueChangedEventArgs>

}
