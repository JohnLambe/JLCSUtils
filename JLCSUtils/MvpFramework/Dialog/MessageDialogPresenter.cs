using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.Attributes;
using MvpFramework.Binding;

namespace MvpFramework.Dialog
{
    public class MessageDialogPresenter : PresenterBase<IMessageDialogView, MessageDialogViewModel>, IMessageDialogPresenter
    {
        public MessageDialogPresenter(IMessageDialogView view,
            [Inject] IControlBinderFactory binderFactory = null)
            : base(view, new MessageDialogViewModel(), binderFactory)
        {
        }

        public virtual object Show(IMessageDialogModel messageDialog)
        {
            Model.Dialog = messageDialog;
            /*return */ View.Show(); return null;   //TODO
        }
    }

    public class MessageDialogViewModel
    {
        public virtual IMessageDialogModel Dialog { get; set; }
    }
}
