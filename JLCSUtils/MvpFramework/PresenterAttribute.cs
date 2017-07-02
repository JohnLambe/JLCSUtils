using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Base class for attributes that flag a class as a Presenter or View to be registered automatically.
    /// </summary>
    public abstract class MvpClassAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// The Presenter/View Interface, that the DI container should map to the attributed class.
        /// If there are multiple interfaces, this returns the first one.
        /// Setting this replaces everything in <see cref="Interfaces"/>.
        /// <para>
        /// When placing this attribute, don't assign both this and <see cref="Interfaces"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="Interfaces">For when there are multiple interfaces.</seealso>
        [TypeValidation(IsInterface = true), Nullable]
        public virtual Type Interface
        {
            get
            {
                return Interfaces?.ElementAtOrDefault(0);
            }
            set
            {
                if (value == null)
                    Interfaces = null;
                else
                    Interfaces = new[] { value };
            }
        }

        /// <summary>
        /// The Presenter/View Interfaces, that the DI container should map to the attributed class.
        /// <para>
        /// When placing this attribute, don't assign both this and <see cref="Interface"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="Interface">For when there is only one interface.</seealso>
        [Nullable]
        // Each element: [TypeValidation(IsInterface = true)] 
        public virtual Type[] Interfaces { get; set; }
    }

    /// <summary>
    /// Flags a Presenter for automatic registration with the DI system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PresenterAttribute : MvpClassAttribute
    {
        /// <summary>
        /// The interface of the View for this Presenter.
        /// null to use the default resolution.
        /// </summary>
        [TypeValidation(Implements = typeof(IView)), Nullable]
        public Type ViewInterface { get; set; }
    }

    /// <summary>
    /// Specifies that the attributed presenter can perform a certain action
    /// on a certain model type.
    /// <para>The attributed class must implement <see cref="IPresenter"/>.</para>
    /// </summary>
    /// <seealso cref="MvpResolver.GetPresenterForModel{TPresenter, TModel}(TModel)"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PresenterForActionAttribute : MvpClassAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="forModel"><see cref="ForModel"/></param>
        /// <param name="forAction"><see cref="ForAction"/></param>
        public PresenterForActionAttribute(Type forModel, Type forAction)
        {
            ForAction = forAction;
            ForModel = forModel;
        }

        /// <summary>
        /// The type of the model.
        /// </summary>
        [NotNull]
        public virtual Type ForModel { get; set; }
        //| Could be called ModelType

        /// <summary>
        /// Iff true, the attributed presenter can handle any type of model assignable to <see cref="ForModel"/>.
        /// </summary>
        public virtual bool AcceptModelSubTypes { get; set; } = true;

        /// <summary>
        /// Identifies the action that the presenter can perform.
        /// This must be an interface or base class (or the class itself) of the attributed presenter.
        /// It is recommended that this is an interface for the action (or role) without requiring knowledge of the model type.
        /// </summary>
        [TypeValidation(IsValueType = false), NotNull]
        public virtual Type ForAction { get; set; }
        //| Could be called ActionHandled

        /// <summary>
        /// Iff true, the attributed presenter can be used for any action type assignable to <see cref="ForAction"/>.
        /// </summary>
        public virtual bool AcceptActionSubTypes { get; set; } = true;

        /// <summary>
        /// If there are multiple presenters that can handle the same model for the same action,
        /// that are otherwise equally preferred,
        /// this determines which one is used - the one with the lowest value is chosen.
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// Tests whether this class can handle a given action and model.
        /// </summary>
        /// <param name="actionInterface"><see cref="ForAction"/></param>
        /// <param name="modelType"><see cref="ForModel"/></param>
        /// <returns>true iff this can handle the given action and model.</returns>
        public virtual bool CanHandle(Type actionInterface, Type modelType)
            => Enabled
                && ForAction == actionInterface
                && /*AcceptModelSubTypes ? ForModel.IsAssignableFrom(modelType) :*/ ForModel == modelType;
        //TODO: Support AcceptModelSubTypes

        /*
        public virtual int MatchScore(Type actionInterface, Type modelType)
        {
            //if (!Enabled || ForAction != actionInterface)
            if(!CanHandle(actionInterface,modelType))
                return 0;

        }
        */
    }

    //| Instead of the above, we could use action interfaces with a model as a type parameter, e.g. IAction<TModel> ,
    //| and tag the interface with an attribute.
    //| An attribute on the presenter enables specifying a preference for one implementing presenter over others
    //| (and having some presenters not be resolved automatically to handle the type).
    /*
    [AttributeUsage(AttributeTargets.Interface,
        AllowMultiple = true,     // could be used for multiple model types
        Inherited = false         // the attribute is defined at the level at which mappings are declared. Mappings may not be declared at lower levels in the hierarchy.
        )]
    public class ActionInterfaceAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// The type of model handled.
        /// Iff null, the first generic type parameter of the interface is used.
        /// </summary>
        public virtual Type ModelType { get; set; }
    }
    */


    /// <summary>
    /// Flags a View for automatic registration with the DI system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : MvpClassAttribute
    {
    }
}
