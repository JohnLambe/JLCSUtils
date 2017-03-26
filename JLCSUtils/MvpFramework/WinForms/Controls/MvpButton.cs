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
    /// <see cref="Button"/> that can be bound automatically by the MVP framework.
    /// </summary>
    [MvpBoundControl]
    public class MvpButton : Button
    {
        [MvpHandlerIdProperty("Click")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.HandlerIdDescription)]
        public virtual string HandlerId { get; set; }
    }
}
