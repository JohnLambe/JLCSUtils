using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    public interface IWindowView : IView
    {
        /// <summary>
        /// Show the window modally.
        /// </summary>
        /// <returns>Depends on the view.</returns>
        object ShowModal();
        // The WinForms equivalent is ShowDialog().

        /// <summary>
        /// Close the window.
        /// </summary>
        void Close();

        /*TODO: Events:
        event EventHandler<WindowViewOpenedEventArgs> Opened;
        event EventHandler Closed;
        */
    }

    public class WindowViewOpenedEventArgs : EventArgs
    {
        /// <summary>
        /// True iff the window was shown modally.
        /// </summary>
        public virtual bool IsModal { get; set; }
    }
}
