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
    /// <para>
    /// Hierarchical MVP:
    /// A layout view would have instances of this for each nested view.
    /// When the presenter for each nested view are being created, its view is placed within this control.
    /// </para>
    /// </summary>
    /// <example>
    /// See <see cref="MvpNestedAttribute"/>.
    /// </example>
    /// <seealso cref="MvpNestedAttribute"/>
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
