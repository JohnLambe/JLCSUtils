using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// A presenter that has its own window.
    /// </summary>
    public interface IWindowPresenter
    {
        /// <summary>
        /// Show window modally.
        /// </summary>
        /// <returns>Depends on the presenter.</returns>
        object ShowModal();

        /// <summary>
        /// Close the window.
        /// </summary>
        void Close();


        /*TODO: Events:
        event EventHandler<WindowViewOpenedEventArgs> Opened;
        event EventHandler Closed;
        */
    }
}
