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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PresenterAttribute : MvpAttribute
    {

        /// <summary>
        /// 
        /// </summary>
//        public Type ViewInterface { get; set; }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : MvpAttribute
    {
    }
}
