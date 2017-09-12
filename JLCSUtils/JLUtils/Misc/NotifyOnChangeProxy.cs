using System;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Misc
{
    public class NotifyOnChangeProxy<T> : DynamicProxyBase<T>
        where T : class
    {
        public NotifyOnChangeProxy(T wrapped) : base(wrapped)
        {
        }

        protected void Changed(string property, object value = null)
        {
            ValueChanged?.Invoke(this, property, value);
        }

        /*
                public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
                {
                    Changed(binder.Name);
                    return base.TryInvokeMember(binder, args, out result);
                }
            */

        // Overriding this rather than TryInvokeMember, so that the event is not fired on invalid invocations.
        protected override bool TryInvokeMemberInternal(InvokeMemberBinder binder, MethodInfo method, object[] args, out object result)
        {
            if (ShouldFire(FireOnCall, method))
                Changed(binder.Name);

            return base.TryInvokeMemberInternal(binder, method, args, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool TryGetMemberInternal(GetMemberBinder binder, PropertyInfo property, out object result)
        {
            if (ShouldFire(FireOnGet, property))
                Changed(binder.Name);

            return base.TryGetMemberInternal(binder, property, out result);
        }

        /// <summary>
        /// Sets the property and raises the event (whether the property value is changed or not).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Changed(binder.Name, value);
            //| This may be setting the property to the same value as it already has.
            //| Even if it is, we still have to call the setter, since it may have side effects.
            //| Also, we can't assume that it would not change the property value.
            //| A setter may set a value different to what is passed, due to validation, for example, or a change of format
            //| (e.g. a relative file pathname to an absolute one).
            //| We could call the getter (if there is one) after invoking the setter,
            //| and fire the event only if it is different.
            //| Currently, the consumer of the event can do this if necessary.

            return base.TrySetMember(binder, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuredValue">The relevant setting of this class.</param>
        /// <param name="attributeProvider">The member that was invoked (or property accessed).</param>
        /// <returns>true iff the <see cref="ValueChanged"/> event should be fired.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool ShouldFire(bool? configuredValue, ICustomAttributeProvider attributeProvider)
        {
            // Determine whether event should be fired:
            if (configuredValue.HasValue)
            {
                return configuredValue.Value;
            }
            else
            {
                var attribute = attributeProvider.GetCustomAttribute<StateChangeAttribute>();
                return attribute == null ? false : attribute.ChangesState;
            }
        }

        public delegate void ChangeEvent(NotifyOnChangeProxy<T> sender, string name, object value);
        //| Could provide PropertyInfo instead of name.

        public event ChangeEvent ValueChanged;// { add; remove; }

        /// <summary>
        /// Indicates whether the <see cref="ValueChanged"/> event is fired on calling
        /// methods on the wrapped object.
        /// <para>If null, <see cref="ChangesStateAttribute"/> is used, and 
        /// if the called method does not have this attribute, the event is not fired.</para>
        /// </summary>
        public virtual bool? FireOnCall { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="ValueChanged"/> event is fired getting a property value
        /// methods on the wrapped object.
        /// <para>If null, <see cref="ChangesStateAttribute"/> is used, and 
        /// if the called method does not have this attribute, the event is not fired.</para>
        /// <para>Defaults to false.</para>
        /// <para>This should be set to true only when getter of the wrapped object have side effects
        /// that change the values of properties.</para>
        /// </summary>
        public virtual bool? FireOnGet { get; set; }

        protected VoidDelegate<string, NotifyOnChangeProxy<T>> _anyChangeEvent;
        //        protected IDictionary<string, VoidDelegate<NotifyOnChangeProxy<T>>> _events = new Dictionary<string, VoidDelegate<NotifyOnChangeProxy<T>>>();

    }


    /// <summary>
    /// Indicates whether calling the attributed method changes the state of the object in a way observable
    /// to the consumer of the object (e.g. changing the value of a readable property).
    /// When used on a property, it applies to the getter only (setters are expected to change state).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class StateChangeAttribute : Attribute
    {
        public StateChangeAttribute(bool changesState = true)
        {
            this.ChangesState = changesState;
        }

        /// <summary>
        /// True (the default) if invoking this member can change the state of the object
        /// in a way that is observable to its consumer.
        /// </summary>
        public bool ChangesState { get; set; }
    }

}
