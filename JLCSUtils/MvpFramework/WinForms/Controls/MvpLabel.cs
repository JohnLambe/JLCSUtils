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
    /// <see cref="Label"/> that can be bound automatically by the MVP framework 
    /// (the label text can be populated from the model).
    /// </summary>
    [MvpBoundControl]
    public class MvpLabel : Label
    {
        /*
        /// <summary>
        /// The name of the bound property on the model.
        /// The label text is populated from the model, but the model is not updated on changing the label text.
        /// </summary>
        [MvpModelProperty("Text")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        public virtual string ModelProperty { get; set; }
        */

#pragma warning disable CS0067   // Suppress 'Event never used'.  This is fired by reflection.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        [MvpModelProperty("Text")]
        public event GetNameDelegate OnGetModelProperty;
#pragma warning restore CS0067
    }
}
