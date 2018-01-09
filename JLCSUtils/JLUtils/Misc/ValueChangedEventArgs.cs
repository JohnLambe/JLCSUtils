using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using System;
using System.ComponentModel;

namespace JohnLambe.Util.Misc
{
    public class ValueChangedEventArgs<TValue> : CancelEventArgs
    {
        public ValueChangedEventArgs(TValue oldValue, TValue newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        /// <summary>
        /// The value that the property had before the change.
        /// </summary>
        [Nullable]
        public virtual TValue OldValue { get; }

        /// <summary>
        /// The value that the property now has (after the change).
        /// Handlers can modify this, and the modified value will be written back (if supported) if not cancelled.
        /// </summary>
        [Nullable]
        public virtual TValue NewValue { get; set; }
    }


    /// <summary>
    /// Details of a property of a model changing.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the property.</typeparam>
    public class ValueChangedEventArgs<TModel, TValue> : ValueChangedEventArgs<TValue>
    {
        /// <summary>
        /// </summary>
        /// <param name="model"><see cref="Model"/></param>
        /// <param name="propertyName"><see cref="PropertyName"/></param>
        /// <param name="oldValue"><see cref="ValueChangedEventArgs{TValue}.OldValue"/></param>
        /// <param name="newValue"><see cref="ValueChangedEventArgs{TValue}.NewValue"/></param>
        /// <param name="originalArgs"><see cref="OriginalArgs"/></param>
        /// <param name="stage"><see cref="Stage"/></param>
        public ValueChangedEventArgs(TModel model, string propertyName, TValue oldValue, TValue newValue, ValidationStage stage = ValidationStage.BeforeChange, EventArgs originalArgs = null)
            : base(oldValue,newValue)
        {
            this.Model = model;
            this.PropertyName = propertyName;
            this.Stage = stage;
            this.OriginalArgs = originalArgs;
        }

        /// <summary>
        /// </summary>
        /// <param name="oldValue"><see cref="ValueChangedEventArgs{TValue}.OldValue"/></param>
        /// <param name="newValue"><see cref="ValueChangedEventArgs{TValue}.NewValue"/></param>
        public ValueChangedEventArgs(TValue oldValue, TValue newValue) : this(default(TModel), null, oldValue, newValue)
        {
        }

        /// <summary>
        /// The model on which the property is changed.
        /// </summary>
        /// <seealso cref="PropertyName"/>
        public virtual TModel Model { get; }

        /// <summary>
        /// The name of the property (possibly indirectly) of <see cref="Model"/> being changed,
        /// in the form used by <see cref="MvpFramework.Binding.GetNameDelegate"/>.
        /// </summary>
        public virtual string PropertyName { get; }

        /// <summary>
        /// The arguments of an underlying event that caused this to be raised.
        /// </summary>
        [Nullable]
        public virtual EventArgs OriginalArgs { get; }

        /// <summary>
        /// The stage of the validation process.
        /// </summary>
        public virtual ValidationStage Stage { get; set; }

        /// <summary>
        /// Set to true to cause the whole View to be refreshed from the model, after the validation events.
        /// This is set back to false when the view is refreshed.
        /// The refresh happens after the Validating and Validated events (if this is true).
        /// </summary>
        public virtual bool InvalidateView { get; set; } = false;

        /// <summary>
        /// Set to true to prevent the change from happening.
        /// <para>
        /// This returns true if either <see cref="CancelEventArgs.Cancel"/> of this instance or of <see cref="OriginalArgs"/> (if it is a <see cref="CancelEventArgs"/>) is true,
        /// and setting it sets both.
        /// </para>
        /// </summary>
        /// <remarks>It is recommended to use this instead of <see cref="CancelEventArgs.Cancel"/>,
        /// so handlers can pass <see cref="OriginalArgs"/> to other code that may set <see cref="CancelEventArgs.Cancel"/>.
        /// This property is added because <see cref="CancelEventArgs.Cancel"/> is not virtual.
        /// </remarks>
        public virtual bool IsCancelled
        {
            get { return Cancel || ((OriginalArgs as CancelEventArgs)?.Cancel ?? false); }
            set
            {
                Cancel = value;
                if ((OriginalArgs as CancelEventArgs) != null)
                    ((CancelEventArgs)OriginalArgs).Cancel = value;
            }
        }
        //| Should this and InvalidateView allow changing from true to false?
    }


    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs<object, object> args);
}
