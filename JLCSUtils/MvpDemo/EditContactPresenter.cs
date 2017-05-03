using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework;
using MvpDemo.Model;
using JohnLambe.Util;
using MvpFramework.Binding;
using MvpFramework.Dialog.Dialogs;

namespace MvpDemo
{
//    [PresenterForAction(typeof(Contact),typeof(IEditPresenter))]
    public interface IEditContactPresenter : IWindowPresenter
    {
    }


    [Presenter]
    public class EditContactPresenter : WindowPresenterBase<IEditContactView, Contact>, IEditContactPresenter
    {
        public EditContactPresenter(IEditContactView view, 
            [MvpParam] Contact model,
            IControlBinderFactory binderFactory,
            IUiController uiController) : base(view, model, binderFactory)
        {
            this.UiController = uiController;
        }

        [MvpHandler(Id = "Ok")]
        public void HandleOkClick()
        {
            UiController.ShowMessage("Ok\nName: " + Model.Name + "\nAddress: " + Model.Address);
            // Equivalent to: System.Windows.Forms.MessageBox.Show("Ok\nName: " + Model.Name);
        }

        [MvpHandler(Id = "Update")]
        public void HandleUpdateClick()
        {
            Model.Address = Model.Address + "(modified)";
            View.RefreshView();
        }

        [MvpHandler(Filter = new string[] { "RightPanel" })]
        public void Command1()
        {
            UiController.ShowMessage("Command1", "Dialog Title");
        }

        [MvpHandler(SingleFilter = "RightPanel")]
        // same as  Filter = new string[] { "RightPanel" }
        public void Command2()
        {
            UiController.ShowMessage(
            new ErrorDialog()
            {
                Message = "Error message"
            });
        }

        [MvpHandler(Filter = new string[] { "RightPanel" })]
        public void ConfirmationDialog()
        {
            UiController.ShowMessage(
            new ConfirmationDialog()
            {
                Title = "Confirm",
                Message = "Confirmation message\nLine 2"
            });
        }

        [MvpHandler("Test")]
        public void TestEventHandler()
        {
            UiController.ShowMessage(
            new ConfirmationDialog()
            {
                Title = "TestEventHandler",
                Message = "TestEventHandler"
            });
        }

        protected readonly IUiController UiController;
    }

}
