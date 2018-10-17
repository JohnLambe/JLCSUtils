using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework;
using MvpFramework.Dialog;

namespace MvpFrameworkTest
{
    //TODO: Move to a new assembly ?

    public class MockDialogService : MessageDialogService
    {
        public MockDialogService() : base(new MockDialogServicePresenterFacotry())
        {
        }

        public override TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel)
        {
            var result = base.ShowMessage(messageModel);

            _dialogsShown.Add(messageModel);

            return result;
        }

        public ICollection<IMessageDialogModel> DialogsShown => _dialogsShown;
        protected List<IMessageDialogModel> _dialogsShown;
    }

    public class MockDialogServicePresenterFacotry : IPresenterFactory<IMessageDialogPresenter>
    {
        public virtual IMessageDialogPresenter Create()
        {
            return new MockDialogServicePresenter();
        }
    }

    public class MockDialogServicePresenter : IMessageDialogPresenter
    {
        public virtual void Close()
        {
        }

        public virtual object Show()
        {
            return null;
        }

        public virtual object ShowDialog(IMessageDialogModel messageDialog)
        {
            var result = messageDialog?.Options?.Default?.Id;
            if(result == null)
            {
                // return the first item in DefaultOptions that exists as an option
                foreach (var id in DefaultOptions)
                {
                    if (messageDialog?.Options?.Children.Where(o => o.Id == id).Any() ?? false)
                    {
                        return id;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// If the dialog has no default option, the first of these that matches an option in the dialog is returned.
        /// </summary>
        protected static readonly string[] DefaultOptions = new[] { MessageDialogResponse.Ok, MessageDialogResponse.Yes, MessageDialogResponse.YesAll };

        public virtual object ShowModal()
        {
            return null;
        }
    }
}
