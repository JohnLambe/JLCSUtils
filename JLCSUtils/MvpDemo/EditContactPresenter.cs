using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework;
using MvpDemo.Model;
using JohnLambe.Util;
using MvpFramework.Binding;

namespace MvpDemo
{
    public interface IEditContactPresenter : IPresenter
    {

    }


    public class EditContactPresenter : PresenterBase<IEditContactView, Contact>, IEditContactPresenter
    {
        public EditContactPresenter(IEditContactView view, Contact model = null, IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }

        /*
        protected override void Bind(IEditContactView view)
        {
            base.Bind(view);
            //            view.OnOkClick += HandleOkClick;
        }
        */

        [MvpHandler(Name = "Ok")]
        public void HandleOkClick()
        {
            System.Windows.Forms.MessageBox.Show("Ok");
        }
    }

}
