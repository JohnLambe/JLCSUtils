using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;

namespace MvpFramework
{
    /// <summary>
    /// Optional base class for Views.
    /// </summary>
    public class ViewBase : Form, IView
    {
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            if (binderFactory != null)
            {
                Binders = new List<IControlBinder>();
                var modelBinder = new ModelBinderWrapper(model);
                foreach (Control control in Controls)
                {
                    var binder = binderFactory.Create(control, presenter);
                    if (binder != null)
                    {
                        Binders.Add(binder);
                        binder.BindModel(modelBinder);
                    }
                }
            }
        }

        protected IList<IControlBinder> Binders { get; private set; }

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        void IView.Refresh()
        {
            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    binder.Refresh();
                }
            }
        }

        void IView.Show()
        {
            base.Show();
        }
    }
}
