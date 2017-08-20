using MvpFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpDemo.Model;
using MvpFramework.Binding;
using DiExtension.Attributes;

namespace MvpDemo.Heirarchical
{
    public interface ITestLayoutPresenter : IPresenter
    {
        IEditContactPresenter Contact { get; }
    }

    [Presenter]
    public class TestLayoutPresenter : WindowPresenterBase<ITestLayoutView, TestHViewModel>, ITestLayoutPresenter
    {
        public TestLayoutPresenter(ITestLayoutView view,
            [MvpParam] TestHViewModel model,
            [Inject] IControlBinderFactory binderFactory,
            [MvpNested("Contact")] IPresenterFactory<IEditContactPresenter, Contact> editContactFactory,
            [Inject] IUiController uiController) : base(view, model, binderFactory)
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

        /// <summary>
        /// Nested presenter.
        /// </summary>
        public IEditContactPresenter Contact { get; protected set; }
    }

}
