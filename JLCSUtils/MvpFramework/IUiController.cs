using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    // To avoid a dependendy on WinForms:
    //
    // Summary (from WinForms):
    //     Specifies whether a window is minimized, maximized, or restored. Used by the
    //     System.Windows.Window.WindowState property.
    /// <summary>
    /// Specifies whether a window is minimized, maximized or neither.
    /// Equivalent to <see cref="System.Windows.WindowState"/> (but not specific to WinForms).
    /// </summary>
    public enum MvpWindowState  //| Named to avoid a name conflict with System.Windows.WindowState.
    {
        /// <summary>The window is neither minimised nor maximized.</summary>
        Normal = 0,
        /// <summary>The window is minimized.</summary>
        Minimized = 1,
        /// <summary>The window is maximized.</summary>
        Maximized = 2
    }


    /// <summary>
    /// Provides access to UI behaviours of the application or the main form.
    /// </summary>
    public interface IUiController : IMessageDialogService
        // or IMainFormController ?
    {
        #region Main Window

        /// <summary>
        /// The state of the application's main window.
        /// </summary>
        MvpWindowState MainFormState { get; set; }

        /// <summary>
        /// Bring this application's main window to the front.
        /// </summary>
        /// <returns>true on success. false if not supported or cannot be done at this time.</returns>
        bool BringToFront();

        // bool StayOnTop { get; set; } ?

        #endregion


        /// <summary>
        /// Show a simple message dialog.
        /// <para>See <see cref="IMessageDialogService"/> for a message dialog with more options.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        void ShowMessage(string message, string title = "");

    }
}
