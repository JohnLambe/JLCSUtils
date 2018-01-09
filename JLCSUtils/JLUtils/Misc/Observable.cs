using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Wraps a value and fires an event when it is modified.
    /// </summary>
    /// <typeparam name="T">The type of the observable value.</typeparam>
    public class Observable<T> : VarBaseExplicitCast<T>
    {
        public Observable(T value) : base(value)
        {
        }

        public override T Value
        {
            get { return base.Value; }
            set
            {
                if (!ObjectUtil.CompareEqual(value, base.Value))
                {
                    Modify(new ValueChangedEventArgs<T>(base.Value, value));
                    base.Value = value;
                }
            }
        }

        /// <summary>
        /// Update the value and fire the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Modify(ValueChangedEventArgs<T> args)
        {
            _value = args.NewValue;
            ValueChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Fired when <see cref="VarBase{T}.Value"/> changes (after the change).
        /// </summary>
        public virtual event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        /// <summary>
        /// Cast a value to this.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Observable<T>(T value)
        {
            return new CancellableObservable<T>(value);
        }
    }


    /// <summary>
    /// As <see cref="Observable{T}"/>, but also fires an event before any change, allowing handlers to prevent the change.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CancellableObservable<T> : Observable<T>
    {
        public CancellableObservable(T value) : base(value)
        {
        }

        public override T Value
        {
            get { return base.Value; }
            set
            {
                if (!ObjectUtil.CompareEqual(value,base.Value))
                {
                    var args = new ValueChangedEventArgs<T>(base.Value, value);
                    ValueChanging?.Invoke(this, args);
                    if (!args.Cancel)
                    {
                        Modify(args);
                    }
                }
            }
        }

        /// <summary>
        /// Fired when <see cref="Value"/> is about to change.
        /// Handlers can cancel the change and/or modify the new value.
        /// </summary>
        public virtual event EventHandler<ValueChangedEventArgs<T>> ValueChanging;

        /// <summary>
        /// Cast a value to this.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator CancellableObservable<T>(T value)
        {
            return new CancellableObservable<T>(value);
        }
    }

}
