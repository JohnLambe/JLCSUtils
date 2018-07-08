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
using JohnLambe.Util.Types;
using JohnLambe.Util.Diagnostic;
using JohnLambe.Util;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// Base class for <see cref="IDependencyInjectionBehavior"/> implementations (SimpleInjector extensions).
    /// </summary>
    public class InjectionBehaviorBase
    {
        /// <summary>
        /// </summary>
        /// <param name="provider">Provider that will be used to look up values for injection by a key.</param>
        public InjectionBehaviorBase(IConfigProvider provider)
        {
            ConfigProvider = provider;
        }

        /// <summary>
        /// Returns the key to be used to look up a value for the given item in the DI container.
        /// </summary>
        /// <param name="member">The item to be injected. (Can be a property or a constructor parameter.)</param>
        /// <param name="attribute">
        /// The <see cref="InjectAttribute"/> defined on the target member.
        /// If null, this is determined in this method.
        /// The caller should pass it, for efficiency, if it already has it.
        /// </param>
        /// <returns>key to use. null if the item should not be resolved by key.</returns>
        public static string GetKeyForMember([NotNull] MemberInfo member, [Nullable] InjectAttribute attribute = null)
        {
            attribute = attribute ?? member.GetCustomAttribute<InjectAttribute>();
            var key = attribute?.Key;
            if (key == InjectAttribute.CodeName)     // if using member name as key
            {
                key = member.Name;                   // use the member (property/parameter) name
            }
            return key;
        }

        //TODO: Refactor to reduce duplication between this and the other overload.
        public static string GetKeyForMember([NotNull] ParameterInfo member, [Nullable] InjectAttribute attribute = null)
        {
            attribute = attribute ?? member.GetCustomAttribute<InjectAttribute>();
            var key = attribute?.Key;
            if (key == InjectAttribute.CodeName)     // if using member name as key
            {
                key = member.Name;                   // use the member (property/parameter) name
                key = char.ToUpper(key[0]) + key.Substring(1);   // make the first letter capital
            }
            return key;
        }

        /// <summary>
        /// Get the key to use to look up a value for the given consumer.
        /// <para>
        /// If the attribute specifies that the name of the attributed item should be used,
        /// this returns the name of the attributed item. If it is a parameter, its first letter is capitalised.
        /// </para>
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="attribute">
        /// The <see cref="InjectAttribute"/> defined on the target member.
        /// If null, this is determined in this method.
        /// The caller should pass it, for efficiency, if it already has it.
        /// </param>
        /// <param name="propertyName">True to use the <see cref="InjectAttribute.Property"/> property. Otherwise, <see cref="InjectAttribute.Key"/> is used.</param>
        /// <returns>The key to use.</returns>
        public static string GetKeyForConsumer([NotNull] InjectionConsumerInfo consumer, [Nullable] InjectAttribute attribute = null, bool propertyName = false)
        {
            //TODO: refactor to call GetKeyForMember

            attribute = attribute ?? GetAttributeForConsumer(consumer);
            var key = propertyName ? attribute?.Property : attribute?.Key;
            if (key == InjectAttribute.CodeName)     // if using member name as key
            {
                key = consumer.Target.Parameter?.Name
                    ?? consumer.Target.Member.Name;  // use the property/parameter name
                if (consumer.Target.Parameter != null && key != null && key.Length > 0)  // if a paramter and key is not null or blank
                {
                    key = char.ToUpper(key[0]) + key.Substring(1);   // make the first letter capital
                }
            }
            return key;
        }

        /// <summary>
        /// Look up a value for injection, by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="requiredType">The type that the found value is required to be.
        /// It may be converted to this.</param>
        /// <returns>The looked up value. Must be of type <paramref name="requiredType"/>.</returns>
        /// <exception cref="DependencyInjectionException">If the lookup fails.</exception>
        public virtual object GetValue(string key, Type requiredType)
        {
            object value;
            if (ConfigProvider.GetValue<object>(key, requiredType, out value))
                return value;
            else
                throw new DependencyInjectionException("Getting value failed for key: " + key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="requiredType"></param>
        /// <param name="expression"></param>
        /// <param name="attribute">
        /// The <see cref="InjectAttribute"/> defined on the target member.
        /// If null, this is determined in this method.
        /// The caller should pass it, for efficiency, if it already has it.
        /// </param>
        /// <returns></returns>
        public virtual bool TryGetExpression(InjectionConsumerInfo member, Type requiredType, out Expression expression, [Nullable] InjectAttribute attribute = null)
        {
            var key = GetKeyForConsumer(member, attribute);
            if (key != null)    // if it has a key
            {
                object value;
                bool resolved = ConfigProvider.GetValue<object>(key, requiredType, out value);
                // For properties, `resolved` should always be true. Otherwise the Property Selection Behaviour should have rejected the property.
                if (resolved)
                {
                    if (!CacheValues)
                    {   // return an Expression to look up the key each time it is required:
                        expression =  Expression.Convert(         // convert to the required type
                            Expression.Call(
                                Expression.Constant(this), GetType().GetMethod(nameof(GetValue), new Type[] { typeof(string), typeof(Type) }),  // GetValue method of this instance
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
//                    Console.WriteLine("Resolution failed for " + member + "; Key=" + key);                    //TODO: remove
                    //TODO: capture information to be added to exception if resolving by other method fails
                }
            }
            expression = null;
            return false;
        }

        /// <summary>
        /// Get the <see cref="InjectAttribute"/> on the consumer.
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns>The attribute, or null if does not have one.</returns>
        protected static InjectAttribute GetAttributeForConsumer(InjectionConsumerInfo consumer)
        {
            return ((ICustomAttributeProvider)consumer.Target.Parameter ?? (ICustomAttributeProvider)consumer.Target.Member)
                .GetCustomAttribute<InjectAttribute>();
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
    /// SimpleInjector extension for injecting items (including properties) based on an attribute (<see cref="InjectAttribute"/>).
    /// </summary>
    public class PropertyInjectionDiBehavior : InjectionBehaviorBase, IDependencyInjectionBehavior
    {
        /// <summary>
        /// </summary>
        /// <param name="existing">Bahvior to use if this one fails (usually the behaviour in place before this one is installed).</param>
        /// <param name="provider">Provider that will be used to look up values for injection by a key.</param>
        public PropertyInjectionDiBehavior(IDependencyInjectionBehavior existing, IConfigProvider provider, IDiResolver resolver = null) : base(provider)
        {
            Existing = existing;
            DiResolver = resolver;
        }

        /// <summary>
        /// Register an instance of this with a SimpleInjector container.
        /// </summary>
        /// <param name="options">The <see cref="ContainerOptions"/> of the container to register with.</param>
        /// <param name="provider">Provider that will be used to look up values for injection by a key.</param>
        /// <returns></returns>
        public static PropertyInjectionDiBehavior RegisterWith(ContainerOptions options, IConfigProvider provider, IDiResolver resolver = null)
        {
            options.PropertySelectionBehavior = new InjectAttributePropertySelectionBehavior(provider);  // enable property injection
            var diBheaviour = new PropertyInjectionDiBehavior(
                options.DependencyInjectionBehavior, provider, resolver);         // new behavior that falls back on the existing one
            options.DependencyInjectionBehavior = diBheaviour;          // set up the new behavior
            return diBheaviour;
        }

        /// <summary>
        /// The underlying IDependencyInjectionBehavior.
        /// This class overrides this and delegates to it for anything that this class does not resolve.
        /// </summary>
        protected readonly IDependencyInjectionBehavior Existing;

        #region IDependencyInjectionBehavior methods

        /// <summary> <see cref="IDependencyInjectionBehavior.BuildExpression(InjectionConsumerInfo)"/> </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public virtual Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            InjectAttribute attribute = GetAttributeForConsumer(consumer);
            Type type = attribute?.ServiceType ?? consumer.Target.TargetType;
            var expr = BuildExpressionSimple(consumer, type, attribute);

            if (attribute?.Property != null)
            {
                string propertyName = GetKeyForConsumer(consumer,attribute,true);
                Type requiredType = consumer.Target.TargetType;    // the final required type (type of the property)

                expr = Expression.Convert(         // convert to the required type
                                    Expression.PropertyOrField(expr, propertyName),  // evaluate the specified property on the result of the original expression
                                requiredType);
            }

            return expr;
        }

        /// <summary>
        /// Returns an Expression to return the value to be injected.
        /// When <see cref="InjectAttribute.Property"/> is used, this returns just the initial instance on which the property is evaluated.
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="requiredType">The type returned by this expression.</param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public virtual Expression BuildExpressionSimple(InjectionConsumerInfo consumer, Type requiredType, InjectAttribute attribute)
        {
            bool triedByKey = false;                // true if we tried resolving by key

            if (CanResolve(consumer, attribute))
            {
                Expression expr;
                if (TryGetExpression(consumer, requiredType, out expr, attribute))
                    return expr;
                // if not resolved here, the default behaviour (resolving by type) will be used, if allowed by the attribute properties (ByType).

                triedByKey = true;
            }

            Exception existingBehaviorException = null;
            if (attribute?.ByType != false)   // if injecting by type is supported (if no attribute, it is)
            {
                try
                {
                    if (requiredType != consumer.Target.TargetType)
                        return CreateResolveExpression(requiredType);
                    else
                        return Existing.BuildExpression(consumer);
                }
                catch(Exception ex)
                {
                    existingBehaviorException = ex;
                }
            }

            throw new DependencyInjectionException(
                (existingBehaviorException != null ? "Exception on resolving for " + TargetToShortString(consumer.Target)
                + ": " + (existingBehaviorException.Message ?? "")
                : "Resolving failed for " + TargetToShortString(consumer.Target)
                ) 
                + StrUtil.NullPropagate(" (", consumer.Target?.Member?.DeclaringType?.FullName, " ", consumer.Target.Member?.ToString()
                    + (consumer.Target.Parameter != null ? " - Parameter#" + consumer.Target.Parameter.Position.ToString() : ""),
                    ")")    // shouldn't be null
                + "\n"
                + StrUtil.NullPropagate("\nService Type: ", consumer.ServiceType?.FullName, "\n")
                + StrUtil.NullPropagate("\nImplementation Type: ", consumer.ImplementationType?.FullName, "\n")
                + (attribute != null ? DiagnosticStringUtil.ObjectToString(attribute) + "\n" : "") 
                + (triedByKey ? " (Tried by Key)" : "")
                + (existingBehaviorException != null ? " (Tried by Type)" : ""),
                existingBehaviorException
                );
        }

        protected static string TargetToShortString(InjectionTargetInfo target)
        {
            return target == null ? "<null>" :
                (target.Property != null ? "property " + target.Member?.DeclaringType?.Name + "." + target.Property?.Name
                : target.Parameter != null ? "parameter " + /*(on " + target.Member?.DeclaringType?.Name + ") " + */ target.Parameter?.Name
                : target.ToString() );
        }

        /// <summary>
        /// Create an <see cref="Expression"/> to resolve the type <see cref="serviceType"/> from the DI container.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        /// <exception cref="DependencyInjectionException">If <see cref="DiResolver"/> is not assigned.</exception>
        protected virtual Expression CreateResolveExpression(Type serviceType)
        {
            if (DiResolver == null)
                throw new DependencyInjectionException("Accessing a property of a resolved item requires an IDiResolver, and none is provided.");
            return Expression.Convert(         // convert to the required type
                Expression.Call(
                    Expression.Constant(this), GetType().GetMethod("Resolve", new Type[] { typeof(Type) }),  // GetValue method of this instance
                    Expression.Constant(serviceType)
                ),
                serviceType);

//            return Expression.Call(
//                Expression.Constant(DiResolver), typeof(IDiResolver).GetMethod("GetInstance"), );
        }

        public virtual object Resolve(Type serviceType)
        {
            return DiResolver.GetInstance<object>(serviceType);
        }

        /// <summary> <see cref="IDependencyInjectionBehavior.Verify(InjectionConsumerInfo)"/> </summary>
        /// <param name="consumer"></param>
        public virtual void Verify(InjectionConsumerInfo consumer)
        {
            if (!CanResolve(consumer))      // if we can't resolve it, test with the underlying behavior
                Existing.Verify(consumer);
        }

        #endregion

        /// <summary>
        /// True iff this behaviour can resolve the given item itself
        /// (without passing to the underlying behaviour).
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="attribute">
        /// The <see cref="InjectAttribute"/> defined on the target member.
        /// If null, this is determined in this method.
        /// The caller should pass it, for efficiency, if it already has it.
        /// </param>
        /// <returns></returns>
        protected virtual bool CanResolve(InjectionConsumerInfo consumer, [Nullable] InjectAttribute attribute = null)
        {
            attribute = attribute ?? GetAttributeForConsumer(consumer);   // get attribute
            return attribute?.Enabled ?? false;  // if the attribute is present and its Enabled property is true
        }

        protected virtual IDiResolver DiResolver { get; }
    }
}
