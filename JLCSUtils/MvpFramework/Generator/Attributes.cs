using MvpFramework.Binding;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Generator
{
    #region For Controls

    /// <summary>
    /// Defines a mapping between controls and the types they handle.
    /// Used for generating controls from a model.
    /// </summary>
    public class MvpControlMappingAttribute : MvpEnabledAttributeBase
    {
        /// <summary>
        /// The data type handled (displayed / edited) by this control.
        /// If it handles more than one, this returns the first one.
        /// Setting this sets <see cref="ForTypes"/> to an array with only the assigned value.
        /// </summary>
        public virtual Type ForType
        {
            get { return ForTypes?[0]; }
            set { ForTypes = new[] { value }; }
        }

        /// <summary>
        /// The data type(s) handled (displayed / edited) by the attributed control.
        /// </summary>
        public virtual Type[] ForTypes { get; set; }
    }

    /// <summary>
    /// Flags a static method to be called by the form generator engine (<see cref="Generator.FormGeneratorBase{TControl}"/>)
    /// to create the control.
    /// The method must be called <see cref="GenerateControlMethod"/>.
    /// <para>For use on static methods of controls only.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MvpGenerateControlAttribute : MvpAttributeBase
    {
        /// <summary>
        /// Name of the static method on control classes, for generating a UI.
        /// </summary>
        public const string GenerateControlMethod = "GenerateControl";
    }

    #endregion

    #region For models

    /// <summary>
    /// Attributes for methods that generate view of data.
    /// <para>
    /// Parameters can be injected from a DI container (attributed with <see cref="DiExtension.Attributes.InjectAttribute"/>)
    /// or supplied by the consumer or user interface (attributed with <see cref="MvpParamAttribute"/>).
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class ListGeneratorBaseAttribute : MvpUiAttributeBase
    {
    }

    /// <summary>
    /// Flags a static method that returns an <see cref="IQueryable"/> for a list
    /// of items for display or output (e.g. a list to choose from in a user interface, or a report).
    /// <para>
    /// The <see cref="MvpUiAttributeBase"/> properties provide details of how this view of the data
    /// appears in list in which the user can choose between the available attributed methods
    /// (e.g. a dropdown box could show a list of these where the user can switch between them).
    /// </para>
    /// </summary>
    public class ListGeneratorAttribute : ListGeneratorBaseAttribute
    {
    }

    /// <summary>
    /// Flags a static method that takes the output of a method on the method attributed with <see cref="ListGeneratorAttribute"/>
    /// and returns a new <see cref="IQueryable"/> that modifies it.
    /// This could filter, sort, project or change the data in other ways.
    /// <para>
    /// These could provide a parameter to a report (possibly with a UI generated to enter it)
    /// or check box (or other control to enter a parameter to this method) on a page/form where the data is displayed.
    /// </para>
    /// <para>The <see cref="MvpUiAttributeBase.Order"/> property specifies the order in which modifier
    /// are show in user interfaces or other lists (NOT the order in which they are applied).</para>
    /// </summary>
    public class ListGeneratorModifierAttribute : ListGeneratorBaseAttribute
    {
        public virtual DataViewModifierType ModifierType { get; set; }

        /// <summary>
        /// Specifies the order of applying modifiers.
        /// 0 if unspecified.
        /// </summary>
        public virtual int ApplyOrder { get; set; }

        //TODO: Properties to specify which basic views (ListGeneratorAttribute),
        //  and other modifiers this is compatible with.
    }

    public enum DataViewModifierType
    {
        None = 0,
        Filter = 1,
        Sort,
        Project
    }

    //TODO?: Grouping??

    #endregion

    #region For Menu

    /// <summary>
    /// Placed on a model class to specify that a menu item should be generated for it.
    /// </summary>
    public class GenerateMenuItemAttribute : MenuItemAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="parentId">Value of <see cref="MenuAttributeBase.ParentId"/>.</param>
        /// <param name="displayName">Value of <see cref="MenuAttributeBase.DisplayName"/>.</param>
        public GenerateMenuItemAttribute(string parentId, string displayName = null)
        {
            ParentId = parentId;
            DisplayName = displayName;
        }

        /// <summary>
        /// Specifies which Views/Presenters should also be generated (these may be invoked - directly or indirectly - from
        /// the generated menu item, or otherwise).
        /// </summary>
        public GeneratedForms GeneratedForms { get; set; } = GeneratedForms.All;
    }

    #endregion

    /// <summary>
    /// Types of items that can be generated by the form generator.
    /// </summary>
    [Flags]
    public enum GeneratedForms
    {
        None = 0,
        Search = 1,
        Edit = 2,
        MenuItem = 4,

        All = Search | Edit
    }
}
