using JohnLambe.Util.DependencyInjection.Attributes;
using JohnLambe.Util.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection.SimpleInject
{
    public class DiSimpleInjectorExtension
    {

    }
/*
    public class ConfigInjectBehaviour : IDependencyInjectionBehavior
    {
        protected IDependencyInjectionBehavior Existing { get; private set; }

        public ConfigInjectBehaviour(IDependencyInjectionBehavior existing, ExtendedDiContext diContext)
        {
            this.Existing = existing;
            this._diContext = diContext;
        }

        /// <summary>
        /// Register this instance with the given container (ContainerOptions instance).
        /// </summary>
        /// <param name="options"></param>
        /// <param name="diContext"></param>
        public static void RegisterWith(ContainerOptions options, ExtendedDiContext diContext)
        {
            options.DependencyInjectionBehavior = new ConfigInjectBehaviour(
                options.DependencyInjectionBehavior,diContext);
            options.PropertySelectionBehavior = new InjectAttributePropertySelectionBehavior(diContext);  ///TODO: Chain
        }

        #region IDependencyInjectionBehavior methods

        public virtual Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            if (CanResolve(consumer))
            {
                var key = GetConfigKey(consumer); 
                object value;

                if (_diContext.ResolveValue(key, consumer.Target.TargetType, out value))
                    return Expression.Constant(key, consumer.Target.TargetType);
            }

            return Existing.BuildExpression(consumer);
        }

        public virtual void Verify(InjectionConsumerInfo consumer)
        {
            if (!CanResolve(consumer))      // if we can't resolve it, test with the underlying behaviour
                Existing.Verify(consumer);
        }

        #endregion

        protected virtual string GetConfigKey(InjectionConsumerInfo consumer)
        {
            return consumer.Target.Member.GetCustomAttribute<InjectAttribute>().Name;
            //TODO: Use member name as key if none in Attribute ?
        }

        /// <summary>
        /// True iff this behaviour can resolve the given item itself
        /// (without passing to the underlying behaviour).
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        protected virtual bool CanResolve(InjectionConsumerInfo consumer)
        {
            if (consumer.Target.Property != null)             // if a property
            {
                if (consumer.Target.Property.GetCustomAttributes(typeof(InjectAttribute),true).Any())  // if it has the attribute
                    return true;
            }
            return false;
        }

        protected IExtendedDiContext _diContext;
    }
    */
}
