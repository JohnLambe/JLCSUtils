using MvpFramework.Binding;
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
}
