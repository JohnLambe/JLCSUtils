using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.Attributes;
using MvpFramework.Binding;

namespace MvpFramework.Dialog
{
    [Presenter(Interface = typeof(IMessageDialogPresenter))]
    public class MessageDialogPresenter : WindowPresenterBase<IMessageDialogView, MessageDialogViewModel>, IMessageDialogPresenter
    {
        public MessageDialogPresenter(IMessageDialogView view,
            [Inject] IControlBinderFactory binderFactory = null)
            : base(view, new MessageDialogViewModel(), binderFactory)
        {
            DisposeOnClose = false;    // this presenter is intended to be shown multiple times
        }

        public virtual object ShowModal(IMessageDialogModel messageDialog)
        {
            Model.Dialog = messageDialog;
            return View.ShowModal();
        }

    }

    public class MessageDialogViewModel
    {
        public virtual IMessageDialogModel Dialog { get; set; }
    }
}
