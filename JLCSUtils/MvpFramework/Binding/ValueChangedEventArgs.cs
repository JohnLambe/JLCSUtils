using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Details of a property of a model changing.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the property.</typeparam>
    public class ValueChangedEventArgs<TModel,TValue> : CancelEventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="model"><see cref="Model"/></param>
        /// <param name="property"><see cref="Property"/></param>
        /// <param name="oldValue"><see cref="OldValue"/></param>
        /// <param name="newValue"><see cref="NewValue"/></param>
        public ValueChangedEventArgs(TModel model, string property, TValue oldValue, TValue newValue)
        {
            this.Model = model;
            this.Property = property;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="oldValue"><see cref="OldValue"/></param>
        /// <param name="newValue"><see cref="NewValue"/></param>
        public ValueChangedEventArgs(TValue oldValue, TValue newValue) : this(default(TModel),null,oldValue,newValue)
        {
        }

        /// <summary>
        /// The model on which the property is changed.
        /// </summary>
        /// <seealso cref="Property"/>
        public virtual TModel Model { get; }
        /// <summary>
        /// The name of the property (possibly indirectly) of <see cref="Model"/> being changed,
        /// in the form used by <see cref="GetNameDelegate"/>.
        /// </summary>
        public virtual string Property { get; }
        /// <summary>
        /// The value that the property had before the change.
        /// </summary>
        public virtual TValue OldValue { get; }
        /// <summary>
        /// The value that the property now has (after the change).
        /// </summary>
        public virtual TValue NewValue { get; }

        //| State: About to change or already changed ?
        //| Subclass CancelEvent if not already changed ?
    }
}
