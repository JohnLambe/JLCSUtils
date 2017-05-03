using JohnLambe.Util;
using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Misc;
using JohnLambe.Util.Text;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    // Users often dismiss a dialog without reading the message (possibly assuming it to be a different message,
    // or just dismissing messages quickly because most of them are unimportant).
    // To reduce this (dismissing a dialog without knowing its purpose), we recommend indicating as much as possible by visual (non-text) cues.
    //
    // This system encourages dialogs that have a visual indication (icon, colour, style, dialog box shape etc.)
    // of the type of dialog (warning, error, confirmation, information, etc.) and
    // another visual indication of the specific message (usually an icon or image).
    // A colour (or colour scheme) or other style (maybe a differently-shaped dialog - with a diagonal cutout in a corner, etc.)
    // for the first one may be preferable to having two icons in the same dialog
    // - to avoid it looking cluttered (though it could have one icon to the right of the window and the other to the left,
    // the dialog type icon could be the icon on the left of the title bar, or the message icon could be a background image).
    // The details of the former are defined in the MessageDialogType object for the dialog.
    // This can include an icon, which, depending on the UI, may be shown in addition to the icon for the specific
    // message, or shown only when the specific message does not have its own icon.
    // In the latter case, it is recommended that the icons for the specific message type should indicate
    // an indication of the dialog type (warning, information, etc.), e.g. by overlaying an icon or by a colour scheme for each type.
    // 


    /// <summary>
    /// Category of message.
    /// </summary>
    [Immutable]  // This must be immutable since instances are used like constants.
    [DefaultDialogModelType(typeof(MessageDialogModel<string>))]
    public class MessageDialogType
    {
        /// <summary>
        /// Conventioanl suffix for names of subclasses of this.
        /// </summary>
        /// <seealso cref="MessageDialogModel{TResult}.ClassNameSuffix"/>
        public const string ClassNameSuffix = "DialogType";

        public MessageDialogType(string id, IOptionCollection options, string description = null)
        {
            if (id == null)
                id = GetType().Name.RemoveSuffix(ClassNameSuffix);

            Id = id;
            Icon = id;
            Description = description ?? CaptionUtil.PascalCaseToCaption(id);
            DefaultOptions = options;
        }

        /// <summary>
        /// Identifies this dialog type. May be used in logs, etc.
        /// </summary>
        public virtual string Id { get; private set; }

        /// <summary>
        /// Human-readable description of this dialog type.
        /// </summary>
        public virtual string Description { get; private set; }

        /// <summary>
        /// Identifier of an icon indicating the type of message or error.
        /// May be null, for a default determined by <see cref="MessageDialogType"/> (possibly no icon).
        /// </summary>
        [IconId]
        public virtual string Icon { get; private set; }

        /// <summary>
        /// Default options for this type of message dialog.
        /// </summary>
        public virtual IOptionCollection DefaultOptions { get; private set; }

        /// <summary>
        /// A suggested color of a UI element in a dialog for this type of message (a color to distinguish types of message).
        /// </summary>
        public virtual Color DefaultColor { get; private set; } = Color.Empty;

        /// <summary>
        /// Usually a less satured version of <see cref="DefaultColor"/>,
        /// intended for use as a background of larger areas, or for a UI with less saturated color.
        /// </summary>
        public virtual Color DefaultColorMuted => DefaultColor;


        #region Common sets of options

        // Note: These are referenced from the 'Standard Dialog Types' region, so must be declared first.

        /// <summary>
        /// An 'Ok' button.
        /// </summary>
        public static readonly IOptionCollection Options_Ok = new OptionCollection(
            new MenuItemModel[]
            {
                new MenuItemModel(MessageDialogResponse.Ok) { DisplayName = "Ok", AcceleratorChar = 'O', IsDefault = true }
            }
        );

        /// <summary>
        /// 'Yes', 'No' and 'Cancel' buttons.
        /// </summary>
        public static readonly IOptionCollection Options_Confirm = new OptionCollection(
            new MenuItemModel[]
            {
                new MenuItemModel(MessageDialogResponse.Yes) { DisplayName = "Yes", AcceleratorChar = 'Y' },
                new MenuItemModel(MessageDialogResponse.No) { DisplayName = "No", AcceleratorChar = 'N' },
                new MenuItemModel(MessageDialogResponse.Cancel) { DisplayName = "Cancel", AcceleratorChar = 'C' },
            }
        );

        #endregion

        #region Standard Dialog Types

        /// <summary>
        /// Information that is not an error or warning.
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        public static readonly MessageDialogType Informational = new InformationalDialogType();

        /// <summary>
        /// Asks the user to confirm an action.
        /// Default options are "Yes", "No" and "Cancel".
        /// </summary>
        public static readonly MessageDialogType Confirmation = new ConfirmationDialogType();

        /// <summary>
        /// A warning.
        /// By default, has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        public static readonly MessageDialogType Warning = new WarningDialogType();

        /// <summary>
        /// A warning that is more severe than the <see cref="Warning"/> option.
        /// This may have a different UI style to draw attention to it, and/or different UI behaviours
        /// (e.g. a delay of 1-2 seconds before allowing the user to respond, so that a user automatically pressing a key to dismiss when they couldn't possibly have read it won't be accepted).
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        public static readonly MessageDialogType SevereWarning = new SevereWarningDialogType();

        /// <summary>
        /// An error.
        /// By default, has only one option (typically called "Ok") - to dismiss it (the same applies to all type with names ending with "Error").
        /// </summary>
        public static readonly MessageDialogType Error = new ErrorDialogType();

        /// <summary>
        /// An error caused by a user, such as user input that failed validation.
        /// </summary>
        public static readonly MessageDialogType UserError = new UserErrorDialogType();

        /// <summary>
        /// An system error, e.g. disc full, network connection failed.
        /// </summary>
        public static readonly MessageDialogType SystemError = new SystemErrorDialogType();

        /// <summary>
        /// An error that should never happen. An internal failure, such as an assertion failure.
        /// </summary>
        public static readonly MessageDialogType InternalError = new InternalErrorDialogType();

        #endregion

    }

    /// <summary>
    /// Information message that is not an error, warning or confirmation, and generally doesn't offer a choice.
    /// By default has only one option (typically called "Ok") - to dismiss it.
    /// </summary>
    public class InformationalDialogType : MessageDialogType
    {
        public InformationalDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Ok, description)
        {
        }

        public override Color DefaultColor => Color.Blue;
        public override Color DefaultColorMuted => Color.FromArgb(200, 200, 255);

        // Suggested icon: 'i' on blue circle (tourist information  sign)
    }

    /// <summary>
    /// Message that asks the user to confirm an action.
    /// Default options are "Yes", "No" and "Cancel".
    /// </summary>
    public class ConfirmationDialogType : MessageDialogType
    {
        public ConfirmationDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Confirm, description)
        {
        }

        public override Color DefaultColor => Color.FromArgb(128, 255, 255);   // cyan
        public override Color DefaultColorMuted => Color.FromArgb(200, 255, 255);  // desaturated cyan

        // Suggested icon: blue '?' in white circle with blue border
    }

    /// <summary>
    /// Message that warns the user of something (it can be something that has happened or is about to be done).
    /// By default, has only one option (typically called "Ok") - to dismiss it.
    /// </summary>
    public class WarningDialogType : MessageDialogType
    {
        public WarningDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Ok, description)
        {
        }

        public override Color DefaultColor => Color.Yellow;
        public override Color DefaultColorMuted => Color.FromArgb(255, 255, 200);  // desaturated yellow

        // Suggested icon: black '!' in yellow isosceles triangle (bottom side horizontal) with black border
    }

    /// <summary>
    /// Message that warns the user about something dangerous, etc. More serious than <see cref="WarningDialogType"/>.
    /// This may have a different UI style to draw attention to it, and/or different UI behaviours
    /// (e.g. a delay of 1-2 seconds before allowing the user to respond, so that a user automatically pressing a key to dismiss when they couldn't possibly have read it won't be accepted).
    /// By default has only one option (typically called "Ok") - to dismiss it.
    /// </summary>
    public class SevereWarningDialogType : WarningDialogType
    {
        public SevereWarningDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options, description)
        {
        }

        public override Color DefaultColor => Color.Red;

        // Suggested icon: Like 'Warning' but with the yellow colour replaced wih red
    }

    /// <summary>
    /// A dialog that gives a warning and confirms an action.
    /// This should have the style of a warning, but would have confirmation options/buttons.
    /// </summary>
    public class WarningConfirmationDialogType : WarningDialogType
    {
        public WarningConfirmationDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Confirm, description)
        {
        }

        // same stlye as Warning
    }

    /// <summary>
    /// An error - something that has gone wrong.
    /// By default, has only one option (typically called "Ok") - to dismiss it (the same applies to all type with names ending with "Error").
    /// </summary>
    [MappedException(typeof(Exception))]
    public class ErrorDialogType : MessageDialogType
    {
        public ErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Ok, description)
        {
        }

        public override Color DefaultColor => Color.LightGray;

        // Suggested icon: white 'X' on red background (circle or square); background could be dark gray if red is reserved for serious warnings.
        //   Must be clearly distinguished from other icons that often have an 'X': Close, Delete, 'No' / Cancel, Abort, Test Failed.
    }

    /// <summary>
    /// An error caused by the user, e.g. invalid input.
    /// </summary>
    [MappedException(typeof(UserErrorException))]
    [MappedException(typeof(System.ComponentModel.DataAnnotations.ValidationException))]
    public class UserErrorDialogType : MessageDialogType
    {
        public UserErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Ok, description)
        {
        }

        public override Color DefaultColor => Color.Khaki;
        public override Color DefaultColorMuted => Color.Beige;

        // Suggested icon: red 'X' on transparent background; OR universal 'no' symbol on white background (foreground could be red or black)
    }

    /// <summary>
    /// An error related to the system or environment, e.g. I/O error, disc full, network connection failed.
    /// </summary>
    [MappedException(typeof(SystemException))]
    // [MappedException(typeof(System.IO.IOException))]  // subclass of SystemException
    //   Sometimes IO errors should be treated as user error (such as if the user entered an invalid filename), but then they should be validated in those cases, an different error thrown/shown.
    // [MappedException(typeof(OutOfMemoryException))] // subclass of SystemException
    public class SystemErrorDialogType : ErrorDialogType
    {
        public SystemErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options, description)
        {
        }

        public override Color DefaultColor => Color.DarkGray;  // Lighter than Gray

        // Suggested icon: Like 'Error' icon but with a cogwheel (or other icon to indicate system internals etc.) added.
    }

    /// <summary>
    /// An error due to something internal to the system, (apparently) not (directly) caused by the user, nor the environment,
    /// e.g. an invalid state that must be the result of a bug, or an assertion failure.
    /// (An error that should never happen.)
    /// </summary>
    [MappedException(typeof(InternalErrorException))]     // explicitly an internal error.
    [MappedException(typeof(AccessViolationException))]
    // Some of the following might be caused indirectly by user input, but then they should be validated before causing these exceptions,
    // so these exceptions still indicate an internal failure (even if it is just poor validation):
    [MappedException(typeof(IndexOutOfRangeException))]
    [MappedException(typeof(NullReferenceException))]
    [MappedException(typeof(ArgumentException))]  // could be treated as either user error (based on the assumption that the argument came from the user originally) or internal error.
    public class InternalErrorDialogType : ErrorDialogType
    {
        public InternalErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options, description)
        {
        }

        public override Color DefaultColor => Color.FromArgb(50,50,50);
        public override Color DefaultColorMuted => Color.Gray;  // Dark gray

        // Suggested icon: Like error but with a black background (white 'X' on black background (circle or square)).
    }


    /// <summary>
    /// Possible return values from message dialogs (also used as the IDs of the options).
    /// </summary>
    public static class MessageDialogResponse
    {
        public const string Yes = "Yes";
        public const string No = "No";
        /// <summary>
        /// A 'Cancel' button: 
        /// Cancels an action.
        /// </summary>
        public const string Cancel = "Cancel";
        /// <summary>
        /// An 'Ok' button:
        /// Commonly used when there is only one button,
        /// and may confirm an action.
        /// </summary>
        public const string Ok = "Ok";
        public const string Abort = "Abort";
        public const string Retry = "Retry";

        //| Could use instances of a custom class instead.
        //| That would be more type-safe and avoid the possibility of two instances unintentionally being defined with the same value.
        //| But increases complexity.
    }


    /*
    /// <summary>
    /// Category of message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// No type specified. Typically displayed with no icon etc. indicating the type of message.
        /// </summary>
        None  = 0,

        /// <summary>
        /// Information that is not an error or warning.
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        Informational = 1000,

        /// <summary>
        /// Asks the user to confirm an action.
        /// Default options are "Yes", "No" and "Cancel".
        /// </summary>
        Confirmation = 2000,

        // An error with a 'Retry' option:
        // Could be a separate type, but it could be more useful to be able to classify the category of error
        // (one of the values below), and specify the buttons separately.

        /// <summary>
        /// A warning.
        /// By default, has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        Warning = 3000,

        /// <summary>
        /// A warning that is more severe than the <see cref="Warning"/> option.
        /// This may have a different UI style to draw attention to it, and/or different UI behaviours
        /// (e.g. a delay of 1-2 seconds before allowing the user to respond, so that a user automatically pressing a key to dismiss when they couldn't possibly have read it won't be accepted).
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        SevereWarning = 3500,

        /// <summary>
        /// An error.
        /// By default, has only one option (typically called "Ok") - to dismiss it (the same applies to all type with names ending with "Error").
        /// </summary>
        Error = 4000,

        /// <summary>
        /// An error caused by a user, such as user input that failed validation.
        /// </summary>
        UserError = 4200,

        /// <summary>
        /// An system error, e.g. disc full, network connection failed.
        /// </summary>
        SystemError = 4600,

        /// <summary>
        /// An error that should never happen. An internal failure, such as an assertion failure.
        /// </summary>
        InternalError = 4800
    }
    */
}
