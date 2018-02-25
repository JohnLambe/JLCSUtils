using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Diagnostics.Contracts;
using JohnLambe.Util.Text;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Validation;
using JohnLambe.Util.Types;
using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.Misc;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Reflection-related utilities.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <seealso cref="GenericTypeUtil">For reflection-related utilities relating to generic types (including conversion between generic and non-generic).</seealso>
    /// <seealso cref="TypeUtil"/>
    public static class ReflectionUtil
    {

        // For AutoConfig

        /// <summary>
        /// Returns a list of all supertypes of the given one, starting at the highest (base class)
        /// and ending with (and including) <paramref name="target"/> itself.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypeHeirarchy([Nullable] Type target)
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

        public static Type GetLowestNonAbstractAncestor(Type type)
        {
            Type nextType = type;
            do
            {
                type = nextType;
                nextType = nextType.BaseType;
            } while (nextType != null && !nextType.IsAbstract);
            return type;
        }


        /*
    public static IEnumerable<MemberInfo> GetMemberHeirarchy(MemberInfo target)
    {
        var supertypes = GetTypeHeirarchy(target.DeclaringType);

    }
    */

        //public static IEnumerable<MemberInfo> GetMemberHeirarchy(MemberInfo target)
        //{
        //    BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        //    Type superType = target.DeclaringType.BaseType;
        //    if (superType == null)
        //        return null;

        //    MemberInfo result = null;

        //    if (!MiscUtil.CastNoReturn<Type>(target,
        //        t =>
        //        {
        //            result = superType;
        //        }
        //        ))
        //    if (!MiscUtil.CastNoReturn<MethodInfo>(target,
        //        t =>
        //        {
        //            var parameters = t.GetParameters();
        //            var parameterTypes = new Type[parameters.Length];
        //            for (int n = 0; n < parameterTypes.Length; n++)
        //            {
        //                parameterTypes[n] = parameters[n].ParameterType;
        //            }
        //            result = superType.GetMethod(target.Name, flags, null, parameterTypes, null);

        //        }
        //        ))
        //    if (!MiscUtil.CastNoReturn<PropertyInfo>(target,
        //        t =>
        //        {
        //            /*
        //            var paramInfo = t.GetIndexParameters();
        //            var parameterTypes = new Type[paramInfo.Length];
        //            for (int n = 0; n < parameterTypes.Length; n++)
        //            {
        //                parameterTypes[n] = paramInfo[n].ParameterType;
        //            }
        //            */
        //            result = superType.GetProperty(target.Name, flags);
        //        }
        //        ))
        //        {

        //        }
        //    }
        //}

        #region Get Overridden Member

        /// <summary>
        /// Returns the method that the given one directly overrides.
        /// null if the given method is not an override.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static MethodInfo GetOverriddenMethod([NotNull] this MethodInfo member)
        {
            if (member.IsVirtual)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                Type superType = member.DeclaringType.BaseType;     // supertype of the declaring type - this is the lowest level in the hierarchy that could have the overridden item
                if (superType == null)
                    return null;                 // no base type, so the method must not have been an override

                var parameters = member.GetParameters();
                var parameterTypes = new Type[parameters.Length];
                for (int n = 0; n < parameterTypes.Length; n++)
                {
                    parameterTypes[n] = parameters[n].ParameterType;
                }

                return superType.GetMethod(member.Name, flags, null, parameterTypes, null);
            }
            else
            {
                return null;   // not virtual, so it can't be an override
            }
        }

        /// <summary>
        /// Returns the property that the given one directly overrides.
        /// null if the given one is not an override.
        /// <para>This does not support properties with index parameters.</para>
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static PropertyInfo GetOverriddenProperty([NotNull] this PropertyInfo member)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            Type superType = member.DeclaringType.BaseType;     // supertype of the declaring type - this is the lowest level in the hierarchy that could have the overridden item
            if (superType == null)
                return null;                 // no base type, so the method must not have been an override

            return superType.GetProperty(member.Name, flags);
        }

        #endregion

        /// <summary>
        /// The original (highest level) definition of the property in a direct or indirect superclass.
        /// This may return the same property given as a parameter.
        /// <para>This does not support properties with index parameters.</para>
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>This is similar to <see cref="MethodInfo.GetBaseDefinition"/> for properties.</remarks>
        public static PropertyInfo GetBaseDefinition([NotNull] this PropertyInfo member)
        {
            // Get the base definition of either accessor, then get its class, and find the property on that class:
            return (member.GetMethod ?? member.SetMethod).GetBaseDefinition().DeclaringType.GetProperty(member.Name);
        }

        #region IsOverride
        // Tests whether a member overrides a member of a base type.

        /// <summary>
        /// True iff this method/property is an override.
        /// False iff this is not a method or property.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsOverride([NotNull] this MemberInfo member)
        {
            return (member as PropertyInfo)?.IsOverride() ??
                (member as MethodInfo)?.IsOverride() ??
                false;    // not overridable
        }

        /// <summary>
        /// True iff this method is an override.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsOverride([NotNull] this MethodInfo member)
        {
            return member.IsVirtual                                    // if it's not virtual, it can't be an override
                && member.DeclaringType != member.GetBaseDefinition().DeclaringType;   // if this declaration is not on the same class as the base one
        }

        /// <summary>
        /// True iff this property overrides a base class property.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsOverride([NotNull] this PropertyInfo member)
        {
            // PropertyInfo doesn't have GetBaseDefinition(), so we use either of the accessor methods:
            return IsOverride(member.GetMethod ?? member.SetMethod);
        }

        #endregion

        #region Hiding

        // These don't use HideBySig.

        /// <summary>
        /// True iff this member hides a member of the base type of the type on which it is declared.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>This returns true if something is made inaccessible in C#. It might still be accessible from another language.</remarks>
        public static bool IsHiding([NotNull] this MethodInfo member)
        {
            return !member.IsVirtual                                               // not virtual
                && !(member.DeclaringType.BaseType?.GetMethodExact(member.Name, member.GetParameters())?.IsPrivate ?? true);  // base class member does not exist or is private
        }

        /// <summary>
        /// Tests whether the given member hides a field or property of a base class.
        /// </summary>
        /// <param name="member">
        /// The derived class member.
        /// If this not a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>,
        /// this method just indicates whether there is a field or property with the same name on a base class,
        /// even though it may not actually be hidden (e.g. a property is not hidden by a method with the same name,
        /// at least not in C#).
        /// </param>
        /// <returns>true iff the given member hides a base class member.</returns>
        /// <remarks>This returns true if something is made inaccessible in C#. It might still be accessible from another language.</remarks>
        public static bool IsHidingFieldOrProperty([NotNull] MemberInfo member)
        {
            if ((member as PropertyInfo)?.IsOverride() ?? false)     // if this is an overriding property
                return false;                                        // it doesn't hide anything

            var baseProperty = member.DeclaringType.BaseType?.GetProperty(member.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (baseProperty != null && !baseProperty.IsPrivate())   // if the property exists on the base class and is not virtual or private
                return true;

            var baseField = member.DeclaringType.BaseType?.GetField(member.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (baseField != null && !baseField.IsPrivate)     // hides an non-private field
                return true;

            return false;
        }

        /// <summary>
        /// True if this member hides a member of the base class (makes it inaccessible from code).
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>This returns true if something is made inaccessible in C#. It might still be accessible from another language.</remarks>
        public static bool IsHiding([NotNull] this FieldInfo member)
            => IsHidingFieldOrProperty(member);
        /*
        return FlowControlUtil.TypeSwitch<MemberInfo, bool>(baseMember)
            .Case<FieldInfo>(m => !m.IsPrivate)    // hides an non-private field
            .Case<PropertyInfo>(m => !m.IsPrivate() && !m.IsVirtual())
            .Default(false);
            */

        /// <summary>
        /// True if this member hides a member of the base class (makes it inaccessible from code).
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>This returns true if something is made inaccessible in C#. It might still be accessible from another language.</remarks>
        public static bool IsHiding([NotNull] this PropertyInfo member)
            => IsHidingFieldOrProperty(member);

        /// <summary>
        /// True if this member hides a member of the base class (makes it inaccessible from code).
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>This returns true if something is made inaccessible in C#. It might still be accessible from another language.</remarks>
        public static bool IsHiding([NotNull] this MemberInfo member)
        {
            return FlowControlUtil.TypeSwitch<MemberInfo,bool>(member)
                .Case<PropertyInfo>(m => m.IsHiding())
                .Case<MethodInfo>(m => m.IsHiding())
                .Case<FieldInfo>(m => m.IsHiding())
                .ReturnValue;
        }

        /// <summary>
        /// True iff there is a base class member with the same name as this member,
        /// that is not overridden by this member,
        /// except if this and the base class member are methods with different signatures.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsHidingNonStrict([NotNull] this MemberInfo member)
        {
            if (member is ConstructorInfo)    // the base class constructors are not publicly visible anyway
            {
                return false;
            }
            else if (member is MethodInfo)           
            {
                if(IsHiding((MethodInfo)member))    // if a method that hides a base class method
                    return true;

                return member.DeclaringType.BaseType?.GetMember(member.Name).Where(m1 => !(m1 is MethodInfo)).Any() ?? false;  // if there is a base class member, other than a method, with the same name
            }
            else if(member is FieldInfo || member is PropertyInfo)
            {
                if (IsHidingFieldOrProperty(member))     // if a field or property hiding a field or property
                    return true;

                return member.DeclaringType.BaseType?.GetMember(member.Name).Where(m1 => !(m1 is FieldInfo || m1 is PropertyInfo)).Any() ?? false;    // if there is a base class member, other than a field or property, with the same name
            }
            else
            {   // other types don't support overriding
                return member.DeclaringType.BaseType?.GetMember(member.Name).Any() ?? false;    // if there is any base class member with the same name
            }
        }

        #endregion

        /// <summary>
        /// Returns a method by its name and parameters.
        /// </summary>
        /// <param name="declaringType">The type on which the method is defined.</param>
        /// <param name="name">The name of the method (case-sensitive).</param>
        /// <param name="p">The parameters (types and modifiers) of the parameters. Only exact matches are returned.</param>
        /// <param name="flags">Binding flags for matching the method.</param>
        /// <returns>The requested method. null if no matching method exists.</returns>
        public static MethodInfo GetMethodExact([NotNull] this Type declaringType, [NotNull] string name, [NotNull] ParameterInfo[] p, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
        {
            return declaringType.GetMethods(flags).Where(m => m.Name.Equals(name) && m.GetParameters().SequenceEqual(p, ParameterInfoComparer.Instance)).FirstOrDefault();
        }

        #region Property metadata

        /// <summary>
        /// True iff the property is virtual.
        /// If it has a getter, it is considered vitual iff the getter is vitual,
        /// otherwise, it is cnsidered virtual if the setter is vitual.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsVirtual([NotNull] this PropertyInfo member)
        {
            return (bool)( member.GetMethod?.IsVirtual ?? member.SetMethod?.IsVirtual );
            // fails if property has neither getter nor setter.
        }

        /// <summary>
        /// True iff the property is private, i.e. if both getter and setter are private.
        /// (Based on C# syntax: The visiblity of a property is the visibility of the more visible of the getter and setter).
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsPrivate([NotNull] this PropertyInfo member)
        {
            return (member.GetMethod?.IsPrivate ?? false) || (member.SetMethod?.IsPrivate ?? false);
        }

        #endregion

        /*
        /// <summary>
        /// Returns a list of interface methods implemented by the given method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetImplementedMethods(this MethodInfo method)
        {
            method.DeclaringType.GetInterfaces().SelectMany(
                //TODO
                );
        }
        */

        #region AssignProperty

        /// <summary>
        /// Assign a specified property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="ignoreInvalid">Iff true, no exception is raised if the property does not exist.
        /// (Exceptions are still raised for invalid property values).</param>
        [Obsolete("Use SetProperty")]
        //TODO: Consider dropping, or making conversion logic the same as SetProperty.
        public static void AssignProperty([NotNull] object target, [NotNull] string propertyName, object value, bool ignoreInvalid = false)
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

        #endregion

        #region Create

        /// <summary>
        /// Instantiate this type with the given constructor arguments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T">The type to be created or a type that it is assignable to.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Create<T>([NotNull] this Type type, params object[] arguments)
            where T : class
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
        /// <returns>The new instance.</returns>
        /// <exception>If the type does not have a constructor with the given argument types.</exception>
        public static T CreateT<T>([NotNull] Type type, [NotNull] Type[] argumentTypes, [NotNull] object[] arguments)
            where T : class
        {
            //TODO: Give correct exception when constructor does not exist.
            return (T)type.GetConstructor(argumentTypes).NotNull(() => "Constructor not found: " + type.FullName + " (" + Collections.CollectionUtil.CollectionToString(argumentTypes) + ")", typeof(MemberAccessException))  //TODO: exception type
                .Invoke(arguments);
        }

        #endregion

        //TODO: Move to TypeUtil?
        //TODO: Use Type.GetTypeArray instead ?
        /// <summary>
        /// Returns an array in which each element is the type of the corresponding element in the given array.
        /// </summary>
        /// <param name="values">
        /// The value to determine the types.
        /// If a null array is passed, null is returned.
        /// </param>
        /// <returns>Array of types, or null if <paramref name="values"/> is null.
        /// Where there is a null value in the input (as an element of the input array), the corresponding type is output is typeof(<see cref="object"/>).
        /// </returns>
        /// <remarks>
        /// <para>
        /// Returning typeof(object) for null inputs is because:
        ///  If the input value had a type, `object` is the only type that we know that its type must have been assignable to.
        /// The inputs can be assigned to the types represented by the corresponding outputs.
        /// This property is important for some uses, for example, finding a method which can take the input object array as parameters, or populating generic type parameters.
        /// </para>
        /// <para>
        /// This can never return elements with the value typeof(<see cref="void"/>) because no object (therefore no input to this) can have that type.
        /// </para>
        /// </remarks>
        public static Type[] ArrayOfTypes([Nullable] params object[] values)
        {
            if (values == null)
                return null;
            Type[] result = new Type[values.Length];    // same length as given array
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = values[i]?.GetType() ?? typeof(object);
            }
            return result;
        }

        #region Calling method

        /// <summary>
        /// Call a static method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments">Arguments to the method to be called.</param>
        /// <returns></returns>
        /// <exception cref="MissingMemberException">If the method does not exist.</exception>
        /// <exception cref="AmbiguousMatchException">If there is more than one matching method.</exception>
        /// <exception cref="NullReferenceException">If <paramref name="target"/> is null.</exception>
        public static T CallStaticMethod<T>([NotNull]Type target, [NotNull] string methodName, params object[] arguments)
        {
            //TODO: if there are overloads, find the one that matches arguments
            var method = target.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public); //, new Type[] { });
            if (method == null)
                throw new MissingMemberException("Static method '" + methodName + "' does not exist on " + target.FullName, methodName);
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
        public static T CallMethod<T>([NotNull] object target, [NotNull] string methodName, params object[] arguments)
        {
            //TODO: if there are overloads, find the one that matches arguments
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public); //, ArrayOfTypes(arguments), );
            if (method == null)
                throw new MissingMemberException("Method '" + methodName + "' does not exist on " + target.GetType().FullName, methodName);
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
        public static TReturn CallMethodVarArgs<TReturn>([NotNull] object target, [NotNull] string methodName, params object[] arguments)
        {
            return CallMethodVarArgsEx<TReturn>(target, methodName, BindingFlagsExt.Instance | BindingFlagsExt.Static | BindingFlagsExt.Public, arguments);
        }

        public static TReturn CallStaticMethodVarArgs<TReturn>([NotNull] Type target, [NotNull] string methodName, params object[] arguments)
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
        public static TReturn CallMethodVarArgsEx<TReturn>([NotNull] object target, [NotNull] string methodName, BindingFlagsExt flags, params object[] arguments)
        {
            Type targetType = flags.HasFlag(BindingFlagsExt.CallStatic) ? (Type)target      // if static, `target` is the type
                : target.GetType();                                                         // otherwise, get the type of `target`
            foreach (var method in targetType.GetMethods(flags.BindingFlags()))
            {   // for each overload/method
                if (method.Name.Equals(methodName))     // if the method has the requested name
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

        #endregion

        /// <summary>
        /// Passed as an argument value to certain methods (that call a method by reflection) to use the default value for that parameter.
        /// </summary>
        public static readonly object DefaultValue = new object();

        //TODO: Move to TypeUtil?
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>true iff this type can be assigned the given value (i.e. <c>x = value</c>, where <c>x</c> is of type <paramref name="type"/>, is valid).</returns>
        public static bool IsAssignableFromValue([NotNull] this Type type, [Nullable] object value)
        {
            if (value == null)
                return !TypeUtil.IsNullable(type);
            else
                return type.IsAssignableFrom(value.GetType());
        }

        //TODO: Move to TypeUtil?
        /// <summary>
        /// Returns true if this type is the given type or a subclass of it.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="c"/> is null.</exception>
        /// <exception cref="NullReferenceException">If <paramref name="type"/> is null.</exception>
        public static bool IsTypeOrSubclassOf([NotNull] this Type type, [NotNull] Type c)
            => type == c || type.IsSubclassOf(c);

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
        /// Parse a property name with an optional suffix for a modifier, into the name and modifier.
        /// </summary>
        /// <param name="propertyReferece"></param>
        /// <param name="modifier">The modifier of the property reference.</param>
        /// <returns></returns>
        private static string ParsePropertyReference([NotNull] string propertyReferece, out PropertyNullabilityModifier modifier)
        {
            var modifierCharacter = propertyReferece.CharAt(propertyReferece.Length - 1);  // get the last character
            switch (modifierCharacter)   //TODO: Use attribute
            {
                case '?':
                    modifier = PropertyNullabilityModifier.Nullable;
                    break;
                case '@':
                    modifier = PropertyNullabilityModifier.ExistsNullable;
                    break;
                case '!':
                    modifier = PropertyNullabilityModifier.NonNullable;
                    break;
                default:
                    modifier = PropertyNullabilityModifier.None;
                    return propertyReferece;
            }
            return propertyReferece.Substring(0, propertyReferece.Length - 1);
        }

        public static bool IsNullable([NotNull] this PropertyNullabilityModifier modifier)
            => modifier <= PropertyNullabilityModifier.ExistsNullable;

        /// <summary>
        /// Get/Set the value of a property, and read the property metadata.
        /// </summary>
        /// <param name="target">The object on which to evaluate the property.
        /// For nested properties, this is the innermost object on exit.</param>
        /// <param name="propertyName">Property name. Can be a nested property.
        /// Each property name in the chain can be suffixed with a symbol to specify nullability - see <see cref="PropertyNullabilityModifier"/>.
        /// <para>The format is (ABNF): *( name [modifier] ".") name [modifier] .</para>
        /// </param>
        /// <param name="action">What to do - see <see cref="PropertyAction"/>.</param>
        /// <param name="value">The value to set; or a reference to receive the value (on Get).
        /// Ignored for <see cref="PropertyAction.GetProperty"/>.
        /// <para>This is modified if and only if <paramref name="action"/> is <see cref="PropertyAction.GetValue"/>.
        /// In this case, it is set to null on failure (if a property does not exist, or nested value that this property is on, is null).
        /// </para>
        /// </param>
        /// <param name="defaultNullability">The nullability of parts of the path that don't have one explicitly specified.</param>
        /// <returns>The details of the innermost property. null if <paramref name="target"/> or the property (or any property in the chain) does not exist, or an item that the requested property is on, is null.</returns>
        //TODO: If there is a null in part of the property name string ("X.Y" where X evaluates to null), try to get the metadata from the compile-time type of X. And update BoundProperty to use this.
        private static PropertyInfo GetSetProperty(ref object target, [NotNull] string propertyName, PropertyAction action, ref object value, PropertyNullabilityModifier defaultNullability = PropertyNullabilityModifier.Nullable)
        {
            PropertyInfo property = null;
            string[] levels = propertyName.Split('.');
            PropertyNullabilityModifier nullabilityModifier = defaultNullability;
            object rootTarget = target;

            for (int level = 0; level < levels.Length; level++)
            {
                var currentLevelNullabilityModifier = nullabilityModifier;  // the nullability modifier on the previous part, which applies at this level

                // Parse the property reference (into name and nullability):
                string localPropertyName = ParsePropertyReference(levels[level], out nullabilityModifier);
                if (nullabilityModifier == PropertyNullabilityModifier.None)    // if none specified explicitly
                    nullabilityModifier = defaultNullability;                 // use the default

                // Determine whether this level is null:
                Exception ex = null;
                bool isNull = false;
                if (target == null)         // trying to dereference a null (before the last level)
                {
                    isNull = true;
                    if (!currentLevelNullabilityModifier.IsNullable())    // null and not nullable
                    {
                        ex = new NullReferenceException("Null reference in '" + propertyName + "'"
                                    + " on (root) " + DiagnosticStringUtil.GetDebugDisplay(rootTarget)
                                + ": '" + localPropertyName + "' (level " + level + ") is null");
                    }
                }
                else
                {
                    property = target.GetType().GetProperty(localPropertyName);
                    if (property == null)               // property does not exist
                    {
                        isNull = true;
                        if (currentLevelNullabilityModifier != PropertyNullabilityModifier.Nullable)
                        {
                            ex = new KeyNotFoundException("Property does not exist in '" + propertyName + "'"
                                    + " on (root) " + DiagnosticStringUtil.GetDebugDisplay(rootTarget)
                                    + " last part: " + DiagnosticStringUtil.GetDebugDisplay(target)
                                    + ": '" + localPropertyName + "' (level " + level + ") not found");
                        }
                    }
                }

                if (isNull)
                {
                    if (action == PropertyAction.GetValue)
                        value = null;
                    // if 'Set' operation, it is ignored.

                    if (ex != null)
                        throw ex;

                    return null;    // no property to return
                }
                else
                {
                    Diagnostic.Diagnostics.Assert(ex == null, "ReflectionUtil.GetSetProperty: Exception was set with isNull=false");
                }

                if (level < levels.Length - 1)  // not last (innermost) level
                {   // dereference object at this level:
                    target = property.GetValue(target);
                }
            }

            switch (action)
            {
                case PropertyAction.GetValue:
                    value = property.GetValue(target);
                    break;
                case PropertyAction.SetValue:
                    property.SetValue(target, GeneralTypeConverter.Convert<object>(value, property.PropertyType));
                    break;
            }
            return property;
        }

        /*
        /// <summary>
        /// Return the requested (possibly nested) property of a given type.
        /// Each property in the chain must be defined on the declared type.
        /// (<see cref="GetProperty(ref object, string)"/> allows properties defined only on the runtime type which can be a subclass or impleemnting class
        /// of the declared type.)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyFromType(Type type, [NotNull] string propertyName)
        {
            //TODO-L
        }
        */

        /// <summary>
        /// Return a possibly-nested property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <returns>The requested property, or null if it does not exist (if any property in the chain doesn't exist).</returns>
        public static PropertyInfo GetProperty(ref object target, [NotNull] string propertyName)
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
        /// <param name="defaultNullability">The nullability of parts of the path that don't have one explicitly specified.</param>
        /// <returns>The property value.</returns>
        //| Could call this "ReadProperty".
        public static T TryGetPropertyValue<T>([NotNull] object target, [NotNull] string propertyName, PropertyNullabilityModifier defaultNullability = PropertyNullabilityModifier.Nullable)
        {
            object value = null;
            GetSetProperty(ref target, propertyName, PropertyAction.GetValue, ref value, defaultNullability);
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
        /// <param name="defaultNullability">The nullability of parts of the path that don't have one explicitly specified.</param>
        public static void TrySetPropertyValue<T>([NotNull] object target, [NotNull] string propertyName, [Nullable] T value, PropertyNullabilityModifier defaultNullability = PropertyNullabilityModifier.Nullable)
        {
            object valueObject = value;
            GetSetProperty(ref target, propertyName, PropertyAction.SetValue, ref valueObject, defaultNullability);
            // Note: target is passed by value to this method. Any change to it is ignored.
        }

        #endregion


        /// <summary>
        /// True iff this type implements the given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool Implements([NotNull] this Type type, [NotNull] Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }


        /// <summary>
        /// Invokes a delegate on the default value of the type <typeparamref name="T"/>.
        /// <para>
        /// This can be used like this:<br/>
        /// <code>InstanceInvoke&lt;ClassForTest&gt;(x => nameof(x.Property2))</code> returns "Property2".<br/>
        /// Since nameof cannot take an expression as a parameter, it would otherwise be necessary to declare an instance to do this.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <returns>The return value of the delegate <paramref name="d"/>.</returns>
        public static string InstanceInvoke<T>(Func<T, string> d)
        {
            return d?.Invoke(default(T));
        }

        /// <summary>
        /// Convert a string with camel casing (first letter of each word except the first is capital) to Pascal casing (first letter of each word, including the first, is capital).
        /// </summary>
        /// <param name="camelCase"></param>
        /// <returns></returns>
        [return: Nullable]
        public static string CamelCaseToPascalCase([Nullable] string camelCase)
        {
            if (camelCase == null)
                return null;
            else if(camelCase == "")
                return "";

            // camelCase must have at least one character (because of previous checks)
            return Char.ToUpperInvariant(camelCase[0]) + camelCase.Substring(1);
        }


        /// <summary>
        /// Get a type with the given name in one of the given assemblies.
        /// </summary>
        /// <param name="className">Full class name, without the assembly.</param>
        /// <param name="assemblies">The assemblies to search (in order).</param>
        /// <returns>The found class, or null if not found.</returns>
        [return: Nullable("Not found")]
        public static Type FindType(string className, Assembly[] assemblies = null)
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assm in assemblies)
            {
                var type = assm.GetType(className);
                if (type != null)
                    return type;
            }
            return null;
        }



        // For converting between nullable and non-nullable types, see GenericTypeUtil.
    }

    /*
    public static class Instance
    {
        public static string Name<T>(Func<T, string> d)
        {
            return d.Invoke(default(T));
        }
    }
    */

    /// <summary>
    /// An option on a property referece, specifying how null or invalid values are handled.
    /// <para>
    /// In strings passed to <see cref="ReflectionUtil.TryGetPropertyValue{T}(object, string, PropertyNullabilityModifier)"/>, <see cref="ReflectionUtil.GetSetProperty(ref object, string, ReflectionUtil.PropertyAction, ref object, PropertyNullabilityModifier)"/>, etc.,
    /// this is specified by a character (specified by <see cref="EnumMappedValueAttribute"/> here) after the property name.
    /// </para>
    /// </summary>
    public enum PropertyNullabilityModifier
    {
        /// <summary>
        /// No nullability modifier is present.
        /// </summary>
        None = 0,
        /// <summary>
        /// The value may be null and/or the property may not exist.
        /// </summary>
        [EnumMappedValue('?')]
        Nullable = 1,
        /// <summary>
        /// The property must exist but may have a null value.
        /// </summary>
        [EnumMappedValue('@')]
        ExistsNullable = 2,
        /// <summary>
        /// The value must not be null.
        /// (The property must exist, since there couldn't be a non-null value otherwise.)
        /// </summary>
        [EnumMappedValue('!')]
        NonNullable = 3
    }

    #region BindingFlagsExt

    /// <summary>
    /// Extends <see cref="BindingFlags"/> with options used by this library.
    /// </summary>
    public enum BindingFlagsExt : long
    {
        #region BindingFlags
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
        /*
        PartMatch        = 0x100000000,
        NullForDefault   = 0x200000000,
        */
        /// <summary>
        /// Call/invoke a static member.
        /// </summary>
        CallStatic       = 0x400000000,
        /// <summary>
        /// Include private members from base classes.
        /// </summary>
        InheritedPrivate = 0x800000000,
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


    public class ParameterInfoComparer : IEqualityComparer<ParameterInfo>
    {
        /// <summary>
        /// Don't create instances of this - use <see cref="Instance"/>, because it has no state.
        /// </summary>
        protected ParameterInfoComparer()
        {
        }

        public virtual bool Equals(ParameterInfo x, ParameterInfo y)
        {
            if (x == y)   // same instance or both null
                return true;
            if (x == null || y == null)   // only one is null
                return false;
            // Compare the main properties:
            return x.ParameterType == y.ParameterType
                && x.IsOptional == y.IsOptional
                && x.IsOut == y.IsOut
                && x.IsIn == y.IsIn;
        }

        public virtual int GetHashCode(ParameterInfo obj)
        {
            return obj.GetHashCode();
        }

        public static ParameterInfoComparer Instance = new ParameterInfoComparer();
    }


    //| Choice of modifiers in property reference strings:
    //|
    //| Visible ASCII characters:
    //|
    //| Symbol	Other uses or associations
    //| .	used (member); Acorn: file path separator.
    //| ?	nullable; question/doubt; help; C# '?.'; C ternary operator; wildcard (Windows filanames)
    //| !	proposed C# feature: non-nullable reference type; logical NOT; comment; ! path; error
    //| @	Prefix for variable or expression in Razor; email address; this?; Twitter account; Acorn: current directory
    //| #	expected to be followed by a number/ID?; Twitter hashtag
    //| $	hexadecimal (Pascal); currency?; string (BASIC); Acorn: root of disc
    //| %	display as percentage; modulo; escaping (e.g. URLs); format specifier (printf); enclosing a variable name; Acorn: library directory
    //| &   address-of; BBC BASIC: hexadecimal
    //| ^	dereference; power (BASIC, Pascal); bitwise XOR
    //| "'()[]{}	brackets, quotes; expected to be matched
    //| * +	mathematical operators; wildcards ('*' (filenames, regex), '+' (regex)); UML visibility
    //| / - mathematical operators; Unix and web path separator ('/'); separators; private visibility (UML '-')
    //| ,:;|	separators; C comma operator; comment (';','|'); bitwise OR ('|')
    //| <>	brackets; comparison; enclose tag (XML, HTML) or variable etc.
    //| =	assignment (e.g. C) / equality (e.g. BASIC, Pascal); name/value separator
    //| `	quote; Maths: decorator; separator (.NET generic type names); too different to '?' and '!' ?
    //| ~	approximate; bitwise NOT; BBC BASIC: display in hex.
    //| \	Windows (and Registry) path separator; escaping next character
    //| 'A'-'Z','a'-'z','0'-'9','_'	identifier characters
    //| ' '

}
