using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using System.Linq.Expressions;
using DiExtension.Attributes;
using JohnLambe.Util.Reflection;
using System.Reflection;
using DiExtension.ConfigInject;

namespace DiExtension.SimpleInject
{
    public class InjectionBehaviorBase
    {
        public InjectionBehaviorBase(IConfigProvider provider)
        {
            ConfigProvider = provider;
        }

        /// <summary>
        /// Returns the key to be used to lookup a value for the given item in the DI container.
        /// </summary>
        /// <param name="member">The item to be injected. (Can be a property or a constructor parameter.)</param>
        /// <returns>key to use. null if the item should not be resolved by key.</returns>
        public static string GetKeyForMember(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<InjectAttribute>();
            var key = attribute?.Key;
            if (key == InjectAttribute.CodeName)     // if using member name as key
            {
                key = member.Name;                   // use the member (property/parameter) name
            }
            return key;
        }

        public static string GetKeyForConsumer(InjectionConsumerInfo consumer)
        {
            var attribute = GetAttributeForConsumer(consumer);
            var key = attribute?.Key;
            if (key == InjectAttribute.CodeName)     // if using member name as key
            {
                key = consumer.Target.Parameter?.Name
                    ?? consumer.Target.Member.Name;  // use the property/parameter name
            }
            return key;
        }

        public virtual object GetValue(string key, Type requiredType)
        {
            object value;
            if (ConfigProvider.GetValue<object>(key, requiredType, out value))
                return value;
            else
                throw new DependencyInjectionException("Getting value failed for key: " + key);
        }

        public virtual bool TryGetExpression(InjectionConsumerInfo member, Type requiredType, out Expression expression)
        {
            var key = GetKeyForConsumer(member);
            if (key != null)    // if it has a key
            {
                object value;
                bool resolved = ConfigProvider.GetValue<object>(key, requiredType, out value);
                // For properties, `resolved` should always be true. Otherwise the Property Selection Behaviour should have rejected the property.
                if (resolved)
                {
                    if (!CacheValues)
                    {   // return an Expression to lookup the key each time it is required:
                        expression =  Expression.Convert(         // convert to the required type
                            Expression.Call(
                                Expression.Constant(this), GetType().GetMethod("GetValue", new Type[] { typeof(string), typeof(Type) }),  // GetValue method of this instance
                                Expression.Constant(key), Expression.Constant(requiredType)
                            ),
                            requiredType);
                        return true;
                    }
                    else
                    {
                        expression = Expression.Constant(value, requiredType);   // return a constant
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Resolution failed for " + member + "; Key=" + key);
                    //TODO: capture information to be added to exception if resolving by other method fails
                }
            }
            expression = null;
            return false;
        }

        protected static InjectAttribute GetAttributeForConsumer(InjectionConsumerInfo consumer)
        {
            return consumer.Target.Parameter != null ?
                consumer.Target.Parameter.GetCustomAttribute<InjectAttribute>()
                : consumer.Target.Member?.GetCustomAttribute<InjectAttribute>();
        }

        /// <summary>
        /// Looks up values to be injected by name.
        /// </summary>
        protected readonly IConfigProvider ConfigProvider;

        /// <summary>
        /// Iff false, keys are resolved to Expression, so that the they are re-evaluated each time they are injected.
        /// </summary>
        public virtual bool CacheValues { get; set; } = true;
    }


    /// <summary>
    /// SimpleInjector extension for injecting properties based on an attribute.
    /// </summary>
    public class PropertyInjectionDiBehavior : InjectionBehaviorBase, IDependencyInjectionBehavior
    {
        /// <summary>
        /// The underlying IDependencyInjectionBehavior.
        /// This class overrides this and delegates to it for anything that this class does not resolve.
        /// </summary>
        protected readonly IDependencyInjectionBehavior Existing;

        public PropertyInjectionDiBehavior(IDependencyInjectionBehavior existing, IConfigProvider context) : base(context)
        {
            Existing = existing;
        }

        public static PropertyInjectionDiBehavior RegisterWith(ContainerOptions options, IConfigProvider context)
        {
            options.PropertySelectionBehavior = new InjectAttributePropertySelectionBehavior(context);
            var diBheaviour = new PropertyInjectionDiBehavior(
                options.DependencyInjectionBehavior, context);
            options.DependencyInjectionBehavior = diBheaviour;
            return diBheaviour;
        }

        #region IDependencyInjectionBehavior methods

        public virtual Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            if (CanResolve(consumer))
            {
                Expression expr;
                if (TryGetExpression(consumer, consumer.Target.TargetType, out expr))
                    return expr;
                // if not resolved here, the default behaviour (resolving by type) will be used.
                //TODO?: Throw exception instead - it would make is easier to diagnose misconfigurations.
            }

            if (consumer.Target.Member.GetCustomAttribute<InjectAttribute>()?.ByType != false)   // if injecting by type is supported (if no attribute, it is)
                return Existing.BuildExpression(consumer);

            throw new DependencyInjectionException("Resolving failed for " + consumer.Target.Name);
        }

        public virtual void Verify(InjectionConsumerInfo consumer)
        {
            if (!CanResolve(consumer))      // if we can't resolve it, test with the underlying behaviour
                Existing.Verify(consumer);
        }

        #endregion

        /// <summary>
        /// True iff this behaviour can resolve the given item itself
        /// (without passing to the underlying behaviour).
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        protected virtual bool CanResolve(InjectionConsumerInfo consumer)
        {
            var attribute = GetAttributeForConsumer(consumer);  // get the attribute
            return attribute != null && attribute.Enabled;             // if the attribute is present and its Enabled property is true
            // (Member is probably never null. Tested anyway above.)
/*                if (consumer.Target.Member.GetCustomAttributes(typeof(InjectAttribute)).Any())
                {
                    return true;
                }
*/
        }

    }
}
