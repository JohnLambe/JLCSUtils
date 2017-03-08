using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Binding;
using MvpFramework.Dialog;

namespace MvpFramework.WinForms
{
    [View(Interface = typeof(IMessageDialogView))]
    public partial class MessageDialogView : WindowViewBase, IMessageDialogView
    {
        public MessageDialogView()
        {
            InitializeComponent();
        }

        public override object ShowModal()
        {
            RefreshView();     // refresh because the model will usually be updated before calling this
            return base.ShowModal();
        }

        public override void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            base.Bind(model, presenter, binderFactory);
            //Text = ((MessageDialogViewModel)model).Dialog?.Title;
        }

    }
}
