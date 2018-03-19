using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;
using static System.Windows.Forms.Control;
using JohnLambe.Util.Reflection;
using MvpFramework.WinForms.Binding;
using JohnLambe.Util;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.ComponentModel.DataAnnotations;
using JohnLambe.Util.Types;
using MvpFramework.Dialog;
using DiExtension.Attributes;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Optional base class for Views.
    /// </summary>
    public class ViewBase : UserControl, IView, IOptionUpdate, IContainerView, INotifyOnDispose, INestableView,
        IValidatableView
    {
        /// <summary>
        /// Initialise with no Message Dialog Service.
        /// </summary>
        public ViewBase()
        {
        }

        public ViewBase(IMessageDialogService dialogService)  //TODO: Change parameter to IMvpFramework ?
        {
            this.DialogService = dialogService;
        }

        public ViewBase(IMvpFrameworkDetails mvpFramework)
        {
            this.DialogService = mvpFramework.MessageDialogService;
        }

        private IMessageDialogService DialogService { get; set; }  //TODO: Make 'protected' ? Change to IMvpFramework ?

        protected virtual void SetDialogService(IMessageDialogService dialogService)
        {
            this.DialogService = dialogService;
        }

        #region Binding

        /// <summary>
        /// Bind the model and presenter to this view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            ViewBinder = new ViewBinder(DialogService);
            ViewBinder.Bind(model, presenter, binderFactory, this);
        }

        /// <inheritdoc cref="IView.RefreshView"/>
        // Not virtual because the other overload should be overridden instead.
        public void RefreshView()
        {
            RefreshView(null);
        }

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        protected virtual void RefreshView([Nullable] Control control)
        {
            ViewBinder.RefreshView(control);
        }

        /// <summary>
        /// Binds this View to the Model and Presenter.
        /// </summary>
        protected virtual ViewBinder ViewBinder { get; private set; }

        #region INestableView

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // This is used only at runtime.
        // It must not appear in the WinForms generated code because it would conflict with the Parent property.
        // In Visual Studio 2015, it would cause the designer to crash.
        public virtual object ViewParent
        {
            get { return Parent; }
            set { Parent = (Control)value; }
        }

        #endregion

        #region IOptionUpdate

        public virtual void UpdateOption(OptionUpdateArgs args)
        {
            ViewBinder.UpdateOption(args);
        }

        #endregion

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ViewBase
            // 
            this.Name = "ViewBase";
            //            this.Size = new System.Drawing.Size(500, 300);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Find a nested view or a parent control to receive a nessted view, within this View.
        /// </summary>
        /// <param name="nestedViewId">ID of the view to locate.</param>
        /// <param name="viewParent">The control into which the specifed view should be placed, or null (if not found).</param>
        /// <returns>The nested (embedded) view, or null (if not found or not an embedded view).</returns>
        /// <seealso cref="INestedView"/>
        /// <exception cref="AmbiguousMatchException">If there is more than one view/placeholder with the given ID.</exception> //TODO: Exception type
        public virtual IView GetNestedView(string nestedViewId, out INestedViewPlaceholder viewParent)
        {
            return GetNestedView(this, nestedViewId, out viewParent);
        }

        protected virtual IView GetNestedView(Control rootControl, string nestedViewId, out INestedViewPlaceholder viewParent)
        {
            foreach (var control in rootControl.Controls)
            {
                if (control is INestedView)
                {
                    if (nestedViewId.Equals(((INestedView)control).ViewId))
                    {
                        if (control is IEmbeddedView)
                        {
                            viewParent = null;
                            return (IView)control;
                        }
                        else if (control is INestedViewPlaceholder)
                        {
                            viewParent = control as INestedViewPlaceholder;
                            return null;
                        }
                        else
                        {
                            // anything else implementing INestedView but not one of the above cannot be used in this context.
                            throw new MvpResolutionException("Nested view type not supported");  //TODO Define more specific exception
                        }
                    }
                    // (if it doesn't match,) we don't search within INestedView
                }
                else if (!(control is IControlBinder) && ((Control)control).HasChildren)   // don't search within IControlBinder
                {   
                    var result = GetNestedView((Control)control, nestedViewId, out viewParent);   // recurse
                    if (result != null || viewParent != null)                   // if found
                        return result;
                }
            }

            // not found:
            viewParent = null;
            return null;
        }


        #region ParentForm

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void OnShown(EventArgs e)
        {
            Shown?.Invoke(this,e);
        }

        private void ParentForm_Shown(object sender, EventArgs e)
        {
            OnShown(e);
        }

        //
        // Summary:
        //     Occurs whenever the form is first displayed.
        //[SRCategoryAttribute("CatBehavior")]
        //[IODescriptionAttribute("FormOnShownDescr")]
        public event EventHandler Shown;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnParentChanged(EventArgs e)
        {
            if(_shownEventForm != null)
            {
                _shownEventForm.Shown -= ParentForm_Shown;
                _shownEventForm = null;
            }
            if (ParentForm != null)
            {
                _shownEventForm = ParentForm;
                _shownEventForm.Shown += ParentForm_Shown;
            }
        }
        private Form _shownEventForm;

        #endregion

        /// <inheritdoc cref="ViewBinderBase{TControl}.ValidateModel"/>
        public virtual bool ValidateModel(object model = null)
        {
            return ViewBinder.ValidateModel(model);
        }

        public virtual bool ValidateControls()
        {
            return ViewBinder.ValidateControls();
        }
    }

}
