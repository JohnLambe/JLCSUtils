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
    /// <see cref="CheckBox"/> that can be bound automatically by the MVP framework.
    /// </summary>
    [DefaultEvent("CheckStateChanged")]  // Fired on setting the state of tri-state check boxes to Indeterminate, in addition to when CheckedChanged is fired.
    [MvpBoundControl]
    [MvpControlMapping(ForTypes = new [] { typeof(bool), typeof(bool?) })]
    public class MvpCheckBox : CheckBox
    {
        /*
        /// <summary>
        /// The name of the bound property on the model.
        /// The recommended types for the bound property are <see cref="bool"/> and bool? (if the check box is tri-state).
        /// </summary>
        [MvpModelProperty("CheckState", "CheckStateChanged")]
        //TODO?:        [MvpModelProperty("CheckState;Checked", "CheckStateChanged")]  // specify secondary properties to be used if the binding the previous one(s) fails.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        public virtual string ModelProperty { get; set; }
        */

#pragma warning disable CS0067   // Suppress 'Event never used'.  This is fired by reflection.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.ModelPropertyNameDescription)]
        [MvpModelProperty("CheckState", "CheckStateChanged")]
        public event GetNameDelegate OnGetModelProperty;
#pragma warning restore CS0067

        /*
        [MvpHandlerIdProperty("CheckStateChanged")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual string TextChangedHandlerId { get; set; }
        */
    }
}
