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
using JohnLambe.Util.Misc;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// View for a message dialog.
    /// <para>A presenter for this is <see cref="MvpFramework.Dialog.MessageDialogPresenter"/>.</para>
    /// </summary>
    [View(Interface = typeof(IMessageDialogView))]   // References its interface explicitly, so that it is validated at compile time.
    public partial class MessageDialogView : WindowViewBase, IMessageDialogView
    {
        public MessageDialogView(IIconRepository<string,object> iconRepository)
        {
            InitializeComponent();

            ExpandedHeight = Height;
            NormalHeight = ExpandedHeight - uiFullDetails.Height;
            Height = NormalHeight;

            _iconRepository = iconRepository ?? new NullIconRepository<string, object>();
            DefaultButtonsBackColor = uiButtons.BackColor;           // the default color is that specified in the form designer
        }

        protected int ExpandedHeight { get; }
        protected int NormalHeight { get; }

        public override void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            base.Bind(model, presenter, binderFactory);
        }

        public override object ShowModal()
        {
            RefreshView();     // refresh because the model will usually be updated before calling this
            return base.ShowModal();
        }

        protected override void RefreshView(Control control)
        {
            base.RefreshView(control);
            if(control == null)
                Populate();
        }

        protected virtual void Populate()
        {
            if(Model.Dialog != null)
            {
                Title = Model.Title;

                uiButtons.Buttons = Model.Dialog.Options;    // populate the buttons

                // set the color of the button panel:
                if(Model.Dialog.MessageType.DefaultColor != Color.Empty)
                    uiButtons.BackColor = Model.Dialog.MessageType.DefaultColorMuted;
                else
                    uiButtons.BackColor = DefaultButtonsBackColor;

                // set the icons:
                uiMainIcon.BackgroundImage = (_iconRepository.GetIcon(Model.Dialog.Icon ?? Model.Dialog.MessageType.Icon) as Image);
                uiIcon2.BackgroundImage = _iconRepository.GetIcon(Model.Dialog.MessageImage) as Image;
                uiIcon2.Visible = uiIcon2.BackgroundImage != null;         // hide the control if there is no image in it

                uiMessageText.Text = Model.Dialog?.Message;

                uiFullDetails.Text = (Model.DetailMessage ?? "")
                    + "\n\n"
                    + Model.Dialog?.Exception?.ToString();

                uiDetails.Visible = Model.DetailMessage != null;

            }
        }

        protected virtual void uiButtons_ButtonClicked(object sender, ButtonContainer.ButtonClickedEventArgs args)
        {
            ModalResult = args.Button.ReturnValue ?? args.Button.Id;
            Close();
        }

        /// <summary>
        /// The repository for icons used in the dialog.
        /// Must not be null.
        /// </summary>
        protected readonly IIconRepository<string, object> _iconRepository;
        /// <summary>
        /// Color of the button container if none is specified in the message.
        /// </summary>
        protected readonly Color DefaultButtonsBackColor;

        [MvpBind(MvpBindAttribute.Model)]
        public virtual MessageDialogViewModel Model { get; set; }

        private void uiDetails_Click(object sender, EventArgs e)
        {
            bool expanded = !uiFullDetails.Visible;
            int expandHeight = uiFullDetails.Height;

            uiFullDetails.Visible = expanded;
            if (expanded)
                Height += expandHeight;
            else
                Height -= expandHeight;
//            Height = expanded ? ExpandedHeight : NormalHeight;
        }

        private void MessageDialogView_Resize(object sender, EventArgs e)
        {
            uiDetails.Top = uiButtons.Top + 12;
        }
    }

    // Instead of (or as well as a color), we could have an icon to the right, or a small icon in the Button Container 
    // to indicate the dialog type.
}
