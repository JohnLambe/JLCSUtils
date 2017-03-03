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

        /// <summary>
        /// Fired when the view is opening or closing.
        /// <para>It can be fired before closing (<see cref="ViewVisibilityChangedEventArgs.Closed"/> is false), giving handlers the option of preventing it from closing.
        /// Views are not required to support that.
        /// If it is fired with <see cref="ViewVisibilityChangedEventArgs.Closed"/>==false, it must still be fired again with <see cref="ViewVisibilityChangedEventArgs.Closed"/>==true (when it is actually closed).
        /// </para>
        /// </summary>
        event ViewVisibilityChangedDelegate ViewVisibilityChanged;

        /*TODO: Events:
        event EventHandler<WindowViewOpenedEventArgs> Opened;
        event EventHandler Closed;
        */
    }

    /*
    public class WindowViewOpenedEventArgs : EventArgs
    {
        /// <summary>
        /// True iff the window was shown modally.
        /// </summary>
        public virtual bool IsModal { get; set; }
    }
    */
}
