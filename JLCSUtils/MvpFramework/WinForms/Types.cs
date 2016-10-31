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

    public static class KeyboardKeyExt
    {
        public static Keys ToKeys(this KeyboardKey key)
        {
            return (Keys)key;
        }

        public static KeyboardKey ToKeyboardKey(Keys key)
        {
            return (KeyboardKey)key;
        }
    }

    public static class MvpWindowStateExt
    {
        public static MvpWindowState ToMvpWindowState(this FormWindowState state)
        {
            return (MvpWindowState)state;
        }

        public static FormWindowState ToWinForms(this MvpWindowState state)
        {
            return (FormWindowState)state;
        }
    }
}
