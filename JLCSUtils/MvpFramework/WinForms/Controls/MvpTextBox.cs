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
    /// <see cref="TextBox"/> that can be bound automatically by the MVP framework.
    /// </summary>
    [MvpBoundControl]
    public class MvpTextBox : TextBox
    {
        /// <summary>
        /// The name of the bound property on the model.
        /// </summary>
        [MvpModelProperty("Text", "TextChanged")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        public virtual string ModelProperty { get; set; }

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        //        [MvpModelProperty("Text", "TextChanged")]
        public event GetNameDelegate GetModelProperty;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        public string[] Items { get; set; }

        [Category(MvpUiComponentConsts.DesignerCategory)]
        public Type TypeProperty { get; set; }
    }

    public delegate string GetNameDelegate();
}
