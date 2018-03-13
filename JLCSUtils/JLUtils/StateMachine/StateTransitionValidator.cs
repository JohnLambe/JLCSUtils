using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.StateMachine
{
    /// <summary>
    /// Validates transitions between states.
    /// Can be used as a base class for state machines, or an instance can be used to hold a state and validate changes to it.
    /// </summary>
    /// <typeparam name="TState">The type that represents a state.
    /// If this implements <see cref="IState{TState, TModel}"/> (with the same type arguments as this class),
    /// it will be called to validate state transitions, in addition to the transitions defined in the instance of this class.
    /// </typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>
    /// A problem that this class is designed to solve is that validation of state transition can be decribed in a state diagram,
    /// but without something like this, the code corresponding to that diagram might not be structured similarly to the diagram.
    /// This allows writing a line of code for each state transition (line) in the diagram.
    /// </remarks>
    public class StateTransitionValidator<TState, TModel>
        //where TState : IState<TState>
        //where TState : IEquatable<TState>
    {
        public StateTransitionValidator(TState initialState = default(TState))
        {
            _state = initialState;
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public virtual TState State
        {
            get
            {
                CheckInvariant();
                return _state;
            }
            set
            {
                CheckInvariant();

                if (!ObjectUtil.CompareEqual(value, _state))   // do nothing the value is the same as the current one
                {
                    var args = new StateChangeEventArgs();

                    ValidateTransition(_state, value);

                    BeforeStateChange?.Invoke(this, args);
                    if (!args.Cancel)
                    {
                        _state = value;

                        StateChanged?.Invoke(this, args);

                        CheckInvariant();
                    }
                }
            }
        }
        protected TState _state;

/*
        protected virtual void CheckInvariant(TState state)
        {
            if (!CheckInvariant(state))
                throw new InvalidOperationException("StateMachine: Invariant failed: " + state);
        }
*/

        /// <summary>
        /// Throws an exception if 
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckInvariant()
        {
            ValidateInvariant(_state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <exception cref="InvalidOperationException"/>
        protected virtual void ValidateTransition(TState oldState, TState newState)
        {
            if(Transitions != null)
            {
                bool valid = false;
                foreach(var transition in Transitions.Where(t => (t.FromAny || ObjectUtil.CompareEqual(t.FromState,oldState))
                    && (t.ToAny || ObjectUtil.CompareEqual(t.ToState, newState))) )   // all defined transitions between the given states (before checking the guard condition)
                {
                    if (transition.Validate(Model))
                    {
                        valid = true;
                        break;     // valid
                    }
                }
                if(!valid)
                    throw new InvalidOperationException("Invalid state transition: from " + oldState + " to " + newState);
            }
            ValidateLeave(_state, newState);
            ValidateEnter(_state, newState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <exception cref="InvalidOperationException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ValidateLeave(TState oldState, TState newState)
        {
            if (!((oldState as IState<TState,TModel>)?.ValidateBeforeLeave(Model,newState) ?? true))
                throw new InvalidOperationException("Invalid state transition: Can't leave " + oldState + " for " + newState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <exception cref="InvalidOperationException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ValidateEnter(TState oldState, TState newState)
        {
            if(!((newState as IState<TState, TModel>)?.ValidateBeforeEnter(Model,oldState) ?? true))
                throw new InvalidOperationException("Invalid state transition: Can't enter " + newState + " from " + oldState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="InvalidOperationException"/>
        public virtual void ValidateInvariant(TState state)
        {
            if (!((state as IState<TState, TModel>)?.ValidateInvariant(Model) ?? true))
                throw new InvalidOperationException("Invariant violated: State " + state);
        }

        /// <summary>
        /// Allowed transitions, or null to allow all.
        /// There may be multiple transitions between the same states. If any one of them is valid (its guard condition passes), the transition is valid.
        /// </summary>
        public IList<StateTransition> Transitions { get; set; }

        /// <summary>
        /// Fired before changing state (not after any rejected (by validation) attempt to change state).
        /// This can prevent the change.
        /// </summary>
        public event EventHandler<StateChangeEventArgs> BeforeStateChange;

        /// <summary>
        /// Fired after changing state (not after any rejected (by validation or the <see cref="BeforeStateChange"/> handler) attempt to change state).
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChanged;

        public virtual TModel Model { get; set; }

        /// <summary>
        /// Defines a transition between two states.
        /// This corresponds to a line in a state diagram.
        /// </summary>
        public class StateTransition
        {
            public StateTransition(TState fromState, TState toState, Func<TModel,bool> validator = null)
            {
                this.FromState = fromState;
                this.ToState = toState;
                this.Validator = validator;
            }

            public StateTransition(bool fromAny, TState fromState, bool toAny, TState toState, Func<TModel, TState, TState, bool> validator = null)
            {
//                Debug.Assert((fromAny && fromState != default(TState)) || (toAny && toState != default(TState)));

                this.FromState = fromState;
                this.FromAny = fromAny;
                this.ToState = toState;
                this.ToAny = toAny;
                this.ValidatorEx = validator;
            }

            public static StateTransition CreateFromAny(TState toState, Func<TModel, TState, TState, bool> validator = null)
            {
                return new StateTransition(true, default(TState), false, toState, validator);
            }

            public static StateTransition CreatToAny(TState fromState, Func<TModel, TState, TState, bool> validator = null)
            {
                return new StateTransition(false, fromState, true, default(TState), validator);
            }

            /// <summary>
            /// Check the guard condition for this transition (check that it is valid to do this transition at this time).
            /// (Returns true if there is no guard condition.)
            /// </summary>
            /// <param name="model"></param>
            /// <returns>true if it valid to do this transition, with the given state of the model.</returns>
            public virtual bool Validate(TModel model)
            {
                return Validator?.Invoke(model) ?? ValidatorEx?.Invoke(model, default(TState), default(TState)) ?? true;
            }

            public virtual bool Validate(TModel model, TState fromState, TState toState)
            {
                return ValidatorEx?.Invoke(model, fromState, toState) ?? Validator?.Invoke(model) ?? false;
            }

            public virtual TState FromState { get; }
            public virtual TState ToState { get; }

            /// <summary>
            /// If true, it allows transition from any state.
            /// </summary>
            public virtual bool FromAny { get; }

            /// <summary>
            /// If true, it allows transition to any state.
            /// </summary>
            public virtual bool ToAny { get; }

            protected Func<TModel, bool> Validator { get; }
            protected Func<TModel, TState, TState, bool> ValidatorEx { get; }
        }
    }

    public interface IState<in TState, TModel>
    {
        /// <summary>
        /// Condition that must always be true while in this state.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ValidateInvariant(TModel model);

        /// <summary>
        /// Validate a guard condition that must be true before leaving this state.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        bool ValidateBeforeLeave(TModel model, TState newState);

        /// <summary>
        /// Validate a guard condition that must be true before entering this state.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oldState"></param>
        /// <returns></returns>
        bool ValidateBeforeEnter(TModel model, TState oldState);
    }

    public class State<TModel> : IState<State<TModel>, TModel>
    {
        public State(Func<TModel, bool> invariant = null, Func<TModel,State<TModel>,bool> beforeEnter = null, Func<TModel, State<TModel>, bool> beforeLeave = null)
        {
            _invariant = invariant;
            _beforeEnter = beforeEnter;
            _beforeLeave = beforeLeave;
        }

        public bool ValidateInvariant(TModel model)
            => _invariant?.Invoke(model) ?? true;

        public bool ValidateBeforeEnter(TModel model, State<TModel> oldState)
            => _beforeEnter?.Invoke(model, oldState) ?? true;

        public bool ValidateBeforeLeave(TModel model, State<TModel> newState)
            => _beforeLeave?.Invoke(model, newState) ?? true;

        protected Func<TModel, bool> _invariant;
        protected Func<TModel, State<TModel>, bool> _beforeEnter;
        protected Func<TModel, State<TModel>, bool> _beforeLeave;
    }

    public class InterceptableEventArgs : EventArgs
    {
        public virtual bool Cancellable { get; }

        public virtual bool Cancel
        {
            get { return _cancel; }
            set
            {
                if (value)
                {
                    if (!Cancellable)
                        throw new InvalidOperationException("Event is not cancellable");
                    _cancel = value;
                }
            }
        }
        protected bool _cancel;
    }

    public class StateChangeEventArgs : InterceptableEventArgs
    {

    }

}
