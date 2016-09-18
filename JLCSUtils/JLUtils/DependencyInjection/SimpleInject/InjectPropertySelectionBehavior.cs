using JohnLambe.Util.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SimpleInjector.Advanced;
using JohnLambe.Util.DependencyInjection.ConfigInject;

namespace JohnLambe.Util.DependencyInjection.SimpleInject
{
    /// <summary>
    /// SimpleInjector extension for injecting properties based on an attribute.
    /// </summary>
    public class InjectAttributePropertySelectionBehavior : IPropertySelectionBehavior
    {
        public InjectAttributePropertySelectionBehavior(IConfigProvider diContext)
        {
            _configProvider = diContext;
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
            if ( attribute != null )   // if the prperty has the attribute
            {       //prop.GetCustomAttributes(typeof(InjectAttribute)).Any() 
                if (attribute.Required)   // if required
                {
                    return true;          // we will attempt to inject it and an exception will be thrown if it cannot be resolved
                }
                else
                {
                    // check that we have a value for it. Don't attempt to inject the property if we don't.
                    object value;
                    return _configProvider.GetValue(PropertyInjectionDiBehavior.GetKeyForMember(prop), prop.PropertyType, out value);  // true iff it can be resolved
                }
            }
            return false;
        }

        protected readonly IConfigProvider _configProvider;
    }
}
