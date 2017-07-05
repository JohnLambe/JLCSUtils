using JohnLambe.Util.Misc;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// <see cref="Button"/> that can be bound automatically by the MVP framework.
    /// </summary>
    [MvpBoundControl(IconProperty = nameof(Image), IconIdProperty = nameof(IconId))]
    public class MvpButton : Button
    {
#pragma warning disable CS0067   // Suppress 'Event never used'.  This is fired by reflection.
        [MvpHandlerIdProperty(nameof(Click))]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description(MvpUiComponentConsts.HandlerIdDescription)]
        public event GetNameDelegate OnGetHandlerId;
        //        public virtual string HandlerId { get; set; }
#pragma warning restore CS0067

        #region Icon

        /// <summary>
        /// The ID of the icon of this control, to be resolved by <see cref="IconRepository"/>.
        /// </summary>
        [Description("The IconId of the icon for this control. null of \"\" to make the icon blank.")]
        [IconId]
        public virtual string IconId
        {
            get { return _iconId; }
            set
            {
                _iconId = value;
                Image = IconUtil.GetImageByIconId(Image, IconRepository, IconId);
            }
        }
        private string _iconId;

        /// <summary>
        /// The icon repository for resolving values of <see cref="IconId"/>.
        /// </summary>
        [Description("The repository to resolve IconId values.")]
        public virtual IIconRepository<string, Image> IconRepository
        {
            get { return _iconRepository; }
            set
            {
                _iconRepository = value;
                Image = IconUtil.GetImageByIconId(Image, IconRepository,IconId);
            }
        }
        private IIconRepository<string, Image> _iconRepository;

        #endregion

        #region HotKey

        /// <summary>
        /// Keystroke to invoke the button.
        /// <para>
        /// This is in additional to any WinForms accelerator character (indicated by an '&' in the caption),
        /// and is not limited to keystrokes using ALT.
        /// </para>
        /// </summary>
        public virtual Keys HotKey { get; set; } = Keys.None;

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (HotKey != Keys.None && kevent.KeyCode == HotKey)   // if there is a HotKey and this is it
            {
                PerformClick();
                kevent.Handled = true;
            }
            else
            {
                base.OnKeyDown(kevent);
            }
        }

        #endregion
    }

}
