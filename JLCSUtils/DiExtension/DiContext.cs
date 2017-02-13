using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension.ConfigInject;
using System.Reflection;

using Microsoft.Practices.Unity;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util;

namespace DiExtension
{
    public class DiContext : IDiContext
    {
        public DiContext(UnityContainer container)
        {
            Container = container;
            RegisterInstance(this);
        }

        public virtual IDiContext BuildUp<T>(T target, params ResolverOverride[] resolverOverride)
        {
            Container.BuildUp(target.GetType(), target, resolverOverride);
            //            Container.BuildUp<T>(target, resolverOverride);
            return this;
        }

        public virtual IDiContext RegisterType(Type type, string name = null, params InjectionMember[] i)
        {
            Container.RegisterType(type, name, i);
            return this;
        }

        public virtual IDiContext RegisterType(Type from, Type to, params InjectionMember[] i)
        {
            Container.RegisterType(from, to, i);
            return this;
        }

        public virtual IDiContext RegisterInstance(object instance)
        {
            Container.RegisterInstance(instance);
            return this;
        }

        public virtual void RegisterInstance(string name, object instance, bool buildUp = true)
        {
            if (name == null)
            {
                RegisterInstance(instance);
            }
            else
            {
                Container.RegisterInstance(name, instance);
            }
            if (buildUp)
                AutoBuildUp(instance);
            //            return this;
        }

        protected virtual void AutoBuildUp(object instance)
        {
            BuildUp(instance);
        }

        protected virtual UnityContainer Container { get; set; }

        T IDiContext.BuildUp<T>(T target)
        {
            BuildUp<T>(target, new ResolverOverride[] { });
            return target;
        }

        public override string ToString()
        {
            return base.ToString()
                + " Container: " + Container.Registrations.ToString();
        }

    }


    public class ExtendedDiContext : DiContext, IExtendedDiContext
    {
        public ExtendedDiContext(UnityContainer container) : base(container)
        {
            ProviderChain = new ConfigProviderChain();

            container.AddExtension(new Unity.DiUnityExtension(this));
        }

        /// <summary>
        /// Scan an assembly for classes (identified by a filter) and register them.
        /// </summary>
        /// <param name="assm"></param>
        public virtual void ScanAssembly(Assembly assm, BooleanExpression<Type> filter = null)
        {
            new DiContextConfigurer(this).ScanAssembly(assm, filter);
        }

        /// <summary>
        /// Resolve a ConfigInject value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool GetValue<T>(string name, Type type, out T value)
        {
            string configKey;
            string defaultValue;
            name.RemovePrefix(ConfigNamePrefix).SplitToVars('=', out configKey, out defaultValue);
                // Remove the prefix that flags this for ConfigInject, and extract the default value, if any.

            bool result = ProviderChain.GetValue(configKey, type, out value);
            if (!result && !string.IsNullOrEmpty(defaultValue))    // if failed and there is a default value
            {
                value = GeneralTypeConverter.Convert<T>(defaultValue, type);
                return true;
            }
            else
            {
                return result;
            }
        }

        public virtual void RegisterType(Type serviceType, Type implementationType)
        {
            base.RegisterType(serviceType, implementationType);
        }

        public virtual void RegisterType(Type serviceType, string name)
        {
            RegisterType(serviceType, name, new InjectionMember[] { });
        }

        public virtual ConfigProviderChain ProviderChain { get; protected set; }

        /// <summary>
        /// Prefix in [Inject] keys to use ConfigInject.
        /// </summary>
        public virtual string ConfigNamePrefix { get; set; } = DefaultConfigNamePrefix;

        /// <summary>
        /// Default value for <see cref="ConfigNamePrefix"/>.
        /// </summary>
        public const string DefaultConfigNamePrefix = "Config:";
    }

}
