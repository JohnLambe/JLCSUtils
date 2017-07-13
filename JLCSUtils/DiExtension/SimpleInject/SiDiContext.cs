using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension.ConfigInject;
using DiExtension.ConfigInject.Providers;
using System.Reflection;

using Microsoft.Practices.Unity;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Collections;
using JohnLambe.Util.TypeConversion;

using SimpleInjector;
using JohnLambe.Util.Types;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// Dependency injection container that extends SimpleInjector's container.
    /// </summary>
    public class SiDiContext : IDiContext, IDiResolver, IDiTypeRegistrar, IDiInstanceRegistrar
    {
        /// <summary>
        /// Initialise with a new <see cref="global::SimpleInjector.Container"/>.
        /// </summary>
        public SiDiContext() : this(new Container())
        {
        }

        /// <summary>
        /// Initialise with an existing <see cref="global::SimpleInjector.Container"/>.
        /// </summary>
        /// <param name="container"></param>
        public SiDiContext(Container container)
        {
            Container = container;
            Init();
            InitialRegistrations();
        }

        /// <summary>
        /// Container setup (adding extensions, configuring etc.).
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// Registers items that are automatically initialsed after container setup.
        /// </summary>
        protected virtual void InitialRegistrations()
        {
            Container.RegisterSingleton<IDiContext>(this);
        }

        /*  Other methods in Unity version:
                public virtual IDiContext RegisterType(Type type, string name = null, params InjectionMember[] i);
                public virtual IDiContext RegisterType(Type from, Type to, params InjectionMember[] i);
                public virtual IDiContext RegisterAndBuilpUpInstance(string name, object instance, bool buildUp = true);
        */

        /// <summary>
        /// Run dependency injection on an instance (inject properties).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual T BuildUp<T>(T target)
        {
            InstanceProducer producer;
            try
            {
                // From SI documentation:
                producer = Container.GetRegistration(target.GetType(), throwOnFailure: true);
            }
            catch (Exception ex) when(ex is ActivationException | ex is DependencyInjectionException)
            {
                producer = Container.GetCurrentRegistrations().Where(r => typeof(T).IsAssignableFrom(r.ServiceType)).FirstOrDefault();
                //                producer = Container.GetCurrentRegistrations().Where(r => typeof(T).IsAssignableFrom(r.Registration.ImplementationType)).First();
                if (producer == null)
                    throw new DependencyInjectionException("Can't run property injection on " + target);
            }

            Registration registration = producer.Registration;
            registration.InitializeInstance(target);

            return target;

            //TODO?: trap exceptions and rethrow with added information.
        }

        /// <summary>
        /// Get the implementation type for the given service type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual Type ResolveType(Type serviceType)
        {
            return Container.GetRegistration(serviceType)?.Registration?.ImplementationType;
        }

        public override string ToString()
        {
            return base.ToString()
                + " Container: " + Container.GetCurrentRegistrations().ToString();
        }

        public virtual void RegisterType(Type implementationType)
        {
            Container.Register(implementationType);
        }

        public virtual void RegisterType(Type serviceType, Type implementationType)
        {
            Container.Register(serviceType, implementationType);
        }

        /// <summary>
        /// Register a singleton instance to be resolved by its compile-time type.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual IDiContext RegisterInstance<TService>(TService instance)
            where TService : class
        {
            Container.RegisterSingleton<TService>(instance);
            return this;
        }

        /// <summary>
        /// Register a singleton instance to be resolved by its runtime type.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual IDiContext RegisterInstanceByRuntimeType([NotNull] object instance)
        {
            Container.RegisterSingleton(instance.GetType(), instance);
            return this;
        }

        /// <summary>
        /// Register a singleton instance to be resolved by a specified type.
        /// </summary>
        /// <param name="serviceType">The type to resolve to <paramref name="instance"/>.
        /// <paramref name="instance"/> must be of a type that is assignable to this type.
        /// </param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual IDiContext RegisterInstance(Type serviceType, object instance)
        {
            Container.RegisterSingleton(serviceType, instance);
            return this;
        }

        /*  Uses extension method instead:
        void IDiInstanceRegistrar.RegisterInstance(object instance)
        {
            RegisterInstance(instance);
        }
        */

        void IDiInstanceRegistrar.RegisterInstance(Type serviceType, object instance)
        {
            RegisterInstance(serviceType, instance);
        }

        /// <summary>
        /// Get an instance of type <typeparamref name="T"/> from the DI container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual T GetInstance<T>()
            where T: class
        {
            return Container.GetInstance<T>();
        }

        /// <summary>
        /// Get an instance of <paramref name="serviceType"/> from the DI container.
        /// </summary>
        /// <typeparam name="T">Type to cast the result to (it may be any type that `serviceType` can be cast to).</typeparam>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual T GetInstance<T>(Type serviceType)
        {
            return (T)Container.GetInstance(serviceType);
        }

        public virtual void RegisterTypes(Type serviceType, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                assemblies = new [] { Assembly.GetExecutingAssembly() };

            IEnumerable<Type> typesToRegister = 
                Container.GetTypesToRegister(serviceType, assemblies, new TypesToRegisterOptions() { IncludeGenericTypeDefinitions = true });

            foreach (var type in typesToRegister)
                Container.Register(type);
                //RegisterType(serviceType, type);
        }

        public virtual T GetInstanceFor<T>(MemberInfo member)
        {
            return GetInstance<T>(GetMemberDataType(member));
        }

        public virtual T GetInstanceFor<T>(ParameterInfo member)
        {
            return GetInstance<T>(member.ParameterType);
        }

        /// <summary>
        /// Get the Type of value stored in the given member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="DependencyInjectionException">If this member type is not supported.</exception>
        protected virtual Type GetMemberDataType(MemberInfo member)
        {
            var type = (member as PropertyInfo)?.PropertyType;   // only Properties are currnetly supported
            if (type == null)
            {
                throw new DependencyInjectionException("Injection failed for member " + member.DeclaringType?.FullName + "." + member.ToString()
                    + ": Member type not supported (only properties are supported)");
            }
            return type;
        }


        /// <summary>
        /// SimpleInjector's container.
        /// </summary>
        public virtual Container Container { get; private set; }
    }


    /// <summary>
    /// Adds some features to <see cref="SiDiContext"/>.
    /// </summary>
    public class SiExtendedDiContext : SiDiContext, IExtendedDiContext, IDiExtInstanceRegistrar
    {
        public SiExtendedDiContext() : base()
        {
        }

        public SiExtendedDiContext(Container container) : base(container)
        {
        }

        protected override void Init()
        {
            ProviderChain = new ConfigProviderChain();
            Provider = new CompositeProvider(ProviderChain);
            Dictionary = new DictionaryConfigProvider<object>();
            ProviderChain.RegisterProvider(Dictionary);
            ConstructorResolutionBehavior.RegisterWith(Container.Options);

            PropertyInjectionBehavior = PropertyInjectionDiBehavior.RegisterWith(Container.Options,this,this);
            CacheValues = true;

            base.Init();
        }

        /// <summary>
        /// Scan an assembly for providers (identified by attributes) and register them.
        /// </summary>
        /// <param name="assm">The assembly to scan. null for the assembly of the caller.</param>
        /// <param name="filter">Filter to specify what types to register.</param>
        public virtual void ScanAssembly(Assembly assm = null, BooleanExpression<Type> filter = null)
        {
            if (assm == null)
                assm = Assembly.GetCallingAssembly();
            new DiContextConfigurer(this).ScanAssembly(assm, filter);
        }

        public virtual void ScanAssemblies(IEnumerable<Assembly> assemblies, BooleanExpression<Type> filter = null)
        {
            foreach (var assm in assemblies)
                ScanAssembly(assm);
        }


        public override T GetInstanceFor<T>(MemberInfo member)
        {
            var key = PropertyInjectionDiBehavior.GetKeyForMember(member);
            if (key == null)
            {
                return base.GetInstanceFor<T>(member);
            }
            else
            {
                return GetValue<T>(key, GetMemberDataType(member));
            }
        }

        public override T GetInstanceFor<T>(ParameterInfo member)
        {
            var key = PropertyInjectionDiBehavior.GetKeyForMember(member);
            if (key == null)
            {
                return base.GetInstanceFor<T>(member);
            }
            else
            {
                return GetValue<T>(key, member.ParameterType);
            }
        }


        /// <summary>
        /// Resolve a ConfigInject value.
        /// </summary>
        /// <typeparam name="T">Type to cast the resolved value to.</typeparam>
        /// <param name="key"></param>
        /// <param name="type">The required type.</param>
        /// <param name="value">The resolved value, or default(<typeparamref name="T"/>) if not resolved.</param>
        /// <returns>true iff resolved.</returns>
        public virtual bool GetValue<T>(string key, Type type, out T value)
        {
            return Provider.GetValue(key, type, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>The resolved value.</returns>
        /// <exception cref="DependencyInjectionException">If resolving fails.</exception>
        public virtual T GetValue<T>(string key, Type type)
        {
            T value;
            if (!GetValue<T>(key, type, out value))
                throw new DependencyInjectionException("Injection failed for key '" + key + "'; Type: " + type.FullName);
            return value;
        }

        public virtual void RegisterType(Type serviceType, string name)
        {
            throw new NotImplementedException("General resolving by Name only is not implemented for SimpleInjector");
            // could implement it, at least for properties, using ConfigInject.
        }

        /// <summary>
        /// Register a named instance with the DI container.
        /// </summary>
        /// <param name="name">The name of this instance.</param>
        /// <param name="instance">The instance to be registered.</param>
        /// <param name="buildUp">Iff true, dependency injection is run on the <paramref name="instance"/> (for property injection, etc.).</param>
        public virtual void RegisterInstance(string name, object instance, bool buildUp = false)
        {
            Dictionary.AsDictionary.SetValue(name, instance);
            if(buildUp)
                BuildUp(instance);
        }

        /// <summary>
        /// If true, values looked up by a key are cached (so if they are modified after being resolved,
        /// the old value will still be used on subsequent resolving).
        /// This is the usual behavior of SimpleInjector.
        /// </summary>
        public virtual bool CacheValues
        {
            get { return PropertyInjectionBehavior.CacheValues; }
            set { PropertyInjectionBehavior.CacheValues = value; }
        }

        public virtual ConfigProviderChain ProviderChain { get; private set; }
        protected virtual PropertyInjectionDiBehavior PropertyInjectionBehavior { get; private set; }

        protected IConfigProvider Provider { get; private set; }
        protected DictionaryConfigProvider<object> Dictionary { get; private set; }
    }

}
