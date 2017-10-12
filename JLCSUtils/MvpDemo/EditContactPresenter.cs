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

        [MvpHandler(Filter = new [] { "RightPanel" }, HotKey = KeyboardKey.F2)]
        public void Command1()
        {
            UiController.ShowMessage("Command1", "Dialog Title");
        }

        [MvpHandler(SingleFilter = "RightPanel", HotKey = KeyboardKey.Q | KeyboardKey.Control)]
        // same as  Filter = new string[] { "RightPanel" }
        public void Command2()
        {
            UiController.ShowMessage(
            new ErrorDialog()
            {
                Message = "Error message"
            });
        }

        [MvpHandler(SingleFilter = "RightPanel")]
        public void DisableAll()
        {
            // Disable all buttons in the panel:
            (View as IOptionUpdate).UpdateOption(new OptionUpdateArgs() { Filter = "RightPanel", OnUpdate = args => args.Option.Enabled = false });
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

        [MvpHandler("Test2Event")]
        public void Test2EventHandler(object sender, EventArgs e)
        {
            UiController.ShowMessage(
            new ConfirmationDialog()
            {
                Title = "Test2EventHandler",
                Message = e.ToString()
            });
        }

        protected readonly IUiController UiController;
    }

}
