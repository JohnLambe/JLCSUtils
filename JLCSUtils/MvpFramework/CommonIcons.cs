using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Identifiers for common non-domain-specific icons.
    /// </summary>
    public static class CommonIcons
    {
        /// <summary>
        /// Delete an entity.
        /// <para>
        /// Note: An 'X' is a common icon for 'Delete', but 'X' can be used for several other things
        /// (Remove, Clear, No, Wrong, Cancel, Abort) so it recommended that this use a different icon,
        /// such as a bin.
        /// </para>
        /// </summary>
        /// <seealso cref="Remove"/>
        public const string Delete = "Delete";

        /// <summary>
        /// Remove something from a list or break its associated with something else.
        /// <para>Suggested icon: 'X'.</para>
        /// </summary>
        public const string Remove = "Remove";

        /// <summary>
        /// Create a new entity.
        /// </summary>
        public const string New = "New";

        /// <summary>
        /// Edit an item. (May be the same button for viewing it).
        /// <para>Common icons: Pen or pencil.</para>
        /// </summary>
        public const string Edit = "Edit";

        /// <summary>
        /// View details of an item without the ability to edit it.
        /// </summary>
        public const string View = "View";

        /// <summary>
        /// Search for something.
        /// <para>Common icons: Magnifying glass or binoculars.</para>
        /// </summary>
        public const string Search = "Search";

        /// <summary>
        /// 
        /// </summary>
        public const string Cancel = "Cancel";

        /// <summary>
        /// 
        /// </summary>
        public const string Ok = "Ok";

        /// <summary>
        /// 
        /// </summary>
        public const string Confirm = "Confirm";

        /// <summary>
        /// Stop and abandon an operation.
        /// <para>
        /// It is suggested that the icon would be in the shape of a traffic Stop sign (octagon).
        /// </para>
        /// </summary>
        public const string Abort = "Abort";

        /// <summary>
        /// Denotes an informational dialog (not a warning, error or confirmation dialog),
        /// or other place that gives similar information.
        /// <para>
        /// Suggested icon: Tourist information symbol: White lowercase letter 'i' on a filled blue circle.
        /// </para>
        /// </summary>
        public const string Information = "Information";

        /// <summary>
        /// Denotes a confirmation dialog (where the user is asked to confirm an action),
        /// or other place with a similar function.
        /// <para>
        /// Note: The icons for both this and 'Help' may have a question mark in them, so there must be a clear distinction between them.
        /// It is suggested that this have a question mark on a diamond-shaped background (flow chart symbol) and
        /// 'Help' would have a circular background.
        /// </para>
        /// </summary>
        public const string Confirmation = "Confirmation";

        /// <summary>
        /// An icon related to a warning to the user (e.g. about something dangerous, or that they may be doing by mistake).
        /// </summary>
        public const string Warning = "Warning";

        /// <summary>
        /// 
        /// </summary>
        public const string Error = "Error";

        /// <summary>
        /// Help: Relates to instructions or guidance to users.
        /// </summary>
        public const string Help = "Help";

        /// <summary>
        /// Leave a page or area of the application, close a window, etc.
        /// </summary>
        public const string Exit = "Exit";

        /// <summary>
        /// Answering 'Yes' to a question.
        /// </summary>
        public const string Yes = "Yes";

        /// <summary>
        /// 'Yes to All': For the user to answer 'Yes' for all items in a collection.
        /// </summary>
        public const string YesAll = "YesAll";

        /// <summary>
        /// Answering 'No' to a question.
        /// </summary>
        /// <remarks>
        /// ISO 3864 defines a standard Universal 'No' Symbol (https://en.wikipedia.org/wiki/No_symbol),
        /// but this is more related to prohibition than answering 'No' to a question.
        /// </remarks>
        public const string No = "No";

        /// <summary>
        /// 'No to All'
        /// </summary>
        public const string NoAll = "NoAll";

        /// <summary>
        /// Icon that opens a menu when clicked/tapped.
        /// </summary>
        public const string Menu = "Menu";

        /// <summary>
        /// Undo the last action.
        /// <para>Common icon: Curved arrow pointing left or anticlockwise.</para>
        /// </summary>
        public const string Undo = "Undo";

        /// <summary>
        /// Redo an action undone with 'Undo'.
        /// <para>Common icon: Curved arrow in a similar style to the 'Undo' icon but pointing in the opposite direction (right or clockwise).</para>
        /// </summary>
        public const string Redo = "Redo";

        /// <summary>
        /// Refresh data - update to the latest version or from a live source.
        /// </summary>
        public const string Refresh = "Refresh";

        /// <summary>
        /// Preferences / settings / application configuration.
        /// </summary>
        public const string Preferences = "Preferences";

        #region Security

        /// <summary>
        /// Icon representing a user of the system.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Log out of the system.
        /// </summary>
        public const string Logout = "Logout";

        // Login ?
        // Role / User group ?

        /// <summary>
        /// Log out of the system.
        /// </summary>
        public const string ChangePassword = "ChangePassword";

        #endregion

        #region File

        /// <summary>
        /// Open/load a file/document.
        /// </summary>
        public const string Open = "Open";

        /// <summary>
        /// Save something to persistent storage.
        /// </summary>
        public const string Save = "Save";

        #endregion

        public const string Home = "Home";

        public const string Print = "Print";

        public const string Scan = "Scan";

        /// <summary>
        /// Bring data into the system from an external source.
        /// <para>Suggested icon: An icon to represent the system or a database, with an arrow pointing towards it.</para>
        /// </summary>
        public const string Import = "Import";

        /// <summary>
        /// Output data to something external to the system.
        /// <para>Suggested icon: Similar to 'Import' but with the arrow pointing in the opoosite direction, and maybe, in a different color.</para>
        /// </summary>
        public const string Export = "Export";

        // Media actions: Playm, Stop, Rewind, etc. ?

        // File types ?

    }
}
