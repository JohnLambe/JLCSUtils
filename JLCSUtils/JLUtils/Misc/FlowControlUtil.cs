using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    public static class FlowControlUtil
    {
        public static TypeSwitchContext<TSelector, TReturn> TypeSwitch<TSelector, TReturn>(TSelector selector)
        {
            return new TypeSwitchContext<TSelector, TReturn>(selector);
        }

        public static TypeSwitchContext<TSelector> TypeSwitch<TSelector>(TSelector selector)
        {
            return new TypeSwitchContext<TSelector>(selector);
        }

        /// <summary>
        /// Type returned to support fluent methods.
        /// </summary>
        /// <typeparam name="TSelector"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        public class TypeSwitchContext<TSelector, TReturn>
        {
            internal TypeSwitchContext(TSelector selector)
            {
                Selector = selector;
            }

            /// <summary>
            /// The value passed, which determines what branch(es) to execute.
            /// </summary>
            public TSelector Selector { get; }

            public TReturn ReturnValue
            {
                get
                {
                    if (HasReturnValue)
                        return _returnValue;
                    else
                        throw new InvalidOperationException("TypeSwitch: Return value not set (Return type: " + typeof(TReturn) + "; Selector: " + Selector + ")");
                }
                protected set
                {
                    _returnValue = value;
                    HasReturnValue = true;
                }
            }
            protected TReturn _returnValue;

            /// <summary>
            /// true iff <see cref="ReturnValue"/> has been assigned.
            /// </summary>
            /// <remarks>
            /// <see cref="ReturnValue"/> may or may not be nullable, so we couldn't use <see cref="Nullable{TReturn}"/>.
            /// </remarks>
            public bool HasReturnValue { get; protected set; } = false;

            /// <summary>
            /// True if no more cases should run.
            /// </summary>
            public bool Handled { get; protected set; } = false;

            public virtual TypeSwitchContext<TSelector, TReturn> Case<TCaseType>(Func<TCaseType, TReturn> del)
                where TCaseType : TSelector
            {
                if (!Handled)
                    if (Selector is TCaseType)
                    {
                        ReturnValue = del((TCaseType)Selector);
                        Handled = true;
                    }
                return this;
            }

            /// <summary>
            /// </summary>
            /// <typeparam name="TCaseType"></typeparam>
            /// <param name="condition">This case is run only if this evaluates to true.
            /// (This delegate is executed only if the type matches.)</param>
            /// <param name="del"></param>
            /// <returns></returns>
            public virtual TypeSwitchContext<TSelector, TReturn> Case<TCaseType>(Func<TCaseType,bool> condition, Func<TCaseType, TReturn> del)
                where TCaseType : TSelector
            {
                if (!Handled)
                {
                    if (Selector is TCaseType)
                    {
                        if (condition((TCaseType)Selector))
                        {
                            ReturnValue = del((TCaseType)Selector);
                            Handled = true;
                        }
                    }
                }
                return this;
            }

            public virtual TReturn Default(Func<TSelector, TReturn> del)
            {
                if (!Handled)
                {
                    ReturnValue = del(Selector);
                    Handled = true;
                }
                return ReturnValue;
            }

            public virtual TReturn Default(TReturn defaultReturnValue)
            {
                if (!Handled)
                {
                    ReturnValue = defaultReturnValue;
                    Handled = true;
                }
                return ReturnValue;
            }

            public delegate void TypeSwitchDelegate<T>(T selector, out bool breakCase);
        }

        public class TypeSwitchContext<TSelector> : TypeSwitchContext<TSelector, object>
        {
            public TypeSwitchContext(TSelector selector) : base(selector)
            {
            }

            public virtual TypeSwitchContext<TSelector, object> Case<TCaseType>(VoidDelegate<TCaseType> del)
                where TCaseType : TSelector
            {
                if (!Handled)
                    if (Selector is TCaseType)
                    {
                        del((TCaseType)Selector);
                        Handled = true;
                    }
                return this;
            }

            public virtual TypeSwitchContext<TSelector, object> Case<TCaseType>(TypeSwitchDelegate<TCaseType> del)
                where TCaseType : TSelector
            {
                if (!Handled)
                    if (Selector is TCaseType)
                    {
                        bool handled;
                        del((TCaseType)Selector, out handled);
                        Handled = handled;
                    }
                return this;
            }

            /// <summary>
            /// </summary>
            /// <typeparam name="TCaseType"></typeparam>
            /// <param name="condition">This case is run only if this evaluates to true.
            /// (This delegate is executed only if the type matches.)</param>
            /// <param name="del"></param>
            /// <returns></returns>
            public virtual TypeSwitchContext<TSelector> Case<TCaseType>(Func<TCaseType, bool> condition, VoidDelegate<TCaseType> del)
                where TCaseType : TSelector
            {
                if (!Handled)
                {
                    if (Selector is TCaseType)
                    {
                        if (condition((TCaseType)Selector))
                        {
                            del((TCaseType)Selector);
                            Handled = true;
                        }
                    }
                }
                return this;
            }

            public virtual void Default(VoidDelegate<TSelector> del)
            {
                if (!Handled)
                {
                    del(Selector);
                    Handled = true;
                }
            }
        }

    }
}