﻿using System;

namespace JohnLambe.Util.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute to indicate that a property may be injected, or to provide more information about how 
    /// something (including constructor parameters) is injected.
    /// <para>
    /// Injecting a field is not supported. (And public fields are not recommended.)
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    //TODO: Allow on methods - to cause the method to be called with each parameter resolved similarly to a constructor parameter.
    // The Key and ByType properties would not be used. Throw exception if non-default values supplied?
    // If Required==false, and not all required parameters arfe resolved, don't call?
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Value of <see cref="Key"/> to indicate that the name in code of the attributed member should be used as the key.
        /// </summary>
        public const string CodeName = "\x00";

        public InjectAttribute(string key = null)
        {
            Key = key;
        }

        /// <summary>
        /// Name/key of the value to be injected in the DI context.
        /// <para>
        /// Iff <see cref="CodeName"/>, the name of the item (property or parameter) is ued.
        /// Iff null (the default), it is not resolved by key.
        /// Must not contain control codes / invisible characters except by using constants of this class.
        /// </para>
        /// </summary>
        //| The default behaviour is to resolve by type (principle of least surprise).
        //| It might be useful to make the default to resolve by the member name when the type is a primitive,
        //| but handling primitives differently could be counter-intuitive - it would be easy for a developer to
        //| think that that is the default for everything.
        public virtual string Key { get; set; }

        /// <summary>
        /// True iff an exception should be thrown if the dependency cannot be resolved.
        /// </summary>
        public virtual bool Required { get; set; } = true;

        /// <summary>
        /// True iff the item should be resolved by the key (rather than just by its type).
        /// </summary>
        public virtual bool ByKey => Key != null;

        /// <summary>
        /// True if resolving by type alone is allowed.
        /// <para>
        /// If a <see cref="Key"/> is given, it overrides this.
        /// If this is true and a <see cref="Key"/> is supplied, resolving by type is done if resolving by Key fails.
        /// </para>
        /// </summary>
        public virtual bool ByType { get; set; } = true;

        /// <summary>
        /// If false, this attribute and any other <see cref="InjectAttribute"/> attributes on the same item
        /// are ignored.
        /// This can be used to override an attribute on base class.
        /// </summary>
        //| Use of this (set to false) could arguably be considered a violation of the Liskov Substitution Principle. Maybe shouldn't be supported.
        //| The same could be said of allowing overriding at all.
        public virtual bool Enabled { get; set; } = true;
    }
}
