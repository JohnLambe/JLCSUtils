using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Menu;

namespace MvpFramework
{
    /// <summary>
    /// Interface to a system for showing message dialogs etc.
    /// </summary>
    public interface IMessageDialog
    {
        /// <summary>
        /// Show a message (dialog etc.).
        /// </summary>
        /// <param name="parameters">Details of the message to be shown.</param>
        /// <returns>Indicates which option was chosen, when the message has multiple options (e.g. buttons).</returns>
        object ShowMessage(MessageDialogParameters parameters);
            //| Create custom type for response ?
    }


    /// <summary>
    /// Model-level parameters for a message dialog (or any UI item that displays a message).
    /// </summary>
    public class MessageDialogParameters
    {
        /// <summary>
        /// Title of the message dialog. (For display).
        /// May be null. (If it is, it is recommended that the UI hide the title bar of the window etc.).
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Main message for display.
        /// </summary>
        public virtual string Message { get; set; }

        public virtual MessageType MessageType { get; set; }
            //| Inferred from MessageTypeId ?

        /// <summary>
        /// Hierarchical ID of the type of message (parts separated by "/").
        /// Could have UI styles or defaults for other properties mapped to it.
        /// May be null.
        /// </summary>
        public virtual string MessageTypeId { get; set; }

        /// <summary>
        /// Identifier of an icon indicating the type of message or error.
        /// May be null, for a default determined by <see cref="MessageType"/>.
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// Identifier of an image related to the message (rather than the type (error, warning, etc.)).
        /// (It might be displayed in the background of the dialog.)
        /// </summary>
        public virtual string MessageImage { get; set; }

        /// <summary>
        /// What type of UI to use to show the message.
        /// </summary>
        public virtual MessageDisplayType DisplayType { get; set; } = MessageDisplayType.Default;

        /// <summary>
        /// The exception which lead to this message.
        /// May be null.
        /// </summary>
        public virtual Exception Exception { get; set; }

        /// <summary>
        /// Low-level details (not shown to the user, except possibly by their requesting it by a UI interaction
        /// (expanding a panel, etc.)).
        /// May be null.
        /// </summary>
        public virtual string DiagnosticMessage { get; set; }

        /// <summary>
        /// ID unique to an error type and a place where it can occur.
        /// May be null.
        /// <para>A hierarchical ID (parts separated by "/") is recommended.</para>
        /// <para>This can be used in analysing log files for occurrences of the same error condition, etc.,
        /// or potentially for applying UI styles.</para>
        /// </summary>
        public virtual string InstanceId { get; set; }

        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// null for default based on MessageType.
        /// </summary>
        public virtual OptionCollection Options { get; set; }

        /// <summary>
        /// Event that is fired when an option is chosen.
        /// (For use with modeless dialogs, but fired whether modal or not).
        /// </summary>
        public virtual event RespondedDelegate Responded;

        /// <summary>
        /// Delegate to be fired when the user responds to a message.
        /// </summary>
        /// <param name="dialog">The <see cref="MessageDialogParameters"/> to which the response relates.</param>
        /// <param name="messageDialogResult">The chosen option - the same as the return value from <see cref="IMessageDialog.ShowMessage(MessageDialogParameters)"/>.</param>
        public delegate void RespondedDelegate (MessageDialogParameters dialog, object messageDialogResult);
    }

    /*
/// <summary>
/// Details of an option that may be available in a message dialog
/// (typically displayed as a button).
/// </summary>
public class DialogOption : MvpFramework.Menu.MenuItemModel
{
    public MenuItemModel(Dictionary<string, Menu.MenuItemModel> allItems, string id) : base(allItems, id)
    {
    }

   /// <summary>
   /// ID of the option, unique within a set of options.
   /// </summary>
   // Same type as return value of IMessageDialog.ShowMessage.
   public virtual object Id { get; set; }

   /// <summary>
   /// The text displayed in the UI (on the button etc.).
   /// </summary>
   public virtual string Caption { get; set; }  //| Rename "DisplayName" ?

   /// <summary>
   /// Identifier of an icon on the UI element (e.g. button) that displays this option.
   /// May be null (to include no icon on the button).
   /// </summary>
   public virtual string IconId { get; set; }

   /// <summary>
   /// Accelerator key to choose this option, if any.
   /// </summary>
   public virtual KeyboardKey AcceleratorKey { get; set; } = KeyboardKey.None;
   //TODO: Make Char Or rename.

   /// <summary>
   /// Sorting order. (Used when options are added to an existing collection).
   /// </summary>
   public int Order { get; set; }

    /// <summary>
    /// True iff this is the default option.
    /// Only one (or zero) option in a set should have this set to true.
    /// </summary>
    public virtual bool IsDefault { get; set; }
    }
*/

    public class OptionCollection
    {
        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// null for default based on MessageType.
        /// </summary>
        public virtual IEnumerable<MenuItemModel> Options { get; set; }

        /// <summary>
        /// The default option.
        /// null if there is no default.
        /// </summary>
        public virtual MenuItemModel Default
            => Options.FirstOrDefault(o => o.IsDefault);
    }

    public class MutableOptionCollection : OptionCollection
    {
        public virtual void AddOption(MenuItemModel option)
        {
            _options.Add(option);
            _options = _options.OrderBy(o => o.Order).ToList();
        }

        public virtual bool RemoveOption(MenuItemModel option)
        {
            return _options.Remove(option);
        }

        public override IEnumerable<MenuItemModel> Options
        {
            get { return _options; }
            set { _options = value.ToList(); }
        }

        protected List<MenuItemModel> _options { get; set; }
    }


    /// <summary>
    /// Category of message.
    /// </summary>
    public enum MessageType
    {
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

        /// <summary>
        /// A warning.
        /// By default has only one option (typically called "Ok") - to dismiss it.
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
        /// By default has only one option (typically called "Ok") - to dismiss it.
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


    /// <summary>
    /// Specifies how (what type of dialog etc.) a message is displayed.
    /// </summary>
    public enum MessageDisplayType
    {
        /// <summary>
        /// To use a default based on other properties of the MessageDialogParameters.
        /// </summary>
        Default = 0,
        //| Could use Nullable<MessageDisplayType> instead ?

        /// <summary>
        /// Don't display. It may be logged.
        /// </summary>
        None,

        /// <summary>
        /// A message appears modelessly and dissappears after a certain time if the user does not explicity dismiss it.
        /// (Like an Android "Toast" message).
        /// </summary>
        Temporary,

        /// <summary>
        /// Show a modeless dialog or message on the bottom or corner of a page/screen/window etc.
        /// </summary>
        NonModal,

        /// <summary>
        /// Show a modal dialog.
        /// </summary>
        Modal,
    }


    public class OptionCollectionBuilder
    {
        /// <summary>
        /// Build a list of options from handlers on a given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual MutableOptionCollection Build(object target, string filter)
        {
            /*
            type.GetMethods()
                .Where(m => m.GetCustomAttribute<MvpHandlerAttribute>()?.Filter?.Contains(filter) ?? false)
                .OrderBy(m => m.GetCustomAttribute<MvpHandlerAttribute>()?.Order);
                */
            var options = new Dictionary<string,Menu.MenuItemModel>();
            foreach(var method in target.GetType().GetMethods())
            {
                var attrib = method.GetCustomAttribute<MvpHandlerAttribute>();
                //TODO: Support multiple handlers on same method.
                if (attrib != null 
                    && ((filter == null) || (attrib.Filter?.Contains(filter) ?? false) )
                    )
                {
                    var option = new MenuItemModel(options, attrib.Name)
                    {
                        DisplayName = attrib.DisplayName,
                        HotKey = attrib.HotKey,
                        IconId = attrib.IconId,
                        IsDefault = attrib.IsDefault,
                        Order = attrib.Order,
                    };
                    option.Invoked += item => method.Invoke(null, new object[] { });
                    options[attrib.Name] = option;
                }
            }
            var optionSet = new MutableOptionCollection()
            {
                Options = (IEnumerable<MenuItemModel>)options.Values.OrderBy(o => o.Order)
            };
            return optionSet;
        }

    }

}
