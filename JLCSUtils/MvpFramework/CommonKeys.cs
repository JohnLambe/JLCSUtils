using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Commonly used or conventional keystrokes.
    /// </summary>
    public static class CommonKeys
    {
        /// <summary>
        /// ALT-ENTER for Properties, or 'edit the selected item'.
        /// Used by some Microsoft applications.
        /// </summary>
        public static readonly KeyboardKey Properties = KeyboardKey.Enter.AddModifier(KeyboardKey.Alt);

        /// <summary>
        /// F1 for Help.
        /// Convention used in Windows.
        /// </summary>
        public static readonly KeyboardKey Help = KeyboardKey.F1;

        /// <summary>
        /// Windows keystroke to close the current window.
        /// </summary>
        public static readonly KeyboardKey CloseWindow = KeyboardKey.F4.AddModifier(KeyboardKey.Alt);

        /// <summary>
        /// ALT-SPACE.
        /// Windows and Linux keystroke to open the system menu of the current window.
        /// </summary>
        public static readonly KeyboardKey SystemMenu = KeyboardKey.Space.AddModifier(KeyboardKey.Alt);

        //TODO
    }
}
