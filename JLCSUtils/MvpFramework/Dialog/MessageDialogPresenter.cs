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

        public virtual object ShowDialog(IMessageDialogModel messageDialog)
        {
            if (messageDialog == null)
                return null;

            Model.Dialog = messageDialog;

            if (Model.Dialog.Options == null)
                Model.Dialog.Options = Model.Dialog.MessageType?.DefaultOptions;
            if (Model.Dialog.Icon == null)
                Model.Dialog.Icon = Model.Dialog.MessageType?.Icon;

            return View.ShowModal();
        }

    }

    /// <summary>
    /// The model of <see cref="IMessageDialogView"/>.
    /// </summary>
    public class MessageDialogViewModel
    {
        /// <summary>
        /// Model of the current dialog.
        /// </summary>
        public virtual IMessageDialogModel Dialog { get; set; }

        /// <summary>
        /// Window title (for the current dialog).
        /// </summary>
        public virtual string Title => Dialog?.Title;
    }
}
