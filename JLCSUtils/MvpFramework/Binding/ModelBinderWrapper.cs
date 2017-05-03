using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;
using JohnLambe.Util.Reflection;

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
        /// All bindable properties of the model.
        /// (All objects that could be returned from <see cref="GetProperty"/>.)
        /// </summary>
        public virtual IEnumerable<ModelPropertyBinder> Properties
        {
            get
            {
                return Model.GetType().GetProperties()
                    .Select(p => new ModelPropertyBinder(Model, p));
            }
        }

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

        /*
        /// <summary>
        /// Get the title of the view, if applicable.
        /// Returns null if the model does not define this.
        /// </summary>
        [Obsolete]
        public virtual string Title
        {
            get
            {
                PropertyInfo titleProperty = Model?.GetType()?.GetProperties()?.Where(p => p.IsDefined<ViewTitleAttribute>()).FirstOrDefault();
                return titleProperty?.GetValue(Model)?.ToString();
            }
        }
        */

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
    /// Metadata of property on an instance.
    /// Provides access to the property value.
    /// </summary>
    public class ModelPropertyBinder : BoundProperty<object, object>
    {
        public ModelPropertyBinder(object target, PropertyInfo property) : base(target, property)
        {
        }

        public ModelPropertyBinder(object target, string propertyName) : base(target, propertyName)
        {
        }

        /// <summary>
        /// The data type of the property.
        /// May be null if the provider of the data does not support it (it may not be a real object property).
        /// </summary>
        public virtual Type PropertyType
        {
            get { return Property?.PropertyType; }
        }

    }

}
