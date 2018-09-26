using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Util;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// Extended tab control.
    /// Handles extra keystrokes for changing tab.
    /// </summary>
    public class ExtTabControl : System.Windows.Forms.TabControl
    {
        public ExtTabControl()
        {
            KeyHandler = new TabControlKeyboardHandler( () => this.SelectedIndex, newIndex => this.SelectedIndex = newIndex);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (KeyHandler.NotifyKey(keyData.ToKeyboardKey()))
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// The modifier keys to use with the specified number keys (<see cref="ChangeTabNumberKeys"/>) to change the tab using the keyboard.
        /// This can be <see cref="Keys.None"/>, but that may then prevent the normal function of the relevant keys from working.
        /// </summary>
        public virtual Keys ChangeTabModifierKeys
        {
            get { return KeyHandler.ModifierKeys.ToKeys(); }
            set { KeyHandler.ModifierKeys = value.ToKeyboardKey(); }
        }

        /// <summary>
        /// Which set of keys to use (with the specified modifier keys <see cref="ChangeTabModifierKeys"/>) to change the tab using the keyboard.
        /// <see cref="Keys.None"/> to disable.
        /// </summary>
        public virtual NumberKeysType ChangeTabNumberKeys
        {
            get { return KeyHandler.NumbersKeys; }
            set { KeyHandler.NumbersKeys = value; }
        }

        protected TabControlKeyboardHandler KeyHandler { get; }
    }
}
