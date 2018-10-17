using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Binding;

namespace MvpFramework
{
    /// <summary>
    /// Constants used in UI components.
    /// </summary>
    public static class MvpUiComponentConsts
    {
        /// <summary>
        /// The category, in a UI designer, of properties relating to this framework on user interface controls,
        /// e.g. for WinForms <see cref="System.ComponentModel.CategoryAttribute"/>.
        /// </summary>
        public const string DesignerCategory = "MVP";

        /// <summary>
        /// The description, for use in a UI designer, for a property with <see cref="MvpModelPropertyAttribute"/>.
        /// </summary>
        public const string ModelPropertyNameDescription = "The name of the bound property on the model.";

        /// <summary>
        /// The description, for use in a UI designer, for a property with <see cref="MvpModelPropertyAttribute"/>, returning a delegate.
        /// </summary>
        public const string ModelPropertyDelegateDescription = "The delegate to return the value of the bound property.";

        /// <summary>
        /// The description, for use in a UI designer, for a property with <see cref="MvpHandlerIdPropertyAttribute"/>.
        /// </summary>
        public const string HandlerIdDescription = "The ID of the handler on Presenter.";
    }
}
