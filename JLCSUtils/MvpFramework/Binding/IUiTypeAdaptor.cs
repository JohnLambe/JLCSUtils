using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public interface IUiTypeAdaptor<T>
    {
        string DisplayText { get; }

        string EditText { get; set; }

        OptionCollection Options { get; }

        // ValidaitonResultEx

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UiTypeAdaptorAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// The class that acts as an adaptor for the attributed item.
        /// Must be a class that implements <see cref="IUiTypeAdaptor{T}"/>.
        /// </summary>
        public virtual Type Adaptor { get; set; }
    }
}
