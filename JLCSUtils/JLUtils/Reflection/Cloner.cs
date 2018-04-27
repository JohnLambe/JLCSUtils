using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Copies objects by Reflection.
    /// </summary>
    public static class Cloner
    {
        /// <summary>
        /// Makes a shallow copy of <paramref name="source"/>.
        /// <para>
        /// All fields are copied, regardless of visibilty. (Properties are not used.)
        /// The class must have a public default constructor.
        /// </para>
        /// <para>
        /// This code must have the right to access non-public members by Reflection.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The object to be copied. If null, null is returned.</param>
        /// <returns>the copy.</returns>
        //| Later version: Use default constructor if protected or private.
        [return: Nullable("If source is null")]
        public static T Clone<T>([Nullable] T source)
            where T : class
        {
            if (source == null)
                return default(T);

            Type targetType = source.GetType();
            /*
            if (EfUtil.IsEfClass(source))
            {
                targetType = source.GetType().BaseType;
            }
            else
            {
                targetType = source.GetType();
                // we could use MemberwiseClone (could call it by Reflection). The result should be the same.
            }
            */

            T target = ReflectionUtil.CreateT<T>(targetType, Type.EmptyTypes, EmptyCollection<object>.EmptyArray);

            Copy(target, source);

            return (T)target;
        }

        /// <summary>
        /// Copy the common fields from <paramref name="source"/> to <paramref name="dest"/>.
        /// The types of the source and destination objects must be the same or one must be a subclass of the other.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">the object to be copied.</param>
        /// <param name="dest">the object to copy to.</param>
        /// <param name="deepCopy">Delegate to decide whether each field is deep copied.
        /// If null, no fields are deep copied. Otherwise, fields are deep copied iff this returns true for them.
        /// </param>
        /// <returns><paramref name="dest"/></returns>
        public static T Copy<T>(T dest, T source, Func<FieldInfo,bool> deepCopy = null)
            where T : class
        {
            dest.ArgNotNull(nameof(dest));

            if (source != null)
            {
                // source and dest are both non-null.

                Type sourceType = source.GetType();
                Type destType = dest.GetType();
                Type definitionType;  // the type to use to read the member definitions. This must be the one higher in the class hierarchy, since its members must be common to the source and destination.

                // determine which to use for the member declarations:
                if (sourceType.IsAssignableFrom(destType))
                {
                    definitionType = sourceType;
                }
                else
                {
                    definitionType = destType;
                }

                // Copy private fields declared on superclasses:
                Type declaredType = definitionType.BaseType;
                while (declaredType != null)               // for each level in the class hierarchy, until `object` is reached:
                {
                    foreach (var field in declaredType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance))
                    {
                        if (field.IsPrivate)
                            field.SetValue(dest, field.GetValue(source));
                    }

                    declaredType = declaredType.BaseType;
                }

                // public and protected fields, and private fields on the lowest level (i.e. everything not done above):
                foreach (var field in definitionType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (deepCopy?.Invoke(field) ?? false)  //(field.IsDefined<ComplexTypeAttribute>())
                    {
                        var sourceValue = field.GetValue(source);
                        var destValue = field.GetValue(dest);

                        if(destValue != null)
                            Copy(field.GetValue(dest), sourceValue);
                        else
                            field.SetValue(dest, Clone(sourceValue));
                    }
                    else
                    {
                        field.SetValue(dest, field.GetValue(source));
                    }
                }
            }
            return dest;
        }
    }
}
