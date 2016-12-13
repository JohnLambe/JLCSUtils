using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    public abstract class MvpAttribute : Attribute
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
    public class PresenterAttribute : MvpAttribute
    {
//        public Type ViewInterface { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PresenterForActionAttribute : MvpAttribute
    {
        public PresenterForActionAttribute(Type forModel, Type forAction)
        {
            ForAction = forAction;
            ForModel = forModel;
        }

        //        public Type ViewInterface { get; set; }

        public Type ForModel { get; set; }

        public Type ForAction { get; set; }
    }


    /// <summary>
    /// Flags a View for automatic registration with the DI system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : MvpAttribute
    {
    }
}
