using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using JohnLambe.Util;

namespace MvpFramework
{

    /// <summary>
    /// Resolves the Presenter for a Model or View for a Presenter.
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

        #endregion

        /// <summary>
        /// Get the Presenter for a given action on a given model.
        /// </summary>
        /// <typeparam name="TPresenter">A Presenter interface for the action to be done with the model.</typeparam>
        /// <typeparam name="TModel">The type of the Model.</typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        //        public virtual TPresenter Resolve<TPresenter, TModel>(TModel model, Type presenterAction)
        public virtual TPresenter GetPresenterForModel<TPresenter, TModel>(TModel model)
            where TPresenter : class
        {
            //TODO

            Type presenterType = ResolvePresenterType(typeof(TPresenter), typeof(TModel));
            if (presenterType != null)
                return GetInstance<TPresenter>(presenterType);

            throw new Exception("Resolution failed for Model: " + typeof(TModel).FullName);

            /*            var presenterType = Type.GetType(model.GetType().FullName.RemoveSuffix(ModelSuffix)
                            + action + PresenterSuffix);    // change '<Name>[Model]' to '<Name><Action>Presenter'
                        // Create Presenter:
                        return ClassUtils.Instantiate<TPresenter>(presenterType, new Type[] { typeof(TModel) }, new object[] { model });
                        //TODO: Get from DI container?
                        */
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

            if (modelType != null)
            {   // try to find an interface or class based on the names of the interface and the model:

                string actionName = presenterInterface.Name.RemovePrefix(InterfacePrefix).RemoveSuffix(PresenterSuffix);
                string modelName = modelType.Name.RemoveSuffix(ModelSuffix);
                string targetNamespace = presenterInterface.Namespace;
                //TODO: attribute on model to override modelName

                // form the conventional name for the presenter interface:
                string presenterInterfaceName = targetNamespace + "." + InterfacePrefix + actionName + modelName + PresenterSuffix;
                Type resolvedPresenterInterface = GetTypeByName(presenterInterfaceName, presenterInterface.Assembly);
                if (resolvedPresenterInterface != null)
                {
                    var result = ResolveType(resolvedPresenterInterface);
                    if (result != null)
                        return result;
                }

                // form the conventional name for the presenter class:
                string presenterClassName = targetNamespace + "." + actionName + modelName + PresenterSuffix;
                Type resolvedPresenterClass = GetTypeByName(presenterClassName, presenterInterface.Assembly);
                if (resolvedPresenterClass != null)
                    return resolvedPresenterClass;
            }

            // if no model type given, or if the above (using the model type failed), just use DI to get the type for the given Presenter interface:
            var resolvedPresenterType = ResolveType(presenterInterface);
            if (resolvedPresenterType != null)
                return resolvedPresenterType;

            throw new Exception("Resolution failed for Presenter type: " + presenterInterface.FullName + ", " + modelType?.FullName);
        }

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

        public virtual TView GetViewForPresenterType<TView>(Type presenterType)
            where TView: IView
        {
            Contract.Requires(presenterType != null);

            // Get the View interface name by naming convention:
            string viewInterfaceName = presenterType.Namespace + "."
                + InterfacePrefix + presenterType.Name.RemovePrefix(InterfacePrefix).RemoveSuffix(PresenterSuffix) + ViewSuffix;
            Type viewInterfaceType = GetTypeByName(viewInterfaceName, presenterType.Assembly);      // get the interface type for this name
            if (viewInterfaceType != null)
            {
                var result = GetInstance<TView>(viewInterfaceType);              // get an implementation of this interface
                if (result != null)
                    return result;
            }

            // Get the View class name by naming convention:
            string viewClassName = presenterType.Namespace + "."
                + presenterType.Name.RemovePrefix(InterfacePrefix).RemoveSuffix(PresenterSuffix) + ViewSuffix;
            Type viewClassType = GetTypeByName(viewClassName, presenterType.Assembly);      // get the type for this name
            if (viewClassType != null)
            {
                var result = GetInstance<TView>(viewInterfaceType);              // get an implementation of this interface
                if (result != null)
                    return result;
            }

            throw new Exception("Resolution failed for Presenter: " + presenterType.FullName);
        }

        /// <summary>
        /// Get the presenter interface (IPresenter subinterface) for the given presenter class.
        /// </summary>
        /// <param name="presenterType"></param>
        /// <returns></returns>
        public virtual Type ResolveInterfaceForPresenterType(Type presenterType)
        {
            return ResolveInterfaceForClass<IPresenter,PresenterAttribute>(presenterType);
            /*
            Type resolvedPresenterInterface = null;

            var attribute = presenterType.GetCustomAttribute<PresenterAttribute>();
            if(attribute != null)
            {
                if (attribute.Interface != null)
                    resolvedPresenterInterface = attribute.Interface;
            }

            if (resolvedPresenterInterface == null)
            {
                string targetNamespace = presenterType.Namespace;

                // form the conventional name for the presenter interface:
                string presenterInterfaceName = targetNamespace + "." + InterfacePrefix + presenterType.Name;
                resolvedPresenterInterface = GetTypeByName(presenterInterfaceName, presenterType.Assembly);
            }

            if (!resolvedPresenterInterface.IsInterface)
                throw new Exception("Invalid presenter interface: " + resolvedPresenterInterface.FullName + " - not an interface");
            if( !typeof(IPresenter).IsAssignableFrom(resolvedPresenterInterface) )
                throw new Exception("Invalid presenter interface: " + resolvedPresenterInterface.FullName + " - not derived from " + typeof(IPresenter).FullName);

            // may be null

            return resolvedPresenterInterface;
            */
        }

        public virtual Type ResolveInterfaceForViewType(Type viewType)
        {
            return ResolveInterfaceForClass<IView,ViewAttribute>(viewType);
        }

        protected virtual Type ResolveInterfaceForClass<TRequiredInterface,TAttribute>(Type classType)
                where TAttribute : MvpAttribute
                where TRequiredInterface: class
        {
            Type resolvedInterface = null;

            var attribute = classType.GetCustomAttribute<TAttribute>();
            if (attribute != null)
            {
                if (attribute.Interface != null)
                    resolvedInterface = attribute.Interface;
            }

            if (resolvedInterface == null)
            {
                string targetNamespace = classType.Namespace;

                // form the conventional name for the presenter interface:
                string presenterInterfaceName = targetNamespace + "." + InterfacePrefix + classType.Name;
                resolvedInterface = GetTypeByName(presenterInterfaceName, classType.Assembly);
            }

            if (!resolvedInterface.IsInterface)
                throw new Exception("Invalid presenter/view interface: " + resolvedInterface.FullName + " - not an interface");
            if (!typeof(TRequiredInterface).IsAssignableFrom(resolvedInterface))
                throw new Exception("Invalid presenter/view interface: " + resolvedInterface.FullName + " - not derived from " + typeof(TRequiredInterface).FullName);

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
        /// 
        /// </summary>
        /// <param name="diContext">Interface to the dependency injection container for resolving.</param>
        public DiMvpResolver(IDiResolver diContext)
        {
            this.Context = diContext;
        }

        protected override T GetInstance<T>(Type forType)
        {
            try
            {
                return Context.GetInstance<T>(forType);
            }
            catch(Exception)    //TODO: Filter exception type (currently depends on IDiResolver implementation)
            {
                return default(T);
            }
        }

        protected override Type ResolveType(Type serviceType)
        {
            return Context.ResolveType(serviceType);
            //TODO null if not resolved
        }

        protected readonly IDiResolver Context;
    }


    /*
    /// <summary>
    /// Handles showing or moving between forms and dialogs.
    /// </summary>
    public class UiNavigator
    {
        public UiNavigator(MvpResolver resolver)
        {
            MvpResolver = resolver;
        }

        /// <summary>
        /// Bind the presenter to a View if it is not already bound, and show it.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="presenter"></param>
        public virtual void ShowForm<TView>(IPresenter presenter)
            where TView : IView
            //TODO: return value
        {
            /-*
            if (!presenter.IsBound)                  // if not bound to a view
            {   // bind it now
                TView view = MvpResolver.Resolve<TView, IPresenter<TView> >(presenter); // resolve View
                presenter.Init(view);
            }
            *-/
            presenter.Show();
        }

        //TODO: Message dialogs:
        //public MessageDialogResult ShowDialog(MessageDialogModel p);
        // generic type for result:   public TResult ShowDialog(MessageDialogModel<TResult> p); ?

        protected MvpResolver MvpResolver { get; private set; }
    }
*/
}