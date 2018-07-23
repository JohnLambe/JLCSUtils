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
        /// Delete an entity (NOT just remove a reference to it), or something, usually from persistent storage.
        /// <para>
        /// Note: An 'X' is a common icon for 'Delete', but 'X' can be used for several other things
        /// (Remove, Clear, No, Wrong, Error, Close, Cancel, Abort) so it recommended that this use a different icon,
        /// such as a bin.
        /// </para>
        /// </summary>
        /// <seealso cref="Remove"/>
        public const string Delete = "Delete";

        /// <summary>
        /// Remove something from a list or break its associated with something else.
        /// If actually deleting an entity, use <seealso cref="Delete"/>.
        /// <para>Opposite of <see cref="Add"/>.</para>
        /// <para>Suggested icon: '-' (minus), or 'X' (in a different style to any other icons that use an 'X').</para>
        /// </summary>
        /// <seealso cref="Delete"/>
        /// <seealso cref="Clear"/>
        public const string Remove = "Remove";

        /// <summary>
        /// Add an item to a collection, or add an association from the current item to a selected one.
        /// If creating a new item to add, <see cref="New"/> is probably the better icon (and a caption of "New").
        /// <para>Opposite of <see cref="Remove"/>.</para>
        /// <para>Possible icon: '+' (plus).</para>
        /// </summary>
        /// <seealso cref="New"/>
        public const string Add = "Add";

        /// <summary>
        /// Remove some details, or set to blank.
        /// <para>Suggested icon: 'X' (in a different style to any other icons that use an 'X').</para>
        /// </summary>
        /// <seealso cref="Remove"/>
        public const string Clear = "Clear";

        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <seealso cref="Add"/>
        public const string New = "New";

        /// <summary>
        /// Edit an item. (May be the same button for viewing it).
        /// <para>Common icons: Pen or pencil.</para>
        /// </summary>
        public const string Edit = "Edit";

        /// <summary>
        /// View details of an item, without the ability to edit it.
        /// <para>Possible icon: Sometimes a magnifying glass is used, but that could be confused with <see cref="Search"/>.</para>
        /// </summary>
        public const string View = "View";

        /// <summary>
        /// View/Edit details of an item.
        /// </summary>
        public const string Detail = "Detail";

        /// <summary>
        /// Represents details/settings/options for use by advanced users.
        /// <para>Possible icon: Cogwheel (but this may be used for <see cref="Preferences"/>).</para>
        /// </summary>
        public const string Advanced = "Advanced";

        /// <summary>
        /// Search for something.
        /// <para>Common icons: Magnifying glass or binoculars.</para>
        /// </summary>
        public const string Search = "Search";

        /// <summary>
        /// A UI element (usually a button) to cancel an action.
        /// <para>Possible icon: 'X' in red circle.</para>
        /// </summary>
        /// <seealso cref="Ok"/>
        /// <seealso cref="Abort"/>
        public const string Cancel = "Cancel";

        /// <summary>
        /// An 'Ok' button or equivalent.
        /// <para>Opposite of <see cref="Cancel"/>.</para>
        /// <para>Common icon: Check mark; Check mark in a green circle, in a similar style and size to the <see cref="Cancel"/> icon.</para>
        /// </summary>
        /// <seealso cref="Cancel"/>
        /// <seealso cref="Confirm"/>
        public const string Ok = "Ok";

        /// <summary>
        /// A UI element (usually a button) to confirm an action.
        /// <para>Suggested icon: Check mark (different style to <see cref="Ok"/>).</para>
        /// </summary>
        /// <seealso cref="Ok"/>
        public const string Confirm = "Confirm";

        /// <summary>
        /// Stop and abandon an operation.
        /// <para>Suggested icon: In the shape of a traffic Stop sign (octagon), possibly colored red.
        /// Sometimes an 'X' is used, but an 'X' icon can be ambiguous.
        /// </para>
        /// </summary>
        /// <seealso cref="Cancel"/>
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
        /// An icon related to a warning to the user (e.g. about something dangerous or irreversible, or that they may be doing by mistake, 
        /// or without understanding the consequences)
        /// (NOT an <see cref="Error"/>).
        /// </summary>
        /// <seealso cref="Error"/>
        public const string Warning = "Warning";

        /// <summary>
        /// Indicates that something has gone wrong, or the user has done or entered something invalid.
        /// </summary>
        public const string Error = "Error";

        /// <summary>
        /// Help: Relates to instructions or guidance to users.
        /// <para>Suggested icon: A book; Question mark on a circular (probably blue) background.</para>
        /// </summary>
        public const string Help = "Help";

        /// <summary>
        /// Leave a page or area of the application, close a window, etc.
        /// <para>Possible icon: Door, possibly with an arrow pointing towards it.</para>
        /// </summary>
        /// <seealso cref="Logout"/>
        public const string Exit = "Exit";       // same as 'Close' ?

        /// <summary>
        /// Answering 'Yes' to a question.
        /// </summary>
        public const string Yes = "Yes";

        /// <summary>
        /// 'Yes to All': For the user to answer 'Yes' for all items in a collection.
        /// <para>Suggested icon: The icon for 'Yes', repeated (2 or 3 of them), overlapping.</para>
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
        /// <para>Suggested icon: The icon for 'No', repeated (2 or 3 of them), overlapping.</para>
        /// </summary>
        public const string NoAll = "NoAll";

        /// <summary>
        /// Icon that opens a menu when clicked/tapped.
        /// <para>Common icon: A vertical row of horizontal lines (representing items in a menu); a menu.</para>
        /// </summary>
        public const string Menu = "Menu";

        /// <summary>
        /// Undo the last action.
        /// <para>Common icon: Curved arrow pointing left or anticlockwise.</para>
        /// </summary>
        /// <seealso cref="Redo"/>
        public const string Undo = "Undo";

        /// <summary>
        /// Redo an action undone with <see cref="Undo"/>.
        /// <para>Common icon: Curved arrow in a similar style to the 'Undo' icon but pointing in the opposite direction (right or clockwise).</para>
        /// </summary>
        /// <seealso cref="Undo"/>
        public const string Redo = "Redo";

        /// <summary>
        /// Refresh data - update to the latest version or from a live source.
        /// <para>Common icon: Two or more arrows going in a circle, often colored green.
        /// The icon must be easily distinguishable from that of <see cref="Undo"/> / <see cref="Redo"/>.</para>
        /// </summary>
        public const string Refresh = "Refresh";

        /// <summary>
        /// Preferences / settings / application configuration.
        /// <para>Common icons: Cogwheel; a few checkboxes (vertical list with lines representing captions beside them), etc.</para>
        /// </summary>
        public const string Preferences = "Preferences";  // could also be called "Configuration"

        #region Security

        /// <summary>
        /// Icon representing a user of the system.
        /// <para>Common icon: A person. (Icons representing different types of people (e.g. customer, contact, blog poster, person in a certain profession) can sometimes be represented
        /// by a person wearing a uniform, or a person icon combined with something else).</para>
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Icon representing a group of users with common rights or characteristics.
        /// (A role).
        /// <para>Common icon: Icon for a user repeated (probably overlapping); A person with a hat (representing their role); a hat.</para>
        /// </summary>
        public const string UserGroup = "UserGroup";

        // user right

        /// <summary>
        /// Log out of the system.
        /// <para>Possible icons: Login dialog (two fields representing username and password); (lock) key.</para>
        /// </summary>
        /// <seealso cref="Logout"/>
        public const string Login = "Login";

        /// <summary>
        /// Log out of the system.
        /// <para>Possible icon: Padlock; Similar to <see cref="Exit"/>; Inverse of <see cref="Login"/> icon if it is something that can have an inverse/opposite.</para>
        /// </summary>
        /// <seealso cref="Exit"/>
        public const string Logout = "Logout";

        /// <summary>
        /// Log out of the system.
        /// <para>Possible icon: Two (lock) keys with an arrow pointing from one to the other (from left to right if text is written from left to right).</para>
        /// </summary>
        public const string ChangePassword = "ChangePassword";

        #endregion

        #region File

        /// <summary>
        /// A file on disc (or equivalent).
        /// </summary>
        public const string File = "File";

        /// <summary>
        /// Open/load a file/document.
        /// </summary>
        public const string Open = "Open";

        /// <summary>
        /// Save something to persistent storage.
        /// </summary>
        public const string Save = "Save";

        // File types ?

        #endregion

        #region Networking

        public const string Server = "Server";

        public const string Client = "Client";

        // network ?
        // connection

        #endregion

        /// <summary>
        /// Go to a 'Home' page or screen/window (an initial point, or the root of a tree of UI pages/screens etc.).
        /// <para>Common icon: A house.</para>
        /// </summary>
        public const string Home = "Home";

        /// <summary>
        /// Print a document or item.
        /// <para>Possible icon: Printer; Printer with an arrow pointing towards it; Arrow pointing from a document or database to a printer.</para>
        /// </summary>
        public const string Print = "Print";

        /// <summary>
        /// Import an image or document from a document scanner.
        /// <para>Possible icon: Scanner; Scanner with an arrow pointing from it; Arrow pointing from a scanner to a database.</para>
        /// </summary>
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


    }
}
