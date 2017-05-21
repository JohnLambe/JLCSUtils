using DiExtension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SimpleInjector.Advanced;
using DiExtension.ConfigInject;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// SimpleInjector extension for injecting properties based on an attribute.
    /// </summary>
    public class InjectAttributePropertySelectionBehavior : IPropertySelectionBehavior
    {
        /// <summary>
        /// </summary>
        /// <param name="provider">Provider that will be used to look up values for injection by a key.</param>
        public InjectAttributePropertySelectionBehavior(IConfigProvider provider)
        {
            _configProvider = provider;
        }

        /// <summary>
        /// Property is selected iff it has an <see cref="InjectAttribute"/> attribute.
        /// </summary>
        /// <param name="type">the type which defines the property.</param>
        /// <param name="prop">the candidate property</param>
        /// <returns>true iff this property can be injected.</returns>
        public virtual bool SelectProperty(Type type, PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<InjectAttribute>();
            if ( attribute?.Enabled ?? false )   // if the member has the attribute, and it's Enabled property is true
            {
                if (attribute.Required)   // if required
                {
                    return true;          // we will attempt to inject it and an exception will be thrown if it cannot be resolved
                }
                else
                {
                    // check that we have a value for it. Don't attempt to inject the property if we don't.
                    object value;
                    return _configProvider.GetValue(PropertyInjectionDiBehavior.GetKeyForMember(prop,attribute), prop.PropertyType, out value);  // true iff it can be resolved
                }
            }
            return false;    // this won't be injected by this behavior
        }

        /// <summary>
        /// Provider for looking up values for injection by a key.
        /// </summary>
        protected readonly IConfigProvider _configProvider;
    }
}
