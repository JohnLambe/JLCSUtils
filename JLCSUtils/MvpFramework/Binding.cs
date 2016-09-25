using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework
{
    /// <summary>
    /// Resolves the Presenter for a Model or View for a Presenter.
    /// </summary>
    public class MvpResolver
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

        #endregion

        /// <summary>
        /// Get the Presenter for a given action on a given model.
        /// </summary>
        /// <typeparam name="TPresenter">A Presenter interface for the action to be done with the model.</typeparam>
        /// <typeparam name="TModel">The type of the Model.</typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
//        public virtual TPresenter Resolve<TPresenter, TModel>(TModel model, Type presenterAction)
        public virtual TPresenter ResolveForModel<TPresenter, TModel>(TModel model)
            where TPresenter : class
        {

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
        /// <param name="presenterInterface"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public virtual Type ResolvePresenterType(Type presenterInterface, Type modelType = null)
        {

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
        public virtual TView Resolve<TView, TPresenter>(TPresenter presenter)
            where TView : IView
            where TPresenter : IPresenter<TView>
        {
            Type viewGenericArg = presenter.GetType().GetInterfaces().FirstOrDefault(i => i is IPresenter<TView>)
                ?.GetGenericArguments()?[0];   // can throw IndexOutOfRange if view interface has no generic type arguments
            return (TView)GetInstance(viewGenericArg);

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

        public virtual IView ViewForPresenterType(Type presenterType)
        {

        }

        /// <summary>
        /// Create an instance of the given type (from DI, etc.).
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        protected virtual object GetInstance(Type forType)
        {

        }
    }


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
        public virtual void ShowForm<TView>(IPresenter<TView> presenter)
            where TView : IView
            //TODO: return value
        {
            /*
            if (!presenter.IsBound)                  // if not bound to a view
            {   // bind it now
                TView view = MvpResolver.Resolve<TView, IPresenter<TView> >(presenter); // resolve View
                presenter.Init(view);
            }
            */
            presenter.Show();
        }

        //TODO: Message dialogs:
        //public MessageDialogResult ShowDialog(MessageDialogModel p);
        // generic type for result:   public TResult ShowDialog(MessageDialogModel<TResult> p); ?

        protected MvpResolver MvpResolver { get; private set; }
    }


    /// <summary>
    /// Interface of an MVP Presenter.
    /// </summary>
    /// <typeparam name="TView">The type of the View for this Presenter. This SHOULD be an interface.</typeparam>
    public interface IPresenter<TView>
        where TView : IView
    {
        void Show();
        //TODO: decide return value. Type parameter?

        //        void Init(TView view);

        /// <summary>
        /// The type of the View for this Presenter.
        /// </summary>
        //        Type ViewType { get; }

        //        bool IsBound { get; }

    }

    public interface IPresenter
    {
        void Show();
    }

    /// <summary>
    /// Base class for Presenters.
    /// </summary>
    /// <typeparam name="TView">The type of the View. Should be an interface.</typeparam>
    /// <typeparam name="TModel">The type of the Model.
    /// Can be anything. Maybe not a primitive type?</typeparam>
    public class PresenterBase<TView, TModel> : IPresenter //<TView>
        where TView : IView
    {
        public PresenterBase(TView view, TModel model = default(TModel))
        {
            Contract.Requires(view != null);
            View = view;
            Model = model;
            Bind(view);
        }

        protected virtual void Bind(TView view)
        {
            //TODO: invoke automatic basic binding

        }

        //protected virtual void BindModel(TModel model)

        public virtual void Show()
        {
            //            Debug.Assert(IsBound, "View not assigned");
            View.Show();
        }


        protected virtual TModel Model { get; private set; }
        // protected?

        //protected virtual TView View { get; private set; }
        protected readonly TView View;

        /// <summary>
        /// The type of the View for this Presenter.
        /// </summary>
        public static Type ViewType => typeof(TView);

        //        public bool IsBound => View != null;

    }


    /// <summary>
    /// Base interface for Views - the interface to the View from the Presenter.
    /// </summary>
    // Note: Presenters reference only their own View. Any reference to other forms is through the Presenters of those forms.
    //  Models cannot reference Views.
    public interface IView
    {
        void Show();
        //* return value?
        //* should Show() be on base interface at all? A View could be for a non-form (panel, etc.).

        void BindModel(object model);
    }
    /* OR
    public interface IView<TResult>
    {
        TResult Show();
            // return value?
    }
    */


    interface IPresenterFactory<TPresenter, TModel>
        where TPresenter : IPresenter
    {
        TPresenter Create(TModel model);
    }

    /*
    interface IPresenterFactory<TPresenter, TView, TModel, TParam1>
        where TPresenter : IPresenter
    {
	    TPresenter Create(TModel model, TParam1 param1);
    }
    */

    public class PresenterFactory<TPresenter, TModel> : IPresenterFactory<TPresenter, TModel>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, UiNavigator navigator, IDiResolver diResolver
            /*, Type targetClass*/
            )
        {
            this.Navigator = navigator;
            this.DiResolver = diResolver;
            this.Resolver = resolver;
            //this.TargetClass = targetClass;
            this.TargetClass = Resolver.ResolvePresenterType(typeof(TPresenter), typeof(TModel));

            TargetConstructor = TargetClass.GetConstructors().First();
            //TODO if multiple constructors, choose one.
            //   Evaluate which are compatible? Use Attribute.
        }

        public virtual TPresenter Create(TModel model)
        {
            var parameters = TargetConstructor.GetParameters();
            object[] args = new object[parameters.Count()];
            int parameterIndex = 0;
            foreach (var parameter in parameters)
            {
                //                if(parameter.ParameterType.IsAssignableFrom(typeof(TView)))
                if (parameterIndex == 0)
                {
                    args[parameterIndex] = Navigator.ViewForPresenterType(typeof(TPresenter));
                    // OR Resolvser.ViewForPresenterType
                    // ...  (TargetClass)
                }
                else if (parameterIndex == 1)
                {
                    args[parameterIndex] = model;
                }
                else
                {
                    args[parameterIndex] = DiResolver.GetInstance<object>(parameter.ParameterType);
                }
                parameterIndex++;
            }

            return (TPresenter)TargetConstructor.Invoke(args);
        }

        protected readonly Type TargetClass;
        protected readonly ConstructorInfo TargetConstructor;
        protected readonly UiNavigator Navigator;
        protected readonly IDiResolver DiResolver;
        protected readonly MvpResolver Resolver;
    }

    public interface IDiResolver
    {
        T GetInstance<T>(Type serviceType);
    }


    //////////////////////
    // Binding:

    public interface IControlBinder
    {
        /// <summary>
        /// Bind the given model to the control.
        /// </summary>
        /// <param name="modelBinder"></param>
        void BindModel(ModelBinder modelBinder);
    }

    public class ControlBinder
    {
        public static IControlBinder GetControlBinder(Control control)
        {
            if (control is IControlBinder)
                return control as IControlBinder;
            else
                return new GeneralControlBinder(control);
            // We could return different classes based on a mapping from Control classes to IControlBinder implementations.
        }
    }

    public class GeneralControlBinder : IControlBinder
    {
        public GeneralControlBinder(Control control)
        {
            ControlToBind = control;
        }

        protected readonly Control ControlToBind;

        public void BindModel(ModelBinder modelBinder)
        {
            throw new NotImplementedException();
        }
    }

    public class ModelBinder
    {
        public ModelBinder(object model)
        {

        }

        public void BindProperty(string propertyName, )
        {

        }
    }


    public class ViewBase : Form, IView
    {
        public void BindModel(object model)
        {
            var modelBinder = new ModelBinder(model);
            foreach (Control control in Controls)
            {
                ControlBinder.GetControlBinder(control)?.BindModel(modelBinder);
            }
        }


        public void Show()
        {
            throw new NotImplementedException();
        }
    }


}
