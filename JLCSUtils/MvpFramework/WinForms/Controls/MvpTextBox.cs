using JohnLambe.Util.Misc;
using MvpFramework.Binding;
using MvpFramework.Generator;
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
    [MvpControlMapping(ForType = typeof(string))]
    public class MvpTextBox : TextBox
    {
        /*
        /// <summary>
        /// The name of the bound property on the model.
        /// </summary>
        [MvpModelProperty("Text", "TextChanged")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        public virtual string ModelProperty { get; set; }
        {
            get { return GetModelProperty?.Invoke(); }
//            set;
        }
        */

#pragma warning disable CS0067   // Suppress 'Event never used'.  This is fired by reflection.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        [MvpModelProperty(nameof(Text), nameof(TextChanged))]
        public event GetNameDelegate OnGetModelProperty;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyDelegateDescription)]
        [MvpModelProperty(nameof(Text), nameof(TextChanged))]
        public event GetPropertyEventHandler OnGetModelPropertyDelegate;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [MvpDefineValidatingEvent]
        public event ValueChangedEventHandler ValidatingExt;  //| Or OnValidatingExt 

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [MvpDefineValidatedEvent]
        public event ValueChangedEventHandler ValidatedExt;
#pragma warning restore CS0067

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [DefaultValue(null)]
        public string[] Items { get; set; }

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [DefaultValue(null)]
        public Type TypeProperty { get; set; }
    }

}
