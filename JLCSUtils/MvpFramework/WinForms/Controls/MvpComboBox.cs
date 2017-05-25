using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// <see cref="ComboBox"/> that can be bound automatically by the MVP framework.
    /// </summary>
    [MvpBoundControl]
//    [MvpControlMapping(ForType = typeof(Enum))]  //TODO: Implement automatic mapping for Enums
    public class MvpComboBox : ComboBox
    {
        /// <summary>
        /// The name of the bound property on the model.
        /// </summary>
        [MvpModelProperty("Value", "ValueChanged")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        public virtual string ModelProperty { get; set; }
    }
}
