using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Diagnostics.Contracts;
using JohnLambe.Util.Text;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Validation;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Reflection-related utilities.
    /// </summary>
    public static class ReflectionUtil
    {

        // For AutoConfig

        /// <summary>
        /// Returns a list of all supertypes of the given one, starting at the highest (base class)
        /// and ending with (and including) `target` itself.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static LinkedList<Type> GetTypeHeirarchy(Type target)
        {
            LinkedList<Type> heirarchy = new LinkedList<Type>();
            while (target != null)
            {
                heirarchy.AddFirst(target);
                target = target.BaseType;
            }
            return heirarchy;
        }


        // end AutoConfig

        //        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type, )


        public static void AssignProperty(object target, string propertyName, object value)
        {
            AssignProperty(target, propertyName, value, false);
        }

        /// <summary>
        /// Assign a specified property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="ignoreInvalid">Iff true, no exception is raised if the property does not exist.
        /// (Exceptions are still raised for invalid property values).</param>
        public static void AssignProperty(object target, string propertyName, object value, bool ignoreInvalid)
        {
            var property = target.GetType().GetProperty(propertyName);
            if (property != null)
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType); // try to get a converter for the target type
                if (converter != null && converter.CanConvertFrom(value.GetType()))  // if there is a converter
                /*CanConvertTo(property.PropertyType)*/
                {
                    if (converter.IsValid(value))
                    {
                        /*value = converter.ConvertTo(value, property.PropertyType);*/
                        value = converter.ConvertFrom(value);
                        property.SetValue(target, value, null);
                    }
                    else
                    {
                        throw new ArgumentException("Value of " + propertyName + " is invalid: " + value);
                        // parameter is not a valid string representation of the target type
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid parameter: " + propertyName);
                    // property cannot be assigned from a string
                }
            }
            else
            {
                if (!ignoreInvalid)
                {
                    throw new ArgumentException("Unrecognised parameter: " + propertyName);
                    // no property with the given name
                }
            }
        }

        /// <summary>
        /// Instantiate this type with the given constructor arguments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T">The </typeparam>
        /// <returns></returns>
        public static T Create<T>(this Type type, params object[] arguments)
            where T: class
        {
            /* old version:
            var argumentTypes = new Type[arguments.Length];
            for (int n = 0; n < arguments.Length; n++)
            {
                if (arguments[n] == null)
                    argumentTypes[n] = typeof(object);
                else
                    argumentTypes[n] = arguments[n].GetType();
            }
            return CreateT<T>(type,argumentTypes,arguments);
            */
            return CreateT<T>(type, ReflectionUtil.ArrayOfTypes(arguments), arguments);            
        }

        /// <summary>
        /// Like <see cref="Create{T}(Type, object[])"/>, but the paramter types are specified explicitly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="argumentTypes"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T CreateT<T>(Type type, Type[] argumentTypes, object[] arguments)
            where T: class
        {
            return (T)type.GetConstructor(argumentTypes).Invoke(arguments);
        }

        /// <summary>
        /// Returns an array in which each element is the type of the corresponding element in the given array.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Array of types, or null if <paramref name="values"/> is null.
        /// Where there is a null value in the input, the corresponding type is output as null.</returns>
        public static Type[] ArrayOfTypes(params object[] values)
        {
            if (values == null)
                return null;
            Type[] result = new Type[values.Length];    // same length as given array
            for(int i = 0; i < values.Length; i++)
            {
                result[i] = values[i]?.GetType();
            }
            return result;
        }

        /// <summary>
        /// Call a static method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments">Arguments to the method to be called.</param>
        /// <returns></returns>
        public static T CallStaticMethod<T>(Type target, string methodName, params object[] arguments)
        {
            //TODO: if there are overloads, find the one that matches arguments
            var method = target.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public); //, new Type[] { });
            return (T)method.Invoke(target, arguments);
        }

        /// <summary>
        /// Call a static or instance method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T CallMethod<T>(object target, string methodName, params object[] arguments)
        {
            //TODO: if there are overloads, find the one that matches arguments
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public); //, ArrayOfTypes(arguments), );
            if (method.IsStatic)
                target = null;      // necessary for static constructors only
            return (T)method.Invoke(target, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn">The return type. It can match methods whose return type is assignable to this.</typeparam>
        /// <param name="target">The instance on which to call the method. Static methods on its type can also be called.</param>
        /// <param name="methodName">The name of the method to call (case-sensitive).</param>
        /// <param name="arguments">The arguments to pass to the method to be called.</param>
        /// <returns>The return value of the called method.</returns>
        public static TReturn CallMethodVarArgs<TReturn>(object target, string methodName, params object[] arguments)
        {
            return CallMethodVarArgsEx<TReturn>(target, methodName, BindingFlagsExt.Instance | BindingFlagsExt.Static | BindingFlagsExt.Public, arguments);
        }

        public static TReturn CallStaticMethodVarArgs<TReturn>(Type target, string methodName, params object[] arguments)
        {
            return CallMethodVarArgsEx<TReturn>(target, methodName, BindingFlagsExt.CallStatic | BindingFlagsExt.Static | BindingFlagsExt.Public, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn">The return type. It can match methods whose return type is assignable to this.</typeparam>
        /// <param name="target"></param>
        /// <param name="methodName"></param>
        /// <param name="flags"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static TReturn CallMethodVarArgsEx<TReturn>(object target, string methodName, BindingFlagsExt flags, params object[] arguments)
        {
            Type targetType = flags.HasFlag(BindingFlagsExt.CallStatic) ? (Type)target      // if static, `target` is the type
                : target.GetType();                                                         // otherwise, get the type of `target`
            foreach (var method in targetType.GetMethods(flags.BindingFlags()))
            {   // for each overload/method
                if( method.Name.Equals(methodName) )     // if the method has the requested name
                {
                    bool fail = false;
                    var parameters = method.GetParameters();
                    int paramCount = parameters.Count();
                    //if (!flags.HasFlag(BindingFlagsExt.PartMatch) && paramCount < arguments.Length)    // if this method has to few parameters
                    //{
                    //    fail = true;
                    //
                    //}

                    var args = new object[paramCount];    // to be populated with the arguments for this method
                    var argIndex = 0;

                    if (typeof(TReturn).IsAssignableFrom(method.ReturnType))    // if the return type is compatible
                    {
                        foreach (var parameter in parameters)
                        {
                            bool noArg = argIndex >= arguments.Length;
                            // if(!noArg)
                            //   if( (flags.HasFlag(BindingFlagsExt.NullForDefault) && arguments[argIndex]==null) || arguments[argIndex]==DefaultValue))
                            //     arguments[argIndex] == null);

                            if (parameter.HasDefaultValue && noArg)  // 
                            {
                                args[argIndex] = parameter.DefaultValue;
                            }
                            else if (IsAssignableFromValue(parameter.ParameterType, arguments[argIndex]))
                            {
                                args[argIndex] = arguments[argIndex];
                            }
                            else
                            {
                                fail = true;
                                break;
                            }
                            argIndex++;
                        }
                        if (!fail)
                        {
                            return (TReturn)method.Invoke(target, args);
                        }
                    }
                }
            }

            throw new ArgumentException("No matching method found for '" + methodName + "' (no overload has compatible parameters)");
            //TODO: Better exceptions
        }

        /// <summary>
        /// Passed as an argument value to certain methods (that call a method by reflection) to use the default value for that parameter.
        /// </summary>
        public static readonly object DefaultValue = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>True iff this type can be assigned the given value.</returns>
        public static bool IsAssignableFromValue(this Type type, object value)
        {
            if (value == null)
                return !type.IsValueType;     // assignable to null iff not a value type
            else
                return type.IsAssignableFrom(value.GetType());
        }

        #region GetProperty

        /// <summary>
        /// An action to be done on a property.
        /// </summary>
        private enum PropertyAction
        {
            /// <summary>
            /// Return the property metadata only.
            /// </summary>
            GetProperty,
            /// <summary>
            /// Get the value of the property.
            /// </summary>
            GetValue,
            /// <summary>
            /// Set the value of the property.
            /// </summary>
            SetValue
        };

        /// <summary>
        /// Get/Set the value of a property, and read the property metadata.
        /// </summary>
        /// <param name="target">The object on which to evaluate the property.
        /// For nested properties, this is the innermost object on exit.</param>
        /// <param name="propertyName">Property name. Can be a nested property.</param>
        /// <param name="action"></param>
        /// <param name="value">The value to set; or a reference to receive the value (on Get).
        /// Ignored for <see cref="PropertyAction.GetProperty"/>.
        /// </param>
        /// <para>This is modified if and only if <paramref name="action"/> is <see cref="PropertyAction.SetValue"/>.
        /// In this case, it is set to null on failure (if a property does not exist, or nested value that this property is on, is null).
        /// </para>
        /// <returns>The details of the innermost property. null if <paramref name="target"/> or the property (or any property in the chain) does not exist, or an item that the requested property is on, is null.</returns>
        private static PropertyInfo GetSetProperty(ref object target, string propertyName, PropertyAction action, ref object value)
        { 
            PropertyInfo property = null;
            string[] levels = propertyName.Split('.');
            for (int level = 0; level < levels.Length; level++)
            {
                if (target == null)         // trying to dereference a null (before the last level)
                {
                    if (action == PropertyAction.GetValue)
                        value = null;
                    return null;
                }
                property = target.GetType().GetProperty(levels[level]);
                if (property == null)               // property does not exist
                {
                    if(action == PropertyAction.GetValue)               
                        value = null;
                    return null;
                }
                if (level < levels.Length - 1)  // not last (innermost) level
                {   // dereference object at this level:
                    target = property.GetValue(target);
                }
            }
            switch(action)
            {
                case PropertyAction.GetValue:
                    value = property.GetValue(target);
                    break;
                case PropertyAction.SetValue:
                    property.SetValue(target, GeneralTypeConverter.Convert<object>(value,property.PropertyType));
                    break;
            }
            return property;
        }

        /// <summary>
        /// Return a possibly-nested property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <returns>The requested property, or null if it does not exist (if any property in the chain doesn't exist).</returns>
        public static PropertyInfo GetProperty(ref object target, string propertyName)
        {
            object dummy = null;
            return GetSetProperty(ref target, propertyName, PropertyAction.GetProperty, ref dummy);
        }

        /// <summary>
        /// Get the value of a given (possibly nested) property on a given object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object from which to read the property.</param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <returns>The property value.</returns>
        public static T TryGetPropertyValue<T>(object target, string propertyName)
        {
            object value = null;
            GetSetProperty(ref target, propertyName, PropertyAction.GetValue, ref value);
            // Note: target is passed by value to this method. Any change to it is ignored.
            return (T)value;
        }

        /// <summary>
        /// Set the value of a given property on a given object.
        /// Does nothing if the property does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object to set the property on.</param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <param name="value">The value to set.</param>
        public static void TrySetPropertyValue<T>(object target, string propertyName, T value)
        {
            object valueObject = value;
            GetSetProperty(ref target, propertyName, PropertyAction.SetValue, ref valueObject);
            // Note: target is passed by value to this method. Any change to it is ignored.
        }

        #endregion
    }

    #region BindingFlagsExt

    public enum BindingFlagsExt : long
    {
        #region
        //
        // Summary:
        //     Specifies no binding flag.
        Default = 0,
        //
        // Summary:
        //     Specifies that the case of the member name should not be considered when binding.
        IgnoreCase = 1,
        //
        // Summary:
        //     Specifies that only members declared at the level of the supplied type's hierarchy
        //     should be considered. Inherited members are not considered.
        DeclaredOnly = 2,
        //
        // Summary:
        //     Specifies that instance members are to be included in the search.
        Instance = 4,
        //
        // Summary:
        //     Specifies that static members are to be included in the search.
        Static = 8,
        //
        // Summary:
        //     Specifies that public members are to be included in the search.
        Public = 16,
        //
        // Summary:
        //     Specifies that non-public members are to be included in the search.
        NonPublic = 32,
        //
        // Summary:
        //     Specifies that public and protected static members up the hierarchy should be
        //     returned. Private static members in inherited classes are not returned. Static
        //     members include fields, methods, events, and properties. Nested types are not
        //     returned.
        FlattenHierarchy = 64,
        //
        // Summary:
        //     Specifies that a method is to be invoked. This must not be a constructor or a
        //     type initializer.
        InvokeMethod = 256,
        //
        // Summary:
        //     Specifies that Reflection should create an instance of the specified type. Calls
        //     the constructor that matches the given arguments. The supplied member name is
        //     ignored. If the type of lookup is not specified, (Instance | Public) will apply.
        //     It is not possible to call a type initializer.
        CreateInstance = 512,
        //
        // Summary:
        //     Specifies that the value of the specified field should be returned.
        GetField = 1024,
        //
        // Summary:
        //     Specifies that the value of the specified field should be set.
        SetField = 2048,
        //
        // Summary:
        //     Specifies that the value of the specified property should be returned.
        GetProperty = 4096,
        //
        // Summary:
        //     Specifies that the value of the specified property should be set. For COM properties,
        //     specifying this binding flag is equivalent to specifying PutDispProperty and
        //     PutRefDispProperty.
        SetProperty = 8192,
        //
        // Summary:
        //     Specifies that the PROPPUT member on a COM object should be invoked. PROPPUT
        //     specifies a property-setting function that uses a value. Use PutDispProperty
        //     if a property has both PROPPUT and PROPPUTREF and you need to distinguish which
        //     one is called.
        PutDispProperty = 16384,
        //
        // Summary:
        //     Specifies that the PROPPUTREF member on a COM object should be invoked. PROPPUTREF
        //     specifies a property-setting function that uses a reference instead of a value.
        //     Use PutRefDispProperty if a property has both PROPPUT and PROPPUTREF and you
        //     need to distinguish which one is called.
        PutRefDispProperty = 32768,
        //
        // Summary:
        //     Specifies that types of the supplied arguments must exactly match the types of
        //     the corresponding formal parameters. Reflection throws an exception if the caller
        //     supplies a non-null Binder object, since that implies that the caller is supplying
        //     BindToXXX implementations that will pick the appropriate method.
        ExactBinding = 65536,
        //
        // Summary:
        //     Not implemented.
        SuppressChangeType = 131072,
        //
        // Summary:
        //     Returns the set of members whose parameter count matches the number of supplied
        //     arguments. This binding flag is used for methods with parameters that have default
        //     values and methods with variable arguments (varargs). This flag should only be
        //     used with System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[]).
        OptionalParamBinding = 262144,
        //
        // Summary:
        //     Used in COM interop to specify that the return value of the member can be ignored.
        IgnoreReturn = 16777216,

        #endregion

        PartMatch = 0x100000000,
        NullForDefault = 0x200000000,
        CallStatic     = 0x400000000
    }

    public static class BindingFlagsExtension
    {
        /// <summary>
        /// Return the <see cref="System.Reflection.BindingFlags"/> part of this value.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static BindingFlags BindingFlags(this BindingFlagsExt flags)
        {
            return (BindingFlags)((int)flags & 0xFFFFFFFF);
        }
    }

    #endregion


    /// <summary>
    /// Metadata of a property (of a class, struct, interface, dictionary, etc.),
    /// and the object it is defined on.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class BoundProperty<TTarget,TProperty> : ICustomAttributeProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The object on which the property is defined.
        /// </param>
        /// <param name="propertyName">The name of the property. This may be a nested property name (with ".").</param>
        public BoundProperty(TTarget target, string propertyName)
        {
            object targetObject = target;
            Property = ReflectionUtil.GetProperty(ref targetObject, propertyName);
            this.Target = (TTarget)targetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The object on which the property is defined.</param>
        /// <param name="property">The property itself.</param>
        public BoundProperty(TTarget target, PropertyInfo property)
        {
            ObjectExtension.CheckArgNotNull(target, nameof(target));
            property.ArgNotNull(nameof(property));
            Contract.Requires(property.DeclaringType.IsAssignableFrom(target.GetType()));
            this.Target = target;
            this.Property = property;
        }

        /// <summary>
        /// True if the property is readable (not write-only).
        /// </summary>
        public virtual bool CanRead
            => Property?.CanRead ?? false;

        /// <summary>
        /// True if the property is settable (not read-only).
        /// </summary>
        public virtual bool CanWrite
            => Property?.CanWrite ?? false;

        /// <summary>
        /// The value of this property on the bound object.
        /// </summary>
        public virtual TProperty Value
        {
            get
            {
                return (TProperty)Property?.GetValue(Target);
            }
            set
            {
                if (Property != null)
                {
                    Validator?.ValidateValueException(Target, ref value, Property);

                    Property?.SetValue(Target, value);
                }
            }
        }

        /// <summary>
        /// The name of the property (as used in code).
        /// </summary>
        public virtual string Name
            => Property.Name;

        /// <summary>
        /// A caption or name of the property for display to a user.
        /// </summary>
        public virtual string DisplayName
            => CaptionUtil.GetDisplayName(Property);

        /// <summary>
        /// The object that declares this property.
        /// If this object was created using a nested property name, this returns the nested object.
        /// </summary>
        public virtual TTarget Target { get; }

        /// <summary>
        /// The Property metadata. This may be null if this object does not represent a class/struct/interface
        /// (e.g. it could be a dictionary or XML file).
        /// </summary>
        public virtual PropertyInfo Property { get; }

        public virtual ValidatorEx Validator { get; set; }

        #region ICustomAttributeProvider
        // Delegates to Property.

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ((ICustomAttributeProvider)Property).GetCustomAttributes(attributeType, inherit);
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return ((ICustomAttributeProvider)Property).GetCustomAttributes(inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return ((ICustomAttributeProvider)Property).IsDefined(attributeType, inherit);
        }

        #endregion

        //| Could, alternatively, subclass PropertyInfo.
        //| Most members would have to delegate to an underlying PropertyInfo (the Reflection calls can return any subclass; PropertInfo is abstract).
        //| That could be broken by changes to PropertyInfo in future .NET framework versions.
    }

}
