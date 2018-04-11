using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Db.Ef
{
    public static class Cloner
    {
        /// <summary>
        /// Makes a shallow copy of <paramref name="source"/>.
        /// <para>
        /// All fields are copied, regardless of visibilty. (Properties are not used.)
        /// The class must have a public default constructor.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="detached">
        /// If <paramref name="source"/> is an Entity Framework proxy, this returns a detached object, of the non-proxy type,
        /// with the same values.
        /// </param>
        /// <returns>the copy.</returns>
        /// | Later version: Use default constructor if protected or private.
        public static T Clone<T>(T source, bool detached = false)
            where T: class
        {
            if (source == null)
                return default(T);

            Type targetType;
            if (EfUtil.IsEfClass(source))
            {
                targetType = source.GetType().BaseType;
            }
            else
            {
                targetType = source.GetType();
                // we could use MemberwiseClone (could call it by Reflection). The result should be the same.
            }

            T target = ReflectionUtil.CreateT<T>(targetType, Type.EmptyTypes, EmptyCollection<object>.EmptyArray);

            Copy(target, source);

            return (T)target;
        }

        /// <summary>
        /// Copy the common fields from <paramref name="source"/> to <paramref name="dest"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns><paramref name="source"/></returns>
        public static T Copy<T>(T dest, T source)
            where T: class
        {
            dest.ArgNotNull(nameof(dest));
            if(source != null)
            {
                // source and dest are both non-null.

                Type sourceType = source.GetType();
                Type destType = dest.GetType();
                Type definitionType;
                if(sourceType.IsAssignableFrom(destType))
                {
                    definitionType = sourceType;
                }
                else
                {
                    definitionType = destType;
                }

                // Copy private fields declared on superclasses:
                Type declaredType = definitionType.BaseType;
                while(declaredType != null)               // for each level in the class hierarchy, until `object` is reached:
                {
                    foreach (var field in declaredType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance))
                    {
                        if(field.IsPrivate)
                            field.SetValue(dest, field.GetValue(source));
                    }

                    declaredType = declaredType.BaseType;
                }

                // public and protected fields, and private fields on the lowest level:
                foreach (var field in definitionType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    field.SetValue(dest, field.GetValue(source));
                }

            }
            return dest;
        }
    }
}
