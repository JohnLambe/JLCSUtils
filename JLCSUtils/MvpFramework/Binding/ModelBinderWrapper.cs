using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;
using JohnLambe.Util.Reflection;
using System.ComponentModel.DataAnnotations;

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

        [Obsolete("Use GetProperty")]
        public ModelPropertyBinder GetProp(string propertName)
        {
            return GetProperty(propertName);
        }

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
        /// (This is usually case-sensitive, but that depends on the type model (the subclass of this)).</param>
        /// <returns></returns>
        public virtual ModelPropertyBinder GetProperty(string propertName)
        {
            return new ModelPropertyBinder(Model, propertName);
        }

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
        {
            get { return Property?.PropertyType; }
        }

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

        public virtual string Group
            => _displayAttribute?.GetGroupName() ?? "";

        //TODO: Lazily initialise:
        protected DisplayAttribute _displayAttribute;
        protected MvpDisplayAttribute _mvpDisplayAttribute;  
    }

}
