namespace MvpFramework.Dialog
{
    /// <summary>
    /// Model-level code relating to message dialogs.
    /// <para>
    /// Features include:<br/>
    /// - A system of mapping exceptions to dialog types.<br/>
    /// - A mechanism to attach handlers for logging and/or intercepting dialogs (for modifying or suppressing them).
    /// </para>
    /// 
    /// <h2>Dialog types</h2>
    /// <para>
    /// Users often dismiss a dialog without reading the message (possibly assuming it to be a different message,
    /// or just dismissing messages quickly because most of them are unimportant).
    /// To reduce this (dismissing a dialog without knowing its purpose), we recommend indicating as much as possible by visual (non-text) cues.
    /// <para>
    /// This system (<see cref="MessageDialogType"/>) encourages dialogs that have a visual indication (icon, colour, style, dialog box shape etc.)
    /// of the type of dialog (warning, error, confirmation, information, etc.) and
    /// another visual indication of the specific message (usually an icon or image).
    /// A colour (or colour scheme) or other style (maybe a differently-shaped dialog - with a diagonal cutout in a corner, etc.)
    /// for the first one may be preferable to having two icons in the same dialog
    /// - to avoid it looking cluttered (though it could have one icon to the right of the window and the other to the left,
    /// the dialog type icon could be the icon on the left of the title bar, or the message icon could be a background image).
    /// The details of the former are defined in the MessageDialogType object for the dialog.
    /// This can include an icon, which, depending on the UI, may be shown in addition to the icon for the specific
    /// message, or shown only when the specific message does not have its own icon.
    /// In the latter case, it is recommended that the icons for the specific message type should indicate
    /// an indication of the dialog type (warning, information, etc.), e.g. by overlaying an icon or by a colour scheme for each type.
    /// </para>
    /// </para>
    /// 
    /// </summary>
    static class NamespaceDoc { }        // Don't add anything here. This class exists only to hold a documentation comment for the namespace.
}
