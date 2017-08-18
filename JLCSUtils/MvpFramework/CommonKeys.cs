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

        /// <summary>
        /// CTRL-C: Copy.
        /// </summary>
        public static readonly KeyboardKey Copy = KeyboardKey.C.AddModifier(KeyboardKey.Control);
        /// <summary>
        /// CTRL-V: Paste.
        /// </summary>
        public static readonly KeyboardKey Paste = KeyboardKey.V.AddModifier(KeyboardKey.Control);
        /// <summary>
        /// CTRL-X: Cut.
        /// </summary>
        public static readonly KeyboardKey Cut = KeyboardKey.X.AddModifier(KeyboardKey.Control);

        /// <summary>
        /// CTRL-A: Select All.
        /// </summary>
        public static readonly KeyboardKey SelectAll = KeyboardKey.A.AddModifier(KeyboardKey.Control);

        /// <summary>
        /// CTRL-Z: Undo.
        /// </summary>
        public static readonly KeyboardKey Undo = KeyboardKey.Z.AddModifier(KeyboardKey.Control);

        /// <summary>
        /// CTRL-SHIFT-Z: Redo.
        /// Some systems use CTRL-Y.
        /// </summary>
        public static readonly KeyboardKey Redo = KeyboardKey.Z.AddModifier(KeyboardKey.Control).AddModifier(KeyboardKey.Shift);


        // CTRL-B: Bold
        // CTRL-D: Bookmark (web browsers).
        // CTRL-E: Get/import (picture etc.) - Quark XPress.
        // CTRL-F: Find.
        // CTRL-G: Go to (line etc.).
        // CTRL-I: Italic.
        // CTRL-N: New (or INS).
        // CTRL-O: Open.
        // CTRL-P / PrtScrn: Print.
        // CTRL-Q: Quit (not common)?
        // CTRL-R: Refresh (or F5).
        // CTRL-S: Save.
        // CTRL-U: Underline.


        //TODO: More keys
    }
}
