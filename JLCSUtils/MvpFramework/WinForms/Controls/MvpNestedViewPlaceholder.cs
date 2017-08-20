using JohnLambe.Util.Misc;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// Control into which a nested View can be inserted.
    /// </summary>
    /// <seealso cref="INestedView"/>
    public class MvpNestedViewPlaceholder : Panel, INestedView, IControlBinder
    {
        /// <inheritdoc cref="INestedView.ViewId"/>
        [Description("The ID of the view to be placed in this control.")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual string ViewId { get; set; }

        #region IControlBinder

        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
        }

        public virtual void MvpRefresh()
        {
        }

        #endregion
    }

}
