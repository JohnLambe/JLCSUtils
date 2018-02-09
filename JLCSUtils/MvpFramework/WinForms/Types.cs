using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    // Conversion to and from WinForms types:

    /// <summary>
    /// Extension methods of or relating to <see cref="KeyboardKey"/>.
    /// </summary>
    public static class KeyboardKeyExtension  //TODO: Rename to avoid conflict with MvpFramework.KeyboardKeyExtension
    {
        /// <summary>
        /// Converts this <see cref="KeyboardKey"/> to a <see cref="System.Windows.Forms.Keys"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Keys ToKeys(this KeyboardKey key)
        {
            return (Keys)key;
        }

        /// <summary>
        /// Converts this <see cref="Keys"/> to a <see cref="KeyboardKey"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyboardKey ToKeyboardKey(Keys key)
        {
            return (KeyboardKey)key;
        }
    }

    /// <summary>
    /// Extension methods of or relating to <see cref="FormWindowState"/>.
    /// </summary>
    public static class MvpWindowStateExtension
    {
        /// <summary>
        /// Converts this <see cref="FormWindowState"/> to a <see cref="MvpWindowState"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static MvpWindowState ToMvpWindowState(this FormWindowState state)
        {
            return (MvpWindowState)state;
        }

        /// <summary>
        /// Converts this <see cref="MvpWindowState"/> to a <see cref="FormWindowState"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static FormWindowState ToWinForms(this MvpWindowState state)
        {
            return (FormWindowState)state;
        }
    }
}
