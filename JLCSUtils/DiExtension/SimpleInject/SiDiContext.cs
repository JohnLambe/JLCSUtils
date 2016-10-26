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

namespace DiExtension.SimpleInject
{
    public class SiDiContext : IDiContext, IDiResolver, IDiTypeRegistrar
    {
        /// <summary>
        /// Initialise with a new <see cref="global::SimpleInjector.Container"/>.
        /// </summary>
        public SiDiContext() : this(new Container())
        {
        }

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

//        public virtual void BuildUp(object target)
        public virtual T BuildUp<T>(T target)
        {
            // From SI documentation:
            InstanceProducer producer =
                Container.GetRegistration(target.GetType(), throwOnFailure: true);
            Registration registration = producer.Registration;
            registration.InitializeInstance(target);

            return target;
        }

        /// <summary>
        /// Get the implementation type for the given service type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual Type ResolveType(Type serviceType)
        {
            return Container.GetRegistration(serviceType).Registration.ImplementationType;
        }

        public override string ToString()
        {
            return base.ToString()
                + " Container: " + Container.GetCurrentRegistrations().ToString();
        }

        public void RegisterType(Type implementationType)
        {
            Container.Register(implementationType);
        }

        public void RegisterType(Type serviceType, Type implementationType)
        {
            Container.Register(serviceType, implementationType);
        }

        public virtual IDiContext RegisterInstance(object instance)
        {
            Container.RegisterSingleton(instance);
            return this;
        }

        /// <summary>
        /// Get an instance of type `T` from the DI container.
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
        /// Get an instance of `serviceType` from the DI container.
        /// </summary>
        /// <typeparam name="T">Type to cast the result to (it may be any type that `serviceType` can be cast to).</typeparam>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual T GetInstance<T>(Type serviceType)
        {
            return (T)Container.GetInstance(serviceType);
        }

        public virtual void RegisterTypes(Type serviceType)
        {
            IEnumerable<Type> typesToRegister = 
                Container.GetTypesToRegister(serviceType, new Assembly[] { Assembly.GetExecutingAssembly() }, new TypesToRegisterOptions() { IncludeGenericTypeDefinitions = true });

            foreach (var type in typesToRegister)
                Container.Register(type);
                //RegisterType(serviceType, type);
        }

        /// <summary>
        /// SimpleInjector's container.
        /// </summary>
        public Container Container { get; private set; }
    }


    public class SiExtendedDiContext : SiDiContext, IExtendedDiContext
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

            PropertyInjectionDiBehavior.RegisterWith(Container.Options,this);

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
            return Provider.GetValue(name, type, out value);
        }

        public virtual void RegisterType(Type serviceType, string name)
        {
            throw new NotImplementedException("General resolving by Name only is not implemented for SimpleInjector");
            // could implement it, at least for properties, using ConfigInject.
        }

        public virtual void RegisterInstance(string name, object instance, bool buildUp = false)
        {
            Dictionary.AsDictionary.SetValue(name, instance);
            if(buildUp)
                BuildUp(instance);
        }


        public virtual ConfigProviderChain ProviderChain { get; private set; }
        protected IConfigProvider Provider { get; private set; }
        protected DictionaryConfigProvider<object> Dictionary { get; private set; }
    }

}
