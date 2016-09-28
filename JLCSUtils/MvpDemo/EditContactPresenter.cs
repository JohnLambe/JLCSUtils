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
        public EditContactPresenter(IEditContactView view, Contact model, IControlBinderFactory binderFactory,
            IUiController uiController) : base(view, model, binderFactory)
        {
            this.UiController = uiController;
        }

        [MvpHandler(Name = "Ok")]
        public void HandleOkClick()
        {
            UiController.ShowMessage("Ok\nName: " + Model.Name);
//            System.Windows.Forms.MessageBox.Show("Ok\nName: " + Model.Name);  // should use injected interface to access 
        }

        protected readonly IUiController UiController;
    }

}
