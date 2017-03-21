using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using JohnLambe.Util;
using JohnLambe.Util.Reflection;

namespace MvpFramework
{

    /// <summary>
    /// Resolves the Presenter for a Model, View for a Presenter,
    /// and other MVP-related resolving.
    /// </summary>
    public abstract class MvpResolver
    {
        #region Naming convention
        // Constants relating to conventions in class names.
        // These could be made public.

        /// <summary>
        /// Conventional suffix in names of Presenter classes.
        /// </summary>
        protected const string PresenterSuffix = "Presenter";

        /// <summary>
        /// Conventional suffix in names of View classes.
        /// </summary>
        protected const string ViewSuffix = "View";

        /// <summary>
        /// Optional suffix in names of Model classes.
        /// (Domain classes usually won't use it. A Model for a certain type of dialog probably would.)
        /// </summary>
        protected const string ModelSuffix = "Model";

        /// <summary>
        /// Prefix of names of interfaces.
        /// </summary>
        protected const string InterfacePrefix = "I";

        protected const string PresenterNamespace = PresenterSuffix;

        protected const string ViewNamespace = ViewSuffix;

        protected const string InterfaceSuffix = "Interface";

        protected const string ViewInterfaceNamespace = ViewSuffix + InterfaceSuffix;

        #endregion

        #region Namespaces

        /// <summary>
        /// If <paramref name="originalNamespace"/> ends with <paramref name="oldNamespace"/>,
        /// change <paramref name="oldNamespace"/> to <paramref name="newNamespace"/>,
        /// otherwise, return <paramref name="originalNamespace"/>.
        /// <para>Change 'Namespace.{old}' to 'Namespace.{new}'. </para>
        /// </summary>
        /// <param name="originalNamespace"></param>
        /// <param name="oldNamespace"></param>
        /// <param name="newNamespace">The new last part of the namespace.</param>
        /// <returns></returns>
        protected virtual string ChangeNamespace(string originalNamespace, string oldNamespace, string newNamespace)
        {
            if (originalNamespace.EndsWith(oldNamespace))
                return originalNamespace.RemoveSuffix("." + oldNamespace) + "." + newNamespace;
            else
                return originalNamespace;
            //return originalNamespace.RemoveSuffix("." + PresenterNamespace) + "." + newNamespace;
        }

        /// <summary>
        /// Same as <see cref="ChangeNamespace(string, string, string)"/> 
        /// where <paramref name="originalNamespace"/> is the conventional namespace.
        /// </summary>
        /// <param name="originalNamespace"></param>
        /// <param name="newNamespace"></param>
        /// <returns></returns>
        protected virtual string ChangePresenterNamespace(string originalNamespace, string newNamespace)
        {
            return ChangeNamespace(originalNamespace, PresenterNamespace, newNamespace);
        }

        #endregion

        /*
                public virtual Type GetModel(string modelName)
                {
                    return Type.GetType(modelName);  //TODO apply default namespace
                }
        */

        /// <summary>
        /// Get a presenter of a known concrete type.
        /// </summary>
        /// <typeparam name="TPresenter"></typeparam>
        /// <param name="presenterType">The concrete presenter type to be returned. Must be assignable to <typeparamref name="TPresenter"/>.</param>
        /// <param name="param">The parameters to the 'Create' method of the factory for the required presenter.</param>
        /// <returns>The requested Presenter.</returns>
        public abstract TPresenter GetPresenterByType<TPresenter>(Type presenterType, params object[] param);

        /// <summary>
        /// Get the Presenter for a given action on a given model.
        /// </summary>
        /// <typeparam name="TPresenter">A Presenter interface for the action to be done with the model.</typeparam>
        /// <typeparam name="TModel">The type of the Model.</typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual TPresenter GetPresenterForModel<TPresenter, TModel>(TModel model)
            where TPresenter : class, IPresenter
        {
            return GetPresenterForModel<TPresenter, TModel>(typeof(TPresenter), typeof(TModel), model);
        }

        /// <summary>
        /// Same as <see cref="GetPresenterForModel{TPresenter, TModel}(TModel)"/> except that the types (generic type paramters to that method)
        /// are passed as parameters.
        /// </summary>
        /// <typeparam name="TPresenter"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="presenterActionType"></param>
        /// <param name="modelType">The type of the model.</param>
        /// <param name="model">The model instance (must be assignable to <paramref name="modelType"/>).</param>
        /// <returns></returns>
        public virtual TPresenter GetPresenterForModel<TPresenter, TModel>(Type presenterActionType, Type modelType, TModel model = default(TModel))
            where TPresenter : class, IPresenter
        {
            Type presenterType = /*ResolvePresenterTypeForAction(presenterActionType, modelType)
                ?? */ ResolvePresenterType(presenterActionType, modelType);
            if (presenterType != null)
                return CreatePresenter<TPresenter, TModel>(presenterType, model);
            //                return GetInstance<TPresenter>(presenterType);

            throw new MvpResolutionException("Resolution failed for Model: " + modelType.FullName);

            /*            var presenterType = Type.GetType(model.GetType().FullName.RemoveSuffix(ModelSuffix)
                            + action + PresenterSuffix);    // change '<Name>[Model]' to '<Name><Action>Presenter'
                        // Create Presenter:
                        return ClassUtils.Instantiate<TPresenter>(presenterType, new Type[] { typeof(TModel) }, new object[] { model });
                        //TODO: Get from DI container?
                        */
        }

        /// <summary>
        /// Create a Presenter of a known type.
        /// </summary>
        /// <typeparam name="TPresenter"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="presenterType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual TPresenter CreatePresenter<TPresenter, TModel>(Type presenterType, TModel model)
            where TPresenter : class, IPresenter
        {
            return GetInstance<TPresenter>(presenterType);
            //TODO: model ?
        }

        /// <summary>
        /// Get the concrete Presenter type for a given Presenter interface, and optionally, Model type.
        /// </summary>
        /// <param name="presenterInterface">The Presenter interface to be resolved.
        /// Must be <see cref="IPresenter"/> or an interface derived from it.</param>
        /// <param name="modelType">The type of the Model.</param>
        /// <returns>The Presenter class.</returns>
        public virtual Type ResolvePresenterType(Type presenterInterface, Type modelType = null)
        {
            Contract.Requires(presenterInterface != null);

            var resolvedPresenterType = ResolvePresenterTypeForAction(presenterInterface, modelType);
            if (resolvedPresenterType != null)
                return resolvedPresenterType;

            if (modelType != null)
            {   // try to find an interface or class based on the names of the interface and the model:

                // form components of the conventional name:
                string actionName = presenterInterface.Name.RemovePrefix(InterfacePrefix).RemoveSuffix(PresenterSuffix);
                // name describing the action of the required presenter, e.g. a presenter action interface of 'IViewDetailPresenter' would yield an action of "ViewDetail".
                string modelName = modelType.Name.RemoveSuffix(ModelSuffix);
                // name describing the model that this presenter acts on. e.g. a class called 'ArticleModel' or 'Article' would yield a model name of "Article".
                //TODO: attribute on model to override modelName
                string targetNamespace = presenterInterface.Namespace;  //TODO

                // form the conventional name for the presenter interface:
                string presenterInterfaceName = targetNamespace + "." + InterfacePrefix + actionName + modelName + PresenterSuffix;
                // I{Action}{Model}Presenter
                Type resolvedPresenterInterface = GetTypeByName(presenterInterfaceName, presenterInterface.Assembly);
                if (resolvedPresenterInterface != null)                   // if an interface with this name exists
                {
                    var result = ResolveType(resolvedPresenterInterface);
                    if (result != null)
                        return result;
                }

                // If not found by resolving the conventional interface name:
                // form the conventional name for the presenter class:
                string presenterClassName = targetNamespace + "." + actionName + modelName + PresenterSuffix;
                Type resolvedPresenterClass = GetTypeByName(presenterClassName, presenterInterface.Assembly);
                if (resolvedPresenterClass != null)
                    return resolvedPresenterClass;
            }

            // if no model type given, or if the above (using the model type failed), just use DI to get the type for the given Presenter interface:
            resolvedPresenterType = ResolveType(presenterInterface);
            if (resolvedPresenterType != null)
                return resolvedPresenterType;

            throw new MvpResolutionException("Resolution failed for Presenter type: " + presenterInterface.FullName + ", " + modelType?.FullName);
        }

        public virtual Type ResolvePresenterTypeForAction(Type actionInterface, Type modelType)
        {
            // Initial inefficient implementation.
            //TODO: Does not currently support multiple PresenterForActionAttributes on the same type.
            //      Does not currently recognise a presenter that handles a superclass (or other type is assignable from) the target one.
            //TODO: Cache mappings.

            if (Assemblies == null)
                return null;
            /*
                        var a1 = Assemblies.SelectMany(a => a.GetTypes())
                            .Select(t => new AttributeAndType<PresenterForActionAttribute>() { DeclaringType = t, Attribute = t.GetCustomAttribute<PresenterForActionAttribute>() });
                        var b = a1
                            .Where(at => at.Attribute != null && at.Attribute.ForAction == actionInterface && at.Attribute.ForModel == modelType);
                        var c = b
                            .Select(at1 => at1.DeclaringType)
                            .FirstOrDefault()
            //                ?.DeclaringType;
            ;
            */
            return Assemblies.SelectMany(a => a.GetTypes())
                .Select(t => new AttributeAndType<PresenterForActionAttribute>() { DeclaringType = t, Attribute = t.GetCustomAttribute<PresenterForActionAttribute>() })
                .Where(at => at.Attribute != null && at.Attribute.ForAction == actionInterface && at.Attribute.ForModel == modelType)
                .Select(at1 => at1.DeclaringType)
                .FirstOrDefault()
            //                ?.DeclaringType;
            ;
        }

        /// <summary>
        /// Assemblies to scan for resolving by Action / Model.
        /// </summary>
        public virtual Assembly[] Assemblies { get; set; }

        /*
        /// <summary>
        /// Get the Presenter for a given action on a given model.
        /// </summary>
        /// <typeparam name="TPresenter">The type of the Presenter.</typeparam>
        /// <typeparam name="TModel">The type of the Model.</typeparam>
        /// <param name="model"></param>
        /// <param name="action">The action to be done with the model.</param>
        /// <returns></returns>
        public virtual TPresenter Resolve<TPresenter, TModel>(TModel model, string action)
            where TPresenter : class
        {
            var presenterType = Type.GetType(model.GetType().FullName.RemoveSuffix(ModelSuffix)
                + action + PresenterSuffix);    // change '<Name>[Model]' to '<Name><Action>Presenter'
            // Create Presenter:
            return ClassUtils.Instantiate<TPresenter>(presenterType, new Type[] { typeof(TModel) }, new object[] { model });
            //TODO: Get from DI container?
        }
    */

        /// <summary>
        /// Get the View for a given Presenter.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TPresenter">The type of the Presenter.</typeparam>
        /// <param name="presenter"></param>
        /// <returns></returns>
        public virtual TView GetViewForPresenter<TView, TPresenter>(TPresenter presenter)
            where TView : IView
            where TPresenter : IPresenter
        {
            Contract.Requires(presenter != null);
            return GetViewForPresenterType<TView>(presenter.GetType());

            /*
            Type viewGenericArg = presenter.GetType().GetInterfaces().FirstOrDefault(i => i is IPresenter<TView>)
                ?.GetGenericArguments()?[0];   // can throw IndexOutOfRange if view interface has no generic type arguments
            return (TView)GetInstance(viewGenericArg);
            */

            //return (TView)GetInstance(presenter.ViewType);

            // Get an instance of the type of the TView generic parameter to TPresenter from the DI container.
            /*            Type[] genericArgs = typeof(TPresenter).GenericTypeArguments;
                        if (genericArgs.Length > 0)
                        {
                            Type viewType = genericArgs[0];
                            // viewType must be assignable to TView, but is not necessarily TView:
                            // it could be derived from it, or a class that implements it (though the latter is not recommended).
                            // Resolving must use the type declared in the presenter class, not the type used in this call.
                            return (TView)GetInstance(viewType);
                        }
                        */
            /*
            var presenterType = Type.GetType(model.GetType().FullName.RemoveSuffix(PresenterSuffix) + ViewSuffix);    // change '<Name>[Controller]' to '<Name>View'
            // Create Presenter:
            return ClassUtils.Instantiate<C>(presenterType, new Type[] { typeof(M) }, new object[] { model });
            */
        }

        /// <summary>
        /// Given a presenter type, create a view of the appropriate type.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="presenterType"></param>
        /// <returns></returns>
        public virtual TView GetViewForPresenterType<TView>(Type presenterType)
            where TView : IView
        {
            Contract.Requires(presenterType != null);

            // form the conventional simple name:
            string simpleName = presenterType.Name.RemoveSuffix(PresenterSuffix) + ViewSuffix;
            // {namespace}.["I"]{Name}["Presenter"] -> ["I"]{Name}"View"
            // e.g. "namespace.IEditContactPresenter"  -> "IEditContactView"
            if (presenterType.IsInterface)
                simpleName = simpleName.RemovePrefix(InterfacePrefix);   // remove leading "I" if it in an interface

            // Get the View interface name by naming convention:
            // Try the same namespace as the Presenter:
            string viewInterfaceName = presenterType.Namespace + "."
                + InterfacePrefix + simpleName;
            Type viewInterfaceType = GetTypeByName(viewInterfaceName, presenterType.Assembly);      // get the interface type for this name

            if (viewInterfaceType == null)       // if not found, try the conventional separate namespace
            {
                viewInterfaceName = ChangePresenterNamespace(presenterType.Namespace, ViewInterfaceNamespace) + "."
                    + InterfacePrefix + simpleName;
                viewInterfaceType = GetTypeByName(viewInterfaceName, presenterType.Assembly);      // get the interface type for this name
            }

            if (viewInterfaceType != null)
            {
                var result = GetInstance<TView>(viewInterfaceType);              // get an implementation of this interface
                if (result != null)
                    return result;
            }

            // Get the View class name by naming convention:
            string viewClassName = ChangePresenterNamespace(presenterType.Namespace, ViewNamespace) + "." + simpleName;
            Type viewClassType = GetTypeByName(viewClassName, presenterType.Assembly);      // get the type for this name
            if (viewClassType != null)
            {
                var result = GetInstance<TView>(viewInterfaceType);              // get an implementation of this interface
                if (result != null)
                    return result;
            }

            throw new MvpResolutionException("Resolution failed on resolving View for Presenter: " + presenterType.FullName + ".\n"
                + (viewInterfaceType == null ? "Failed to resolve View Interface (Check that it is defined with a conventional name, e.g. " + viewInterfaceName + ")\n"
                : "View Interface: " + viewInterfaceType.FullName + " (Check that there is a DI registration for this interface, or an implementing class with the [View] attribute)\n")
                + (viewClassType != null ? "Tried getting class " + viewClassType.FullName + " from DI\n"
                : !string.IsNullOrEmpty(viewClassName) ? "No class found called " + viewClassName + "\n" : "")
                );
        }

        /// <summary>
        /// Get the presenter interface (IPresenter subinterface) for the given presenter class.
        /// </summary>
        /// <param name="presenterType"></param>
        /// <returns></returns>
        public virtual Type ResolveInterfaceForPresenterType(Type presenterType)
        {
            return ResolveInterfaceForClass<IPresenter, PresenterAttribute>(presenterType);
        }

        /// <summary>
        /// Get the view interface (IView subinterface) for the given view class.
        /// </summary>
        /// <param name="presenterType"></param>
        /// <returns></returns>
        public virtual Type ResolveInterfaceForViewType(Type viewType)
        {
            return ResolveInterfaceForClass<IView, ViewAttribute>(viewType);
        }

        /// <summary>
        /// Return the presenter/view interface type for a presenter/view class:
        /// The interface that a DI framework typically resolves to this class.
        /// </summary>
        /// <typeparam name="TRequiredInterface">Base type of the required interface.</typeparam>
        /// <typeparam name="TAttribute">An attribute that may be on the class, specifying the interface type.</typeparam>
        /// <param name="classType">A presenter or view class.</param>
        /// <returns>The presenter/view interface type.</returns>
        protected virtual Type ResolveInterfaceForClass<TRequiredInterface, TAttribute>(Type classType)
                where TAttribute : MvpAttribute
                where TRequiredInterface : class
        {
            // look for attribute first:
            Type resolvedInterface = classType.GetCustomAttribute<TAttribute>()?.Interface;

            /*
            var attribute = classType.GetCustomAttribute<TAttribute>();
            if (attribute != null)
            {
                if (attribute.Interface != null)
                    resolvedInterface = attribute.Interface;
            }
            */

            if (resolvedInterface == null)     // if not resolved by the attribute
            {
                string simpleName = InterfacePrefix + classType.Name;

                string targetNamespace = classType.Namespace;

                // form the conventional name for the interface:
                string interfaceName = targetNamespace + "." + simpleName;
                resolvedInterface = GetTypeByName(interfaceName, classType.Assembly);

                if (resolvedInterface == null)
                {
                    targetNamespace = classType.Namespace + InterfaceSuffix;  // e.g. "namespace.View" => "namespace.ViewInterface"

                    // form the conventional name for the interface:
                    interfaceName = targetNamespace + "." + simpleName;
                    resolvedInterface = GetTypeByName(interfaceName, classType.Assembly);
                }
            }

            if (resolvedInterface != null)
            {
                // Validate the interface to be returned:
                if (!resolvedInterface.IsInterface)
                    throw new MvpResolutionException("Invalid presenter/view interface: " + resolvedInterface.FullName + " - not an interface");
                if (!typeof(TRequiredInterface).IsAssignableFrom(resolvedInterface))
                    throw new MvpResolutionException("Invalid presenter/view interface: " + resolvedInterface.FullName + " - not derived from " + typeof(TRequiredInterface).FullName);
            }

            // may be null

            return resolvedInterface;
        }

        #region Resolving types

        /// <summary>
        /// Create an instance of the given type (from DI, etc.).
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        protected abstract T GetInstance<T>(Type forType);

        protected abstract Type ResolveType(Type serviceType);

        protected virtual Type GetTypeByName(string typeName, Assembly assm = null)
        {
            if (assm == null)
                assm = Assembly.GetEntryAssembly();
            return assm.GetType(typeName);
        }

        #endregion
    }


    /// <summary>
    /// MVP resolver using dependency injection.
    /// </summary>
    public class DiMvpResolver : MvpResolver
    {
        /// <summary>
        /// The name of the 'Create' method of <see cref="IPresenterFactory{TPresenter}"/> (and other generic variants).
        /// </summary>
        protected const string CreateMethodName = "Create";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diContext">Interface to the dependency injection container for resolving.</param>
        public DiMvpResolver(IDiResolver diContext)
        {
            this.Context = diContext;
        }

        public override TPresenter GetPresenterByType<TPresenter>(Type presenterType, params object[] param)
        {
            if (param == null)
                param = new object[] { };
            Type[] paramTypes = ReflectionUtils.ArrayOfTypes(param);   // the types of the parameters to the factory's Create method
            //TODO: Test with runtime types different to (but assignable to) the actual types.

            // Make the generic factory type:
            Type factoryType = GenericTypeUtils.ChangeGenericParameters(typeof(KnownPresenterFactory<>), 
                (new Type[] { presenterType }).Concat(paramTypes).ToArray());
            //typeof(PresenterFactory<,>).MakeGenericType(presenterType, param.GetType());

            // Get its constructor:
            var factoryConstructor = factoryType.GetConstructor(new Type[] { GetType(), typeof(IDiResolver), typeof(IResolverExtension), typeof(Type) });

            // Invoke the constructor to get the factory:
            var factory = factoryConstructor.Invoke(new object[] { this, Context, GetInstance<IResolverExtension>(typeof(IResolverExtension)), presenterType });

            // Get the factory method:
            var createMethod = factoryType.GetMethod(CreateMethodName, paramTypes);

            // Invoke the factory method to get the instance:
            return (TPresenter)createMethod.Invoke(factory, param);
        }

        protected override T GetInstance<T>(Type forType)
        {
            try
            {
                return Context.GetInstance<T>(forType);
            }
            catch (Exception /* ex*/)    //TODO: Filter exception type (currently depends on IDiResolver implementation)
            {   // could either have failed to resolve the type or to cast it to the required type
                //                Console.WriteLine(ex);
                return default(T);
            }
        }

        protected override Type ResolveType(Type serviceType)
        {
            return Context.ResolveType(serviceType);
            //TODO null if not resolved
        }

        protected override TPresenter CreatePresenter<TPresenter, TModel>(Type presenterType, TModel model)
        {
            // Create a factory to create the required Presenter type:
            IPresenterFactory<TPresenter, TModel> factory = new KnownPresenterFactory<TPresenter, TModel>(this, Context, GetInstance<IResolverExtension>(typeof(IResolverExtension)), presenterType);

            // Create the Presenter:
            return factory.Create(model);
        }

        protected readonly IDiResolver Context;
    }


    public class AttributeAndType<T>
        where T : Attribute
    {
        public Type DeclaringType;
        public T Attribute;
    }

}