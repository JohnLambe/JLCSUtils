using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Menu;
using JohnLambe.Util.Text;
using JohnLambe.Util.Misc;
using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Types;

namespace MvpFramework.Dialog
{

    /// <summary>
    /// Model-level parameters for a message dialog (or any UI item that displays a message).
    /// <para>
    /// Icon IDs (provided by some properties) are resolved by the Message Dialog Service, typically by delegating to an Icon Repository.
    /// A value of "" explicitly specifies that there should be no icon, even if there would be a default one if null was supplied.
    /// </para>
    /// </summary>
    public class MessageDialogModel<TResult> : IMessageDialogModel<TResult>
    {
        /// <summary>
        /// Conventional suffix of class names of subclasses of this.
        /// <para>
        /// This is used when mapping from <see cref="MessageDialogType"/> subclasses to subclasses of this - their suffix is replaced with this,
        /// and their namespace is changed.
        /// </para>
        /// </summary>
        public const string ClassNameSuffix = "Dialog";

        public MessageDialogModel()
        {
            MessageTypeId = GetType().FullName;
        }

        /// <inheritdoc cref="IMessageDialogModel.InstanceId"/>
        public virtual string InstanceId { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.Title"/>
        public virtual string Title { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.Message"/>
        [Nullable("The exception message is returned if this is null. If both are null, null is returned.")]
        public virtual string Message
        {
            get { return _message ?? ExceptionUtil.ExtractException(Exception)?.Message; }
            set { _message = value; }
        }
        private string _message;

        /// <inheritdoc cref="IMessageDialogModel.MessageType"/>
        public virtual MessageDialogType MessageType { get; set; } = MessageDialogType.Informational;
        //| Inferred from MessageTypeId ?

        /// <inheritdoc cref="IMessageDialogModel.MessageTypeId"/>
        public virtual string MessageTypeId { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.Icon"/>
        [IconId]
        public virtual string Icon { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.MessageImage"/>
        [IconId]
        public virtual string MessageImage { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.DisplayType"/>
        public virtual MessageDisplayType DisplayType { get; set; } = MessageDisplayType.Default;

        /// <inheritdoc cref="IMessageDialogModel.Exception"/>
        public virtual Exception Exception { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.DiagnosticMessage"/>
        public virtual string DiagnosticMessage { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.Options"/>
        public virtual IOptionCollection Options { get; set; }

        /// <summary>
        /// The minimum log detail level at which this message should be logged.
        /// </summary>
        public virtual int LogLevel { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.InterceptFlags"/>
        public virtual MessageDialogInterceptFlags InterceptFlags => MessageDialogInterceptFlags.Default;


        /// <summary>
        /// A human-readable description of this type of message.
        /// Can be used for showing configuration settings relating to it, etc.
        /// </summary>
        public virtual string Description { get; set; }

        #region Dialog grouping

        /// <inheritdoc cref="IMessageDialogModel.GroupId"/>
        public virtual string GroupId { get; set; }

        /// <inheritdoc cref="IMessageDialogModel.CommonMessage"/>
        public virtual string CommonMessage { get; set; }

        #endregion

        /// <inheritdoc cref="IMessageDialogModel.Responded"/>
        public virtual event MessageDialogRespondedDelegate Responded;

        /// <inheritdoc cref="IMessageDialogModel.FireResponded"/>
        public virtual void FireResponded(object messageDialogResult)
        {
            Responded?.Invoke(this, new MessageDialogRespondedEventArgs(messageDialogResult));
        }
    }


    /// <summary>
    /// Specifies how (what type of dialog etc.) a message is displayed.
    /// </summary>
    public class MessageDisplayType
    {
        /// <summary>
        /// To use a default based on other properties of the <see cref="MessageDialogModel{T}"/>.
        /// </summary>
        public static readonly MessageDisplayType Default = new MessageDisplayType();
        //| Could use Nullable<MessageDisplayType> instead ?

        /// <summary>
        /// Don't display. It may be logged.
        /// </summary>
        public static readonly MessageDisplayType None = new MessageDisplayType();

        /// <summary>
        /// A message appears modelessly and dissappears after a certain time if the user does not explicity dismiss it.
        /// (Like an Android "Toast" message).
        /// </summary>
        public static readonly MessageDisplayType Temporary = new MessageDisplayType();

        /// <summary>
        /// Show a modeless dialog or message, typically on the bottom or corner of a page/screen/window etc.
        /// </summary>
        public static readonly MessageDisplayType NonModal = new MessageDisplayType();

        /// <summary>
        /// Show a modal dialog.
        /// </summary>
        public static readonly MessageDisplayType Modal = new MessageDisplayType();
    }

}
