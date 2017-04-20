using MvpFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpDemo.Model;
using MvpFramework.Binding;

namespace MvpDemo.Heirarchical
{
    public interface ITestHPresenter : IPresenter
    {
        IEditContactPresenter Contact { get; }
    }

    [Presenter]
    public class TestHPresenter : PresenterBase<ITestHView, TestHViewModel>, ITestHPresenter
    {
        public TestHPresenter(ITestHView view,
            [MvpParam] TestHViewModel model,
            IControlBinderFactory binderFactory,
            [MvpNested] IPresenterFactory<IEditContactPresenter,Contact> editContactFactory,
            IUiController uiController) : base(view, model, binderFactory)
        {
            this.UiController = uiController;

            Contact = editContactFactory.Create(model.Contact);
        }

        [MvpHandler(Id = "Ok")]
        public void HandleOkClick()
        {
//            UiController.ShowMessage("Ok\nName: " + Model.Name);
        }

        protected readonly IUiController UiController;

        public IEditContactPresenter Contact
        {
            get;
            protected set;
        }
    }

}
