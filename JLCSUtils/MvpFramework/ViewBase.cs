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
                var modelBinder = new ModelBinderWrapper(model);
                foreach (Control control in Controls)
                {
                    binderFactory.Create(control, presenter)?.BindModel(modelBinder);
                }
            }
        }

        void IView.Show()
        {
            base.Show();
        }
    }
}
