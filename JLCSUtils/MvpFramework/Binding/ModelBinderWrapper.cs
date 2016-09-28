using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Wrapper for a model, used by control binders (<see cref="IControlBinder"/>).
    /// </summary>
    //| A model could be a map, or XmlNode, IConfigValueProvider, etc., and this class or a subclass (maybe refactored as an interface,
    //| with a factory that gets an implementation for a given model) could return properties in a standard way.
    //| Currently, it supports only accessing properties of an object.
    public class ModelBinderWrapper
    {
        public ModelBinderWrapper(object model)
        {
            this.Model = model;
        }

        public virtual void BindProperty(string propertyName /* ...,  */) //TODO
        {

        }

        public virtual object GetValue(string propertyName)
        {
            return GetProperty(propertyName)?.GetValue(Model);
        }

        public virtual void SetValue(string propertyName, object value)
            => GetProperty(propertyName)?.SetValue(Model, value);

        public virtual bool CanRead(string propertyName)
            => GetProperty(propertyName)?.CanRead ?? false;

        public virtual bool CanWrite(string propertyName)
            => GetProperty(propertyName)?.CanWrite ?? false;

        /// <summary>
        /// Get a property of the model.
        /// Returns null if this model does not support it.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual PropertyInfo GetProperty(string propertyName)
            => Model.GetType().GetProperty(propertyName);

        public virtual string GetCaptionForProperty(string propertyName)
        {
            return CaptionUtils.PropertyToCaption(GetProperty(propertyName));
        }

        protected readonly object Model;
    }
}
