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
    public enum MvpWindowState  //TODO: find a better name
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
    public interface IUiController : IMessageDialog
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
        /// <para>See <see cref="IMessageDialog"/> for a message dialog with more options.</para>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        void ShowMessage(string message, string title = "");

    }






    // Not part of framework:

    /// <summary>
    /// Provides access to features related to locking a session/workstation/screen.
    /// </summary>
    public interface ISessionLockController
    {
        /// <summary>
        /// Lock the current session (i.e. hide all windows (or at least all potentially confidential content within them,
        /// and disable all functionality of the system (except closing it) until the user authenticates).
        /// </summary>
        /// <returns>true if the session was locked by this call, or was already locked when called.
        /// False if locking is disabled or not supported.</returns>
        bool Lock();

        /// <summary>
        /// true iff the current sesson is locked.
        /// </summary>
        bool IsLocked { get; }

        // OR bool Locked { get; set; }    but setting to false wouldn't be allowed.

        /// <summary>
        /// Iff the system is configured to lock when there is no UI activity for a certain time,
        /// this resets that timer (i.e. indicates that something has been done that implies that the
        /// user is present).
        /// If no such locking is enabled/supported, this does nothing.
        /// </summary>
        void NotifyActive();

        /// <summary>
        /// Require the logged in user to re-authenticate themselves.
        /// (Can be used before doing or accessing something security-critical).
        /// </summary>
        /// <returns>true if successfully authenticaed; false if authentication failed; null if not supported or the system is configured to not require it.</returns>
        bool? Reauthenticate();

        /// <summary>
        /// Temporarily disable session locking.
        /// </summary>
        /// <param name="maxTime">If not 0, if this time expires, locking is automatically resumed (in milliseconds).</param>
        /// <returns></returns>
        ISessionLockSuspensionHandler SuspendLocking(int maxTime = 0);
        // OR void SuspendLocking(INotifyOnDispose caller);  // locking is resumed if the given object is disposed
        //    void ResumeSessionLocking(INotifyOnDispose caller);
    }


    /// <summary>
    /// Acquired by a call to <see cref="ISessionLockController.SuspendLocking(int)"/>.
    /// If any of these handles are active, session locking is disabled.
    /// </summary>
    public interface ISessionLockSuspensionHandler
    {
        /// <summary>
        /// Resume session locking and destroy this instance.
        /// </summary>
        void Release();
    }


    public interface ISesson
    {
        string SessionId { get; set; }

        ISessionLockController LockController { get; }

        // other Session-related properties ...
    }
}
