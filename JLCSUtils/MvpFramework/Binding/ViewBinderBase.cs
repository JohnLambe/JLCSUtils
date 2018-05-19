using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Base class for classes that bind a view to its model and presenter.
    /// Specific UI frameworks can subclass this.
    /// This class is independent of the UI framework.
    /// </summary>
    /// <typeparam name="TControl">The type of the UI framework's control/widget base class.</typeparam>
    public class ViewBinderBase<TControl> : Component, IOptionUpdate, IViewBinder
        where TControl : class
    {
        public ViewBinderBase(IMessageDialogService dialogService = null)
        {
            this.DialogService = dialogService;
        }

        /// <summary>
        /// Dialog service used for displaying validation errors.
        /// </summary>
        protected virtual IMessageDialogService DialogService { get; }

        /// <summary>
        /// Bind the model and presenter to the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        /// <param name="view"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory, TControl view)
        {
            View = view;
            ModelBinder = new ModelBinderWrapper(model, this);
            PresenterBinder = new PresenterBinderWrapper(presenter);

            //TODO: add event handlers to ModelBinder for validation notification ?
            // ModelBinder.ValidationStateChanged += ModelBinder_ValidationStateChanged;

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

            /*TODO
                    if (binderFactory != null)
                    {
                        Binders = new List<IControlBinder>();

                        //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                        BindControl(view, binderFactory, presenter);   // bind the root control recursively
                    }
            */
        }

        /// <summary>
        /// Set up the binding of a control, including its children.
        /// </summary>
        /// <param name="control">The control to bind.</param>
        /// <param name="binderFactory"></param>
        /// <param name="presenter"></param>
        protected virtual void BindControl(TControl control, IControlBinderFactory binderFactory, IPresenter presenter)
        {
            //TODO
            try
            {
                bool bindChildren = true;     // children are bound by default (so that controls on panels, etc. are scanned)
                var binder = binderFactory.Create(control);
                if (binder != null)
                {
                    Binders.Add(binder);
                    if (binder is IControlBinderV2)
                        (binder as IControlBinderV2).MvpBind(new MvpControlBindingContext(ModelBinder, PresenterBinder, this));
                    else
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
                    foreach (TControl childControl in ControlAdaptor.GetChildren(control))
                    {
                        BindControl(childControl, binderFactory, presenter);    // recurse
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MvpBindingException("Binding Control failed: " + ControlAdaptor.GetName(control), ex);
            }
        }

        public void RefreshView() => RefreshView(null);

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        public virtual void RefreshView(TControl control)
        {
            //TODO: Provide a way to refresh only properties on the View itself ?

            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    if (control == null || ControlAdaptor.IsInControl((TControl)(binder as IControlBinderExt)?.BoundControl, control))      // if refreshing all controls, or this one is in the requested one
//                        if (control == null || binder.IsInControl(control))      // if refreshing all controls, or this one is in the requested one
                        binder.MvpRefresh();
                }
            }

            if (control == null)        // if we refreshed all controls
                Invalidated = false;
        }

        /// <summary>
        /// The binder for the model.
        /// </summary>
        protected virtual ModelBinderWrapper ModelBinder { get; set; }

        /// <summary>
        /// The binder for the presenter.
        /// </summary>
        protected virtual PresenterBinderWrapperBase PresenterBinder { get; set; }

        #region IOptionUpdate

        /// <inheritdoc cref="IOptionUpdate.UpdateOption(OptionUpdateArgs)"/>
        public virtual void UpdateOption(OptionUpdateArgs args)
        {
            OptionUpdate?.Invoke(args);
        }

        protected event UpdateOptionDelegate OptionUpdate;

        #endregion

        /// <inheritdoc cref="ViewBinderBase{TControl}.ValidateModel"/>
        public virtual bool ValidateModel(object model = null)
        {
            return ModelBinder.Validate(model, DialogService);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <returns>true iff there were no validation errors.</returns>
        public virtual bool ValidateControls(TControl control = null)
        {
            bool valid = true;  // true iff all controls so far are valid
            if (Binders != null)
            {
                var binders = Binders.OfType<IValidateableControl>().OrderBy(b => b.ValidationOrder);  // get all validatable control binders

                // Highlight all invalid:
                foreach (var binder in binders)
                {
                    if (control == null || ControlAdaptor.IsInControl((TControl)(binder as IControlBinderExt)?.BoundControl, control))      // if refreshing all controls, or this one is in the requested one
                    {
                        bool currentValid = ((IValidateableControl)binder).Validate(ControlValidationOptions.Highlight);   // do this even if !valid
                        valid = valid && currentValid;
                    }
                }

                if (!valid)   // if any failed, set focus to or require entry of first invalid one
                {
                    foreach (var binder in binders)
                    {
                        if (control == null || ControlAdaptor.IsInControl((TControl)(binder as IControlBinderExt)?.BoundControl, control))      // if refreshing all controls, or this one is in the requested one
                        {
                            if (!((IValidateableControl)binder).Validate(ControlValidationOptions.Activate))
                                break;
                        }
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Fire a handler on the presenter.
        /// </summary>
        /// <param name="handlerId"></param>
        /// <param name="args"></param>
        // View base classes could have a method that delegates to this.
        public virtual void FireHandler(string handlerId, EventArgs args = null)
        {
            PresenterBinder.GetHandler(handlerId).Invoke(View, args ?? EventArgs.Empty);
        }

        /// <summary>
        /// Process a keystroke on the form. (Pass a keyboard event to relevant bound controls.)
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// Views call this to pass on keystrokes not processed by the more specific controls.
        /// </remarks>
        //TODO: Attach event handler in Bind(...) instead of views calling this explicitly?
        public virtual void ProcessKey(KeyboardKeyEventArgs args)
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
                        if (args.Cancel)
                            break;
                    }
                }
            }
        }

        #region Invalidation

        /// <summary>
        /// Cause the view to be refreshed, while not in a validation handler.
        /// </summary>
        public virtual void InvalidateView()
        {
            if (_ValidationStage == ValidationStage.NotValidating)
                RefreshView();
            else
                Invalidated = true;
        }
        /// <summary>
        /// true iff a refresh of the view is pending.
        /// </summary>
        protected bool Invalidated { get; set; }

        protected virtual void RefreshIfInvalidated()
        {
            if(Invalidated)
            {
                RefreshView();
                Invalidated = false;
            }
        }

        public virtual void NotifyValidationStage(ValidationStage stage)
        {
            if(stage == ValidationStage.NotValidating || stage == ValidationStage.AfterValidated
                || stage == ValidationStage.AfterValidating  // TEMPORARY
                )
            {
                _ValidationStage = ValidationStage.NotValidating;
                RefreshIfInvalidated();
            }
            else
            {
                _ValidationStage = stage;
            }
        }
        protected ValidationStage _ValidationStage;

        /*
        protected virtual void ModelBinder_ValidationStateChanged(object sender, BinderValidationState state)
        {
            if (state == BinderValidationState.Validated)
                RefreshIfInvalidated();
        }
        */

        #endregion

        /// <summary>
        /// Collection of binders for the controls in this view.
        /// </summary>
        protected virtual IList<IControlBinder> Binders { get; /*private*/ set; }

        /// <summary>
        /// The bound view.
        /// </summary>
        [NotNull]
        protected virtual TControl View { get; set; }

        protected virtual IControlAdaptor<TControl> ControlAdaptor { get; set; }
    }

}
