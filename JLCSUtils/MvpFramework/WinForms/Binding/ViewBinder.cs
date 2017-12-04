using MvpFramework.Binding;
using MvpFramework.WinForms.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MvpFramework.WinForms.Binding;
using JohnLambe.Util.Reflection;
using JohnLambe.Util;
using JohnLambe.Util.Collections;
using MvpFramework.Menu;
using MvpFramework.Dialog;
using JohnLambe.Util.Types;

namespace MvpFramework.WinForms.Binding
{
    /// <summary>
    /// Binds a WinForms View to a Model and Presenter.
    /// <para>(<see cref="ViewBinderBase{TControl}"/> subclass for WinForms.)</para>
    /// </summary>
    public class ViewBinder : ViewBinderBase<Control>
    {
        public ViewBinder(IMessageDialogService dialogService = null) : base(dialogService)
        {
            ControlAdaptor = new WinFormsControlAdaptor();
        }

        /// <summary>
        /// Bind the model and presenter to the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        /// <param name="view">The view to be bound by this binder.</param>
        public override void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory, Control view)
        {
            if (binderFactory != null)
            {
                view.SuspendLayout();
                try
                {
                    base.Bind(model, presenter, binderFactory, view);
                }
                finally
                {
                    view.ResumeLayout();
                }
            }
        }

        /* moved to base class:
        /// <summary>
        /// Set up the binding of a control, including its children.
        /// </summary>
        /// <param name="control">The control to bind.</param>
        /// <param name="binderFactory"></param>
        /// <param name="presenter"></param>
        protected override void BindControl(Control control, IControlBinderFactory binderFactory, IPresenter presenter)
        {
            try
            {
                bool bindChildren = true;     // children are bound by default (so that controls on panels, etc. are scanned)
                var binder = binderFactory.Create(control);
                if (binder != null)
                {
                    Binders.Add(binder);
                    binder.BindModel(ModelBinder, presenter);

                    bindChildren = control.GetType().GetCustomAttribute<MvpBindChildrenAttribute>()?.Enabled ?? false;
                    // By default, controls that implement IControlBinder do not have their children bound (because the children are probably used by the control and bound by it if necessary),
                    // but this can be overridden with the attribute.
                }

                // if this control implements the interface, add its handler to the event:
                if (control is IOptionUpdate && control != View)
                    OptionUpdate += args => ((IOptionUpdate)control).UpdateOption(args);

                if (bindChildren)
                {   // bind the children of this control:
                    foreach (Control childControl in control.Controls)
                    {
                        BindControl(childControl, binderFactory, presenter);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new MvpBindingException("Binding Control failed: " + control.Name, ex);
            }
        }
        */

        public override void RefreshView(Control control = null)
        {
            View.SuspendLayout();
            try
            {
                //TODO: Move to base class:
                if (Binders != null)
                {
                    foreach (var binder in Binders)
                    {
                        if (control == null || binder.IsInControl(control))      // if refreshing all controls, or this one is in the requested one
                            binder.MvpRefresh();
                    }
                }
                base.RefreshView(control);
            }
            finally
            {
                View.ResumeLayout();
            }
        }

        /*
        public virtual MenuItemModel GetOption(string id)
        {
        }
        */

        /* Moved to base class:
    public override void ProcessKey(KeyboardKeyEventArgs args)
    {
        if (Binders != null)
        {
            foreach (var binder in Binders)
            {
                if (args.Cancel)            // if cancelled (by previous handler, or before entering this method)
                    return;                 // don't process remaining handlers

                if (binder is IKeyboardKeyHandler)
                {
                    ((IKeyboardKeyHandler)binder).NotifyKeyDown(args);
                }
            }
        }
    }
        */

    }

    /// <summary>
    /// Implementation of <see cref="IControlAdaptor{TControl}"/> for WinForms 
    /// (to enable the framework to support WinForms).
    /// </summary>
    public class WinFormsControlAdaptor : IControlAdaptor<Control>
    {
        public IEnumerable<Control> GetChildren(Control control)
        {
            return control.Controls.Cast<Control>();
        }

        public string GetName(Control control)
        {
            return control.Name;
        }

        public bool IsInControl(Control control, Control testParent)
        {
            return control.IsInControl(testParent);
        }
    }

}
