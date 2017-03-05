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

        // Methods obsoleted. Use GetProp instead:

        [Obsolete]
        public virtual object GetValue(string propertyName)
        {
            //            return GetProperty(propertyName)?.GetValue(Model);
            return ReflectionUtils.TryGetPropertyValue<object>(Model, propertyName);
        }

        [Obsolete]
        public virtual void SetValue(string propertyName, object value)
            //            => GetProperty(propertyName)?.SetValue(Model, value);
            => ReflectionUtils.TrySetPropertyValue<object>(Model, propertyName, value);

        [Obsolete]
        public virtual bool CanRead(string propertyName)
            => GetProperty(propertyName)?.CanRead ?? false;

        [Obsolete]
        public virtual bool CanWrite(string propertyName)
            => GetProperty(propertyName)?.CanWrite ?? false;

        /// <summary>
        /// Get a property of the model.
        /// Returns null if this model does not support it.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual PropertyInfo GetProperty(string propertyName)
        {
            var model = Model; 
            return ReflectionUtils.GetProperty(ref model, propertyName);
        }

        [Obsolete]
        public virtual string GetCaptionForProperty(string propertyName)
        {
            return CaptionUtils.GetDisplayName(GetProperty(propertyName));
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

        public virtual IEnumerable<ModelPropertyBinder> Properties
        {
            get
            {
                return Model.GetType().GetProperties()
                    .Select(p => new ModelPropertyBinder(Model, p));
            }
        }

        public virtual ModelPropertyBinder GetProp(string propertName)
        {
            return new ModelPropertyBinder(Model, propertName);
        }

        /// <summary>
        /// Get the title of the view, if applicable.
        /// Returns null if the model does not define this.
        /// </summary>
        public virtual string Title
        {
            get
            {
                PropertyInfo titleProperty = Model?.GetType()?.GetProperties()?.Where(p => p.IsDefined<ViewTitleAttribute>()).FirstOrDefault();
                return titleProperty?.GetValue(Model)?.ToString();
            }
        }

        protected readonly object Model;
    }


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
