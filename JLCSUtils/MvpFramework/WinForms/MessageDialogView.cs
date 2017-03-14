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
    /// </summary>
    [View(Interface = typeof(IMessageDialogView))]   // References its interface explicitly, so that it is validated at compile time.
    public partial class MessageDialogView : WindowViewBase, IMessageDialogView
    {
        public MessageDialogView(IIconRepository<string,object> iconRepository)
        {
            InitializeComponent();

            _iconRepository = iconRepository ?? new NullIconRepository<string, object>();
            DefaultButtonsBackColor = uiButtons.BackColor;           // the default color is that specified in the form designer
        }

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

                //TODO: Detail message

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

        public virtual MessageDialogViewModel Model { get; set; }
    }
}
