using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Interface to a View Binder from other parts of the framework (currently from the Model Binder).
    /// </summary>
    public interface IViewBinder
    {
        /// <summary>
        /// Notifies the View Binder of the stage of the validation process.
        /// </summary>
        /// <param name="stage"></param>
        void NotifyValidationStage(ValidationStage stage);

        /// <inheritdoc cref="ViewBinderBase{TControl}.RefreshView(TControl)"/>
        void RefreshView();

        void InvalidateView();
    }


    public interface IControlAdaptor<TControl>
    {
        /// <summary>
        /// Return the collection of children of the given control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        IEnumerable<TControl> GetChildren(TControl control);

        /// <summary>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="testParent"></param>
        /// <returns>true iff <paramref name="control"/> is a direct or indirect child of <paramref name="testParent"/>.</returns>
        bool IsInControl(TControl control, TControl testParent);

        /// <summary>
        /// </summary>
        /// <param name="control"></param>
        /// <returns>The name of the control as used in code.</returns>
        string GetName(TControl control);
    }


    /*
        public interface IUiFrameworkAdaptor<TControl, TView>
        {
            IEnumerable<TControl> GetChildren(TControl control);

            bool IsInControl(TControl control, TControl testParent);

            void SetViewTitle(TView view, string title);
        }
    */

    /// <summary>
    /// Event that notifies that a key (or keystroke) was pressed.
    /// </summary>
    public class KeyboardKeyEventArgs : CancelEventArgs
    {
        public KeyboardKeyEventArgs()
        {
        }

        public KeyboardKeyEventArgs(KeyboardKey key)
        {
            this.Key = key;
        }

        /// <summary>
        /// The keystroke pressed.
        /// </summary>
        public virtual KeyboardKey Key { get; set; }

        /// <summary>
        /// Handlers set this to true if they handle this event.
        /// <para>Same as <see cref="CancelEventArgs.Cancel"/>.</para>
        /// </summary>
        public bool Handled { get { return Cancel; } set { Cancel = value; } }
        //| Not virtual because overriding this would make it inconsistent wih 'Cancel'.
        //| 'Handled' is a better name for the 'Cancelled' flag in this case.
        //| We could use our own base class, and also make this property virtual.
        //| Using CancelEventArgs may be conventient for interoperability (e.g. this event may be useful where WinForms already uses a CancelEventHandler).
    }

    /// <summary>
    /// A handler that receives key events.
    /// <para>
    /// <see cref="ViewBinderBase{TControl}"/> calls this on bound controls that implement it.
    /// </para>
    /// </summary>
    public interface IKeyboardKeyHandler
    {
        void NotifyKeyDown(KeyboardKeyEventArgs args);
    }

}
