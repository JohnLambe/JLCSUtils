﻿using JohnLambe.Util;
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
    // A colour (or colour scheme) for the first one may be preferable to having two icons in the same dialog
    // to avoid it looing cluttered (though it could have one icon to the right of the window and the other to the left,
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
    public class MessageDialogType
    {
        public MessageDialogType(string id, IOptionCollection options, string description = null)
        {
            if (id == null)
                id = GetType().Name.RemoveSuffix("DialogType");

            Id = id;
            Icon = id;
            Description = description ?? CaptionUtils.PascalCaseToCaption(id);
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

    public class ErrorDialogType : MessageDialogType
    {
        public ErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options ?? Options_Ok, description)
        {
        }

        public override Color DefaultColor => Color.LightGray;

        // Suggested icon: white 'X' on red circle; circle could be dark gray if red is reserved for serious warnings.
    }

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

    public class SystemErrorDialogType : ErrorDialogType
    {
        public SystemErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options, description)
        {
        }

        public override Color DefaultColor => Color.DarkGray;  // Lighter than Gray

        // Suggested icon: Like 'Error' icon but with a cogwheel (or other icon to indicate system internals etc.) added
    }

    public class InternalErrorDialogType : ErrorDialogType
    {
        public InternalErrorDialogType(string id = null, IOptionCollection options = null, string description = null)
            : base(id, options, description)
        {
        }

        public override Color DefaultColor => Color.FromArgb(50,50,50);
        public override Color DefaultColorMuted => Color.Gray;  // Dark gray

        // Suggested icon: white 'X' on black circle
    }


    /// <summary>
    /// Possible return values from message dialogs (also used as the IDs of the options).
    /// </summary>
    public static class MessageDialogResponse
    {
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Cancel = "Cancel";
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
