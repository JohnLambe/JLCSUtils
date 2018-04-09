using JohnLambe.Util.Misc;
using JohnLambe.Util.Types;
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
    /// <para>
    /// This class implements <see cref="IControlBinder"/>, to prevent the framework trying to bind the contents of this control
    /// (the placed view) to the model of its containing view.
    /// </para>
    /// </summary>
    /// <example>
    /// See <see cref="MvpNestedAttribute"/>.
    /// </example>
    /// <seealso cref="MvpNestedAttribute"/>
    /// <seealso cref="INestedView"/>
    public class MvpNestedViewPlaceholder : Panel, INestedViewPlaceholder, IControlBinder
    {
        #region INestedViewPlaceholder

        /// <inheritdoc cref="INestedView.ViewId"/>
        [Description("The ID of the view to be placed in this control.")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [DefaultValue(null)]
        public virtual string ViewId { get; set; }

        /// <summary>
        /// The view nested in this one.
        /// </summary>
        [Nullable("If there is no nested view.")]
        [DefaultValue(null)]
        public virtual INestableView NestedView
        {
            get { return _nestedView; }
            set
            {
                if (value != _nestedView)
                    NestedViewChanged.Invoke(this, EventArgs.Empty);

                //if((NestedView as Control)?.Parent == this)
                //    ((Control)NestedView).Parent = null;
                if (NestedView != null)
                {   // remove any existing nested view:
                    NestedView.ViewParent = null;
                    NestedView = null;
                }

                if(value != null)
                    value.ViewParent = this;
                if (value is Control)
                {
                    //((Control)nestedView).Parent = this;
                    ((Control)value).Dock = NestedDock;
                }
                _nestedView = value;
            }
        }
        protected INestableView _nestedView;

        /// <summary>
        /// The nested view control.
        /// null if the nested view is not a <see cref="Control"/>.
        /// </summary>
        [Nullable]
        public virtual Control ViewControl => NestedView as Control;

        /// <summary>
        /// When when <see cref="NestedView"/> changes.
        /// </summary>
        public virtual event EventHandler NestedViewChanged;

        #endregion

        #region IControlBinder

        /// <summary>
        /// Does nothing.
        /// Binding the outer (containing) view does not bind the nested (contained) one.
        /// The contained view must be bound separately in the usual way.
        /// </summary>
        /// <param name="modelBinder"></param>
        /// <param name="presenter"></param>
        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
        }

        /// <summary>
        /// Refreshes the contained view iff <see cref="CascadeRefresh"/> is true.
        /// </summary>
        public virtual void MvpRefresh()
        {
            if(CascadeRefresh)
                NestedView?.RefreshView();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
                e.Graphics.DrawString(ViewId,Font,Brushes.Green, 12, 12);
        }

        [Category("Layout")]
        [Description("How the nested view is docked within this control.")]
        [DefaultValue(DockStyle.Fill)]
        public virtual DockStyle NestedDock { get; set; } = DockStyle.Fill;

        // AutoSize

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("true to make the contained view refresh when the containing one is refreshed.")]
        [DefaultValue(true)]
        public virtual bool CascadeRefresh { get; set; } = true;
    }

}
