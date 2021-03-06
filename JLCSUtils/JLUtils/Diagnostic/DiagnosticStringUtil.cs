﻿using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Diagnostic
{
    public static class DiagnosticStringUtil
    {
        /// <summary>
        /// Returns a string description of the given instance for diagnostic purposes -
        /// a descritpion intended for developers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ObjectToString<T>(T instance)
        {
            if (instance == null)
            {
                if (typeof(T).IsNullableValueType())
                    return Nullable.GetUnderlyingType(typeof(T)).Name + "?(null)";
                else
                    return ((typeof(T) != typeof(object)) ? typeof(T).FullName : "")
                        + "(null)";
            }
            else
            {
                if (instance.GetType().IsPrimitive || instance is string)
                {
                    return instance.GetType().Name + "("
                        + ExceptionUtil.TryEvaluate(() => instance.ToString(), ex => "<" + ex.Message + ">")
                        + ")";
                }
                else
                {
                    StringBuilder s = new StringBuilder(256);
                    s.Append(GetTypeDisplayName(instance.GetType()) + "(");
                    bool first = true;   // true for the first item
                    foreach (var property in instance.GetType().GetProperties())
                    {
                        if (!first)
                            s.Append(' ');
                        else
                            first = false;
                        s.Append(property.Name + "=" 
                            + ExceptionUtil.TryEvaluate(() => property.GetValue(instance), ex => "<" + ex.Message + ">")
                            );
                    }
                    s.Append(')');
                    return s.ToString();
                }
            }
        }

        public static string GetTypeDisplayName(Type t)
        {
            if (IsAnonymousType(t))
                return "";
            else if (t.IsPrimitive)
                return t.Name;
            else
                return t.FullName;
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff <paramref name="t"/> is an anonymous type.</returns>
        public static bool IsAnonymousType(Type t)
        {
            return t.Name.StartsWith("<>");
            // e.g. "<>f__AnonymousType0`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"
        }

        /// <summary>
        /// Converts the member to a string, with more detail than <see cref="Object.ToString"/>.
        /// This always returns the same value for two instances that reference the same member, even if an instance
        /// was returned from a different type (that inherited the member).
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo member)
        {
            if (member == null)
                return null;
            string result = null;
            /*
            if(!MiscUtil.CastNoReturn<MethodInfo>(member,
                m => result = TypeUtil.TypeNameOrVoid(m.ReturnType)
                    + " " + m.DeclaringType.FullName + "." + m.Name
                    + "("
                    + (m.GetParameters()?.ToString() ?? "")  //TODO: convert the parameters to a string in C# syntax
                    + ")"
                ))
                */
            if (!MiscUtil.CastNoReturn<PropertyInfo>(member,
                m => result = TypeUtil.TypeNameOrVoid(m.PropertyType)
                    + " " + m.DeclaringType.FullName + "." + m.Name
                ))
            {
                result = member.DeclaringType.FullName + ": " + member?.ToString();
            }
            return result;
        }

        /// <summary>
        /// Returns a description of the given object for display for diagnostics purposes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDebugDisplay(object value)
        {
            return (value?.ToString() ?? "<null>") + " (type: " + value.GetType().Name + ")";
            //TODO: Use System.Diagnostics.DebuggerDisplayAttribute
        }

    }
}
