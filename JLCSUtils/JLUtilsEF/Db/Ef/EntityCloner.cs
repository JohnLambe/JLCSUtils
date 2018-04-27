using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Db.Ef
{
    public static class EntityCloner
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
        /// <param name="source"></param>
        /// <param name="detached">
        /// If true, 
        /// then if <paramref name="source"/> is an Entity Framework proxy, this returns a detached object, of the non-proxy type,
        /// with the same values.
        /// </param>
        /// <returns>the copy.</returns>
        //| Later version: Use default constructor if protected or private.
        public static T Clone<T>(T source, bool detached = false)
            where T : class
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

            return Cloner.Copy(target, source, field => field.IsDefined<ComplexTypeAttribute>());
        }
    }
}
