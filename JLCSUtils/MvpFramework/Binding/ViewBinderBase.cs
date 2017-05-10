using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class ViewBinderBase<TControl> : Component, IOptionUpdate
    {
        /*        
                /// <summary>
                /// Bind the model and presenter to the view.
                /// </summary>
                /// <param name="model"></param>
                /// <param name="presenter"></param>
                /// <param name="binderFactory"></param>
                public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory, TControl view)
                {
                    View = view;
                    ModelBinder = new ModelBinderWrapper(model);
                    PresenterBinder = new PresenterBinderWrapper(presenter);

                    if (binderFactory != null)
                    {
                        Binders = new List<IControlBinder>();

                        //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                        BindControl(view, binderFactory, presenter);   // bind the root control recursively
                    }
                }

                protected virtual void BindControl(TControl control, IControlBinderFactory binderFactory, IPresenter presenter)
                {
                    var binder = binderFactory.Create(control);
                    if (binder != null)
                    {
                        Binders.Add(binder);
                        binder.BindModel(ModelBinder, presenter);
                    }

                    var controls = GetChildren(control);
                    foreach (TControl childControl in controls)
                    {
                        BindControl(childControl, binderFactory, presenter);
                    }
                }

                /// <summary>
                /// Refresh the view, or a specified control on it, from the model.
                /// </summary>
                /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
                public virtual void RefreshView(TControl control)
                {
                    var viewTitle = ModelBinder.Title;
                    if (viewTitle != null)
                        View.Text = viewTitle;

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
                /// <param name="param"></param>
                // View bases classes could have a method that delegates to this.
                public virtual void FireHandler(string handlerId, EventArgs args = null)
                {
                    PresenterBinder.GetHandler(handlerId).Invoke(View, args ?? EventArgs.Empty);
                }

                /// <summary>
                /// Return the collection of children of the given control.
                /// </summary>
                /// <param name="control"></param>
                /// <returns></returns>
                protected virtual IEnumerable<TControl> GetChildren(TControl control)
                {
                    return JohnLambe.Util.Collections.EmptyCollection<TControl>.EmptyArray;
                }

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
                protected virtual TControl View { get; set; }
                */

        public virtual void UpdateOption(OptionUpdateArgs args)
        {
            OptionUpdate?.Invoke(args);
        }

        protected event UpdateOptionDelegate OptionUpdate;
    }

    /*
        public interface IFrameworkAdaptor<TControl, TView>
        {
            IEnumerable<TControl> GetChildren(TControl control);

            bool IsInControl(TControl control, TControl testParent);

            void SetViewTitle(TView view, string title);
        }
        */

}
