using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

using JohnLambe.Util.Reflection;

namespace JohnLambe.Util
{
    /// <summary>
    /// Base class for proxy classes.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicProxyBase<T> : DynamicObject
        where T: class
    {
        /// <summary>
        /// Create with no wrapped instance (for subclasses only).
        /// It can be assigned by the subclass later.
        /// <para>Methods of this class that try to invoke anything require <see cref="Wrapped"/>
        /// to be populated.</para>
        /// </summary>
        protected DynamicProxyBase()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrapped">The object to be wrapped by this istance.</param>
        public DynamicProxyBase(T wrapped)
        {
            this.Wrapped = wrapped.ArgNotNull(nameof(wrapped));
        }

        //
        // Summary:
        //     Provides the implementation for operations that invoke a member. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as calling a method.
        //
        // Parameters:
        //   binder:
        //     Provides information about the dynamic operation. The binder.Name property provides
        //     the name of the member on which the dynamic operation is performed. For example,
        //     for the statement sampleObject.SampleMethod(100), where sampleObject is an instance
        //     of the class derived from the System.Dynamic.DynamicObject class, binder.Name
        //     returns "SampleMethod". The binder.IgnoreCase property specifies whether the
        //     member name is case-sensitive.
        //
        //   args:
        //     The arguments that are passed to the object member during the invoke operation.
        //     For example, for the statement sampleObject.SampleMethod(100), where sampleObject
        //     is derived from the System.Dynamic.DynamicObject class, args[0] is equal to 100.
        //
        //   result:
        //     The result of the member invocation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type[] parameters = new Type[args.Length];
            for(int index=0; index < args.Length; index++)
            {
                if(args[index] != null)
                    parameters[index] = args[index].GetType();
            }
            var method = Wrapped.GetType().GetMethod(binder.Name,
                //DefaultBindingFlags,
                parameters
                );
            if (method != null)
            {
                return TryInvokeMemberInternal(binder, method, args, out result);
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Called when an attempt to invoke a member is resolved to a method.
        /// </summary>
        /// <param name="method">The method to be invoked. Never null.</param>
        /// <param name="args">The arguments to this method.</param>
        /// <param name="result">The result of the member invocation. This will be returned by <see cref="TryInvokeMember"/>.</param>
        /// <returns>
        /// The return value of TryInvokeMember:
        ///     (from its documentation:) 
        ///     true if the operation is successful; otherwise, false. If this method returns
        ///     false, the run-time binder of the language determines the behavior. (In most
        ///     cases, a language-specific run-time exception is thrown.)
        /// </returns>
        protected virtual bool TryInvokeMemberInternal(InvokeMemberBinder binder, MethodInfo method, object[] args, out object result)
        {
            result = method.Invoke(Wrapped, args);
            return true;
        }

        //
        // Summary:
        //     Provides the implementation for operations that get member values. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as getting a value for a property.
        //
        // Parameters:
        //   binder:
        //     Provides information about the object that called the dynamic operation. The
        //     binder.Name property provides the name of the member on which the dynamic operation
        //     is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty)
        //     statement, where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies
        //     whether the member name is case-sensitive.
        //
        //   result:
        //     The result of the get operation. For example, if the method is called for a property,
        //     you can assign the property value to result.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a run-time exception is thrown.)
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = Wrapped.GetType().GetProperty(binder.Name, DefaultBindingFlags);
            if (property != null)
            {
                return TryGetMemberInternal(binder, property, out result);
            }
            else
            {
                result = null;
                return false;
            }
        }

        protected virtual bool TryGetMemberInternal(GetMemberBinder binder, PropertyInfo property, out object result)
        {
            result = property.GetValue(Wrapped);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = Wrapped.GetType().GetProperty(binder.Name, DefaultBindingFlags);
            if (property != null)
            {
                return TrySetMemberInternal(binder,property,value);
            }
            else
            {
                return false;
            }
        }

        protected virtual bool TrySetMemberInternal(SetMemberBinder binder, PropertyInfo property, object value)
        {
            property.SetValue(Wrapped, value);
            return true;
        }

        //
        // Summary:
        //     Provides the implementation for operations that get a value by index. Classes
        //     derived from the System.Dynamic.DynamicObject class can override this method
        //     to specify dynamic behavior for indexing operations.
        //
        // Parameters:
        //   binder:
        //     Provides information about the operation.
        //
        //   indexes:
        //     The indexes that are used in the operation. For example, for the sampleObject[3]
        //     operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived
        //     from the DynamicObject class, indexes[0] is equal to 3.
        //
        //   result:
        //     The result of the index operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a run-time exception is thrown.)
        /*        public virtual bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
                {

                    Wrapped.GetType().
                }
                */

        //
        // Summary:
        //     Provides the implementation for operations that set a value by index. Classes
        //     derived from the System.Dynamic.DynamicObject class can override this method
        //     to specify dynamic behavior for operations that access objects by a specified
        //     index.
        //
        // Parameters:
        //   binder:
        //     Provides information about the operation.
        //
        //   indexes:
        //     The indexes that are used in the operation. For example, for the sampleObject[3]
        //     = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject
        //     is derived from the System.Dynamic.DynamicObject class, indexes[0] is equal to
        //     3.
        //
        //   value:
        //     The value to set to the object that has the specified index. For example, for
        //     the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic),
        //     where sampleObject is derived from the System.Dynamic.DynamicObject class, value
        //     is equal to 10.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.
        //        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value);

        //
        // Summary:
        //     Provides the implementation for operations that invoke an object. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as invoking an object or a delegate.
        //
        // Parameters:
        //   binder:
        //     Provides information about the invoke operation.
        //
        //   args:
        //     The arguments that are passed to the object during the invoke operation. For
        //     example, for the sampleObject(100) operation, where sampleObject is derived from
        //     the System.Dynamic.DynamicObject class, args[0] is equal to 100.
        //
        //   result:
        //     The result of the object invocation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.
        //        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result);

        //
        // Summary:
        //     Provides implementation for type conversion operations. Classes derived from
        //     the System.Dynamic.DynamicObject class can override this method to specify dynamic
        //     behavior for operations that convert an object from one type to another.
        //
        // Parameters:
        //   binder:
        //     Provides information about the conversion operation. The binder.Type property
        //     provides the type to which the object must be converted. For example, for the
        //     statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic),
        //     where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, binder.Type returns the System.String type. The binder.Explicit property
        //     provides information about the kind of conversion that occurs. It returns true
        //     for explicit conversion and false for implicit conversion.
        //
        //   result:
        //     The result of the type conversion operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if(Wrapped.GetType().IsAssignableFrom(binder.Type))
            {
                result = Wrapped;
                return true;
            }
            else
            {
                result = null;
                if (binder.Explicit)
                {
                    try
                    {
                        result = Convert.ChangeType(Wrapped, binder.Type);
                        return true;
                    }
                    catch(System.InvalidCastException)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        //
        // Summary:
        //     Provides implementation for unary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as negation, increment, or decrement.
        //
        // Parameters:
        //   binder:
        //     Provides information about the unary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     negativeNumber = -number statement, where number is derived from the DynamicObject
        //     class, binder.Operation returns "Negate".
        //
        //   result:
        //     The result of the unary operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        //        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result);

        //
        // Summary:
        //     Provides implementation for binary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as addition and multiplication.
        //
        // Parameters:
        //   binder:
        //     Provides information about the binary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     sum = first + second statement, where first and second are derived from the DynamicObject
        //     class, binder.Operation returns ExpressionType.Add.
        //
        //   arg:
        //     The right operand for the binary operation. For example, for the sum = first
        //     + second statement, where first and second are derived from the DynamicObject
        //     class, arg is equal to second.
        //
        //   result:
        //     The result of the binary operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        //        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);

        //
        // Summary:
        //     Provides implementation for binary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as addition and multiplication.
        //
        // Parameters:
        //   binder:
        //     Provides information about the binary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     sum = first + second statement, where first and second are derived from the DynamicObject
        //     class, binder.Operation returns ExpressionType.Add.
        //
        //   arg:
        //     The right operand for the binary operation. For example, for the sum = first
        //     + second statement, where first and second are derived from the DynamicObject
        //     class, arg is equal to second.
        //
        //   result:
        //     The result of the binary operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
//        public virtual bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);

        //
        // Summary:
        //     Provides implementation for unary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as negation, increment, or decrement.
        //
        // Parameters:
        //   binder:
        //     Provides information about the unary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     negativeNumber = -number statement, where number is derived from the DynamicObject
        //     class, binder.Operation returns "Negate".
        //
        //   result:
        //     The result of the unary operation.
        //
        // Returns:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
//        public virtual bool TryUnaryOperation(UnaryOperationBinder binder, out object result);

        protected const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        protected T Wrapped { get; set; }
    }

    /// <summary>
    /// A proxy that populates the wrapped object on the first attempt to access it (any member or the value of the object).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyPopulatedProxy<T> : DynamicProxyBase<T>
        where T: class
    {
        public LazyPopulatedProxy(Func<T> populateDelegate) : base()
        {
            PopulateDelegate = populateDelegate;
        }

        protected virtual void Populate()
        {
            Wrapped = PopulateDelegate();
            Populated = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void LazyPopulated()
        {
            if (!Populated)
                Populate();
        }

        // These methods will populate this object even on a failed attempt to invoke a member (when the member does not exist).
        // We could first check that the invocation attempt is valid based on typeof(T),
        // but invalid calls are probably rare (not worth optimising for)
        // and that would give different behaviour where the delegate returns a subclass of T.

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            LazyPopulated();
            return base.TryInvokeMember(binder,args,out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            LazyPopulated();
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            LazyPopulated();
            return base.TrySetMember(binder, value);
        }

        protected virtual Func<T> PopulateDelegate { get; set; }
        protected bool Populated { get; set; }
    }


    public class NotifyOnChangeProxy<T> : DynamicProxyBase<T>
        where T: class
    {
        public NotifyOnChangeProxy(T wrapped): base(wrapped)
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
            if(ShouldFire(FireOnCall, method))
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

        protected VoidDelegate<string,NotifyOnChangeProxy<T>> _anyChangeEvent;
//        protected IDictionary<string, VoidDelegate<NotifyOnChangeProxy<T>>> _events = new Dictionary<string, VoidDelegate<NotifyOnChangeProxy<T>>>();

    }

    /// <summary>
    /// Indicates whether calling the attributed method changes the state of the object in a way observable
    /// to the consumer of the object (e.g. changing the value of a readable property).
    /// When used on a property, it applies to the getter only (setters are expected to change state).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property,Inherited=true,AllowMultiple=false)]
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
