using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Wrapper for a model, used by control binders (<see cref="IControlBinder"/>).
    /// </summary>
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
            var property = Model.GetType().GetProperty(propertyName);
            return property?.GetValue(Model);
        }

        public virtual void SetValue(string propertyName, object value)
        {
            var property = Model.GetType().GetProperty(propertyName);
            property?.SetValue(Model, value);
        }

        public virtual bool CanRead(string propertyName)
            => GetProperty(propertyName)?.CanRead ?? false;

        protected virtual PropertyInfo GetProperty(string propertyName)
            => Model.GetType().GetProperty(propertyName);

        public virtual bool CanWrite(string propertyName)
            => GetProperty(propertyName)?.CanWrite ?? false;

        public virtual string GetCaptionForProperty(string propertyName)
        {
            var property = Model.GetType().GetProperty(propertyName);
            return property?.Name;
        }

        protected readonly object Model;
    }
}
