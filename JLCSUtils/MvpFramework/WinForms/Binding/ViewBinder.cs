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

namespace MvpFramework.WinForms.Binding
{
    /// <summary>
    /// Binds a WinForms View to a Model and Presenter.
    /// <para>(<see cref="ViewBinderBase{TControl}"/> subclass for WinForms.)</para>
    /// </summary>
    public class ViewBinder : ViewBinderBase<Control>
    {
        /// <summary>
        /// Bind the model and presenter to the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        /// <param name="view"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory, Control view)
        {
            View = view;
            ModelBinder = new ModelBinderWrapper(model);
            PresenterBinder = new PresenterBinderWrapper(presenter);

            if (binderFactory != null)
            {
                Binders = new List<IControlBinder>();

                var viewControlBinder = ViewControlBinder.GetBinderForView(View);
                if (viewControlBinder != null)
                {
                    viewControlBinder.BindModel(ModelBinder, presenter);
                    //TODO: Don't bind if there was nothing to bind
                    Binders.Add(viewControlBinder);
                }

                //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                BindControl(view, binderFactory, presenter);   // bind the root control recursively
            }
        }

        /// <summary>
        /// Set up the binding of a control, including its children.
        /// </summary>
        /// <param name="control">The control to bind.</param>
        /// <param name="binderFactory"></param>
        /// <param name="presenter"></param>
        protected virtual void BindControl(Control control, IControlBinderFactory binderFactory, IPresenter presenter)
        {
            try
            {
                bool bindChildren = true;     // children are bound by default (so that controls on panels, etc. are scanned)
                var binder = binderFactory.Create(control);
                if (binder != null)
                {
                    Binders.Add(binder);
                    /*
                    if (binder is IEmbeddedView)
                    {
                        var viewId = ((IEmbeddedView)binder).ViewId;
                        var subModel = ReflectionUtils.TryGetPropertyValue<object>(binder,viewId);
                        IPresenter subPresenter = ReflectionUtils.TryGetPropertyValue<IPresenter>(presenter, viewId);
                        ((IEmbeddedView)binder).Bind(subModel, subPresenter, binderFactory);
                        //TODO
                    }
                    else */
                    {
                        binder.BindModel(ModelBinder, presenter);
                    }

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
                throw new MvpBindingException("Binding Control failed: " + control.Name,
                    ex);
            }
        }

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        public virtual void RefreshView(Control control)
        {
            //TODO: Provide a way to refresh only properties on the View itself ?

            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    if (control == null || binder.IsInControl(control))      // if refreshing all controls, or this one is in the requested one
                        binder.MvpRefresh();
                }
            }
        }

        /// <summary>
        /// Fire a handler on the presenter.
        /// </summary>
        /// <param name="handlerId"></param>
        /// <param name="args"></param>
        // View bases classes could have a method that delegates to this.
        public virtual void FireHandler(string handlerId, EventArgs args = null)
        {
            PresenterBinder.GetHandler(handlerId).Invoke(View, args ?? EventArgs.Empty);
        }

        /*
        public virtual MenuItemModel GetOption(string id)
        {

        }
        */

        /// <summary>
        /// Collection of binders for the controls in this view.
        /// </summary>
        protected virtual IList<IControlBinder> Binders { get; private set; }

        /// <summary>
        /// The binder for the model.
        /// </summary>
        protected virtual ModelBinderWrapper ModelBinder { get; set; }

        /// <summary>
        /// The binder for the presenter.
        /// </summary>
        protected virtual PresenterBinderWrapperBase PresenterBinder { get; set; }

        /// <summary>
        /// The bound view.
        /// </summary>
        protected virtual Control View { get; set; }
    }

}
