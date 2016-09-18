using System;
using System.Linq;

using System.Reflection;
using System.Diagnostics;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;

namespace JohnLambe.Util.DependencyInjection.AutoFactory
{
    /// <summary>
    /// For creating factories that implement one of the IFactory&lt;TInterface,...&gt; interaces.
    /// The created factories call a constructor with the same parameters as the 'Create' method.
    /// This has to be provided with the actual class to be created.
    /// This is intended to be used with a system (such as a dependency injection framework)
    /// that resolves TInterface (the interface of the instance created) to a class.
    /// </summary>
    /// <typeparam name="TResolverParams">The type of resolver parameters used by the Resolve methods
    /// and <see cref="OnResolveAutoInterface"/> method.
    /// The consumer of this object chooses a type to use and provides events using that type.
    /// </typeparam>
    public class AutoFactoryFactory<TResolverParams>
    {
        /// <summary>
        /// Create a factory implementing <see cref="TFactoryInterface"/>,
        /// that creates instances of <see cref="TargetClass"/>.
        /// </summary>
        /// <typeparam name="TFactoryInterface"></typeparam>
        /// <param name="targetClass"></param>
        /// <returns></returns>
        public virtual TFactoryInterface CreateFactory<TFactoryInterface>(Type targetClass)
            where TFactoryInterface : class
        {
            return (TFactoryInterface)CreateFactory(typeof(TFactoryInterface), targetClass);
        }

        /// <summary>
        /// Create a factory implementing <see cref="TFactoryInterface"/>,
        /// that always returns <see cref="targetInstance"/>.
        /// </summary>
        /// <typeparam name="TFactoryInterface"></typeparam>
        /// <param name="targetClass"></param>
        /// <returns></returns>
        public virtual TFactoryInterface CreateFactoryForInstance<TFactoryInterface>(Type arguments, object targetInstance)
            where TFactoryInterface : class
        {
            return (TFactoryInterface)CreateFactory(typeof(TFactoryInterface), null, targetInstance);
        }

        public virtual object CreateFactory(Type arguments, Type targetClass, object targetInstance = null)
        {
            var genericArguments = arguments.GenericTypeArguments;
            if (genericArguments.Length < 1)
                throw new ArgumentException("AutoFactory: The interface must have at least one generic parameter");  // the interface must have at least one generic parameter - the type created by the factory
/*
            var interfaceType = genericArguments[0];
            Type[] parameterTypes = genericArguments.Skip(1).ToArray();
                // generic arguments of the factory interface except the first one (which is the type of the instance created by the factory)
                // - these are the parameters to the `Create` method.
*/

            var t = GetGenericClassType(genericArguments);    // the type of the factory class
            var constructor = t.GetConstructor(new Type[] { typeof(Type), typeof(object) });    // get the constructor
            return constructor.Invoke(new object[] { targetClass, targetInstance });    // create an instance of the factory
        }

        #region Resolve
        // This region is used for integration with dependency injection
        // but can also be called directly.

        public virtual TInterfaceType Resolve<TInterfaceType>(TResolverParams resolveParams)
        {
            return (TInterfaceType)ResolveEx(typeof(TInterfaceType),resolveParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType">The factory interface type.</param>
        /// <param name="resolveParams">Parameters to the resolver.</param>
        /// <returns></returns>
        public virtual object ResolveEx(Type interfaceType, TResolverParams resolveParams)
        {
            if (interfaceType != null)
            {
                if (interfaceType.IsInterface && interfaceType.HasCustomAttribute<AutoFactoryAttribute>())
                {
                    var genericArguments = interfaceType.GetGenericArguments();   // generic type arguments of the interface type
                    if (genericArguments != null && genericArguments.Length > 0)  // if it has generic arguments
                    {
                        var returnType = genericArguments[0];       // the return type of the `Create` method is the first one
                        var resolved = ResolveAutoInterface(returnType, resolveParams);
                        if (resolved != null)
                        {
                            if (resolved is Type)    // if resolved to a Type
                            {
                                return CreateFactory(interfaceType, (Type)resolved);
                                // create a factory implementing the required interface, to return the mapped type
                            }
                            else   // resolution returned an instance - there must be a mapping from 
                            {
                                return CreateFactory(interfaceType, null, resolved); ;
                                // create a factory that always returns the resolved instance.
                            }
                        }
                    }
                }
            }

            return null;
        }

        #region ResolveAutoInterface event

        /// <summary>
        /// Invoke the OnResolveAutoInterface event.
        /// </summary>
        /// <param name="interfaceType">The type that the factory returns.</param>
        /// <param name="resolveParams">Parameters to the resolver.</param>
        /// <returns>Resolved type or null (if not resolved, including if there are no event handlers).</returns>
        protected virtual object ResolveAutoInterface(Type interfaceType, TResolverParams resolveParams)
        {
            Debug.Assert(interfaceType != null);

            if (OnResolveAutoInterface != null)
                return OnResolveAutoInterface.Invoke(interfaceType, resolveParams);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType">The type that the factory returns. AutoFactoryFactory never passes null.</param>
        /// <param name="resolveParams">Parameters to the resolver: Any other value that the event handlers may use in resolving.
        /// The consumer of this class chooses a type </param>
        /// <returns>The class (Type) that the factory should create (if Type is returned),
        /// or an instance (of any type other Type and subclasses of it) that the factory should always create.</returns>
        //| `resolveParams` is an object rather than a 'params' array for efficiency (this may be called a lot by a DI framework).
        public delegate object ResolveAutoInterfaceDelegate(Type interfaceType, TResolverParams resolveParams);

        /// <summary>
        /// Called to resolve a type to be created by a factory to a type 
        /// or instance that implements it.
        /// </summary>
        public event ResolveAutoInterfaceDelegate OnResolveAutoInterface;

        #endregion

        #endregion

        /// <summary>
        /// Return the type `AutoFactory` with generic arguments for a Type and the specified number of 
        /// parameters of type `object`.
        /// </summary>
        /// <param name="argumentCount"></param>
        /// <returns></returns>
        protected static Type GetGenericClassType(int argumentCount)
        {
            // This could be done dynamically with reflection.
            switch(argumentCount)
            {
                case 0: return typeof(AutoFactory<Type>);
                case 1: return typeof(AutoFactory<Type, object>);
                case 2: return typeof(AutoFactory<Type, object, object>);
                case 3: return typeof(AutoFactory<Type, object, object, object>);
                case 4: return typeof(AutoFactory<Type, object, object, object, object>);
                case 5: return typeof(AutoFactory<Type, object, object, object, object, object>);
                default:
                    throw new ArgumentException("Unsupported number of generic parameters");
            }
        }

        /// <summary>
        /// Returns the (generic) type of the factory class, given the types of its generic arguments.
        /// </summary>
        /// <param name="genericArguments">The generic arguments of the factory class (same as one of the IFactory&lt;TInterface,...&gt; interaces).</param>
        /// <returns>the type of the factory class</returns>
        protected static Type GetGenericClassType(Type[] genericArguments)
        {
            return GetGenericClassType(genericArguments.Length-1).GetGenericTypeDefinition().MakeGenericType(genericArguments);
        }

/*
        public static AutoFactoryFactory Instance { get; private set; }

        static AutoFactoryFactory()
        {
            Instance = new AutoFactoryFactory();
        }
 */
    }

    /// <summary>
    /// Non-generic version of <see cref="AutoFactoryFactory"/>.
    /// This is useful when not using the Resolve methods, etc.
    /// </summary>
    public class AutoFactoryFactory : AutoFactoryFactory<object>
    {
    }

    /*
    public class Factory<TInterface> : IFactory<TInterface>
        where TInterface: class
    {
        public Factory(Type targetClass)
        {
            this.TargetClass = targetClass;
        }

        protected Type TargetClass { get; set; }

        public TInterface Create()
        {
            return (TInterface)TargetClass.GetConstructor(new Type[] {}).Invoke(new object [] {});
        }
    }

    public class Factory<TInterface, TParam> : IFactory<TInterface, TParam>
        where TInterface : class
    {
        public Factory(Type targetClass)
        {
            this.TargetClass = targetClass;
        }

        protected Type TargetClass { get; set; }

        public TInterface Create(TParam param)
        {
            return TargetClass.Create<TInterface>(new Type[] {typeof(TParam)}, new object[] { param });
        }
    }

*/

    /// <summary>
    /// Base class for classes that implement the IFactory&lt;TInterface,...&gt; interaces.
    /// Subclasses just implement the interface by delegating to a protected method of this class.
    /// </summary>
    /// <typeparam name="TInterface">The type returned from this factory (usually an interface).</typeparam>
    public abstract class AutoFactoryBase<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetClass">The class to create.</param>
        /// <param name="targetInstance">If not null, this instance is always returned.</param>
        public AutoFactoryBase(Type targetClass, object targetInstance = null)
        {
            if (targetInstance != null)
            {
                this.TargetInstance = targetInstance;
            }
            else
            {
                this.TargetClass = targetClass.ArgNotNull("AutoFactory: The target class cannot be null");
                var constructorArguments = GetType().GenericTypeArguments.Skip(1).ToArray();
                _constructor = targetClass.GetConstructor(constructorArguments);
                // generic arguments of the factory interface except the first one (which is the type of the instance created by the factory)
                if (_constructor == null)
                {
                    throw new ArgumentException("AutoFactory: " + targetClass.FullName + " does not have a constructor with the required arguments: " + constructorArguments.FormatCollection());
                }
            }
        }

        protected TInterface CreateInstance(params object[] constructorArguments)
        {
            if (TargetInstance != null)
                return (TInterface)TargetInstance;
            else
                return (TInterface)_constructor.Invoke(constructorArguments);
        }

        /// <summary>
        /// The class created by this factory.
        /// Currently, this is always null if TargetInstance != null.
        /// </summary>
        protected Type TargetClass { get; set; }

        /// <summary>
        /// If not null, this factory always returns this instance
        /// (regardless of the value of TargetClass).
        /// </summary>
        protected object TargetInstance { get; set; }

        /// <summary>
        /// The constructor of <see cref="TargetClass"/> used for creating instances by this factory.
        /// Should be null if <see cref="TargetClass"/> is null.
        /// </summary>
        private ConstructorInfo _constructor;
    }

}
