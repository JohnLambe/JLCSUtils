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
    public abstract class MvpClassAttribute : MvpAttribute
    {
        /// <summary>
        /// The Presenter/View Interface, that the DI container should map to the attributed class.
        /// </summary>
        public virtual Type Interface { get; set; }
    }

    /// <summary>
    /// Flags a Presenter for automatic registration with the DI system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PresenterAttribute : MvpClassAttribute
    {
//        public Type ViewInterface { get; set; }
    }

    /// <summary>
    /// Specifies that the attributed presenter can perform a certain action
    /// on a certain model type.
    /// <para>The attributed class must implement <see cref="IPresenter"/>.</para>
    /// </summary>
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
        public virtual Type ForModel { get; set; }

        /// <summary>
        /// Iff true, the attributed presenter can handle any type of model assignable to the given one.
        /// </summary>
        public virtual bool AcceptModelSubTypes { get; set; } = true;

        /// <summary>
        /// Identifies the action that the presenter can perform.
        /// This must be an interface or base class (or the class itself) of the attributed presenter.
        /// It is recommended that this is an interface for the action (or role) without requiring knowledge of the model type.
        /// </summary>
        public virtual Type ForAction { get; set; }

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
    }


    /// <summary>
    /// Flags a View for automatic registration with the DI system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : MvpClassAttribute
    {
    }
}
