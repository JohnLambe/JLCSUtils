using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Utilities for working with Attributes.
    /// </summary>
    public static class AttributeUtil
    {
        /*
        /// <summary>
        /// Returns all attributes of types assignable to <typeparamref name="T"/> (which may be an interface).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="inherited"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider, bool inherited = true)
        {
            return provider.GetCustomAttributes(inherited).OfType<T>();
        }
        */

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider)
        {
//            return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(provider);  // the bool parameter defaults to true

            return (T) provider.GetCustomAttributes(typeof(T),true).FirstOrDefault();
//            return provider.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
            /*
            foreach (var attribute in provider.GetCustomAttributes(true))
            {
                if (attribute is T)
                    return (T)attribute;
            }
            return default(T);
            */
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider type, bool inherit = true)
        {
            /*
            // Allocate a temporary array for the filtered collection of attributes, then copy to an array of the correct type.
            // Three arrays allocated.
            object[] attribs = type.GetCustomAttributes(inherit).Where(a => a is T).ToArray();
            T[] result = new T[attribs.Length];
            attribs.CopyTo(result, 0);
            return result;
            */

            // The extra call to Count may cause Type.GetCustomAttributes(bool) to be called twice.
            var attribs = type.GetCustomAttributes(inherit).Where(a => a is T);
            T[] result = new T[attribs.Count()];
            int index = 0;
            foreach (object a in attribs)
            {
                result[index++] = (T)a;
            }
            return result;

            /* Alternative implementation:
            // Evaluates only once, but uses a list.
            var list = new List<T>();
            foreach (var attribute in type.GetCustomAttributes(inherit).Where(a => a is T))
            {
                list.Add((T)attribute);
            }
            return list;
            */
            // Can't do     return (IEnumerable<T>)type.GetCustomAttributes(inherit).Where( a => a is T );
            // because type.GetCustomAttributes(bool) returns object[] .
            // The above could only return IEnumerable<object> .
        }

        /*
        [Obsolete("Use ICustomAttributeProvider.IsDefined")]
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider provider)
        {
            return provider.IsDefined(typeof(T), true);
        }
        */
        
        public static bool IsDefined<T>(this ICustomAttributeProvider provider, bool inherited = true)
        {
            return provider.IsDefined(typeof(T), inherited);
            //return provider.GetCustomAttribute<T>() != null;
        }

        /// <summary>
        /// Same as <see cref="GetCustomAttributes{T}(ICustomAttributeProvider, bool)"/> except that if
        /// any of the attributes implement <see cref="IEnabledAttribute"/> and their <see cref="IEnabledAttribute.Enabled"/> property
        /// is false, they are excluded.
        /// All attributes that do not implement <see cref="IEnabledAttribute"/> are treated the same as those that do and have <see cref="IEnabledAttribute.Enabled"/> == true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider">An item that can have attributes.</param>
        /// <param name="inherit">True iff inherited attributes should be included (otherwise only those defined on the <paramref name="provider"/> itself are returned.</param>
        /// <returns>The attributes.</returns>
        public static IEnumerable<T> GetAttributesEnabled<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T : Attribute
        {
            return provider.GetCustomAttributes(inherit).Where(
                a => a is T
                    && ( (a as IEnabledAttribute)?.Enabled ?? true )
                ).Cast<T>();
        }

        /*
        public static IEnumerable<T> GetAttributesFiltered<T>(this ICustomAttributeProvider provider, bool inherit = true, string filter = null)
            where T : Attribute
        {
            var attribs = provider.GetCustomAttributes(false).Where(
                a => a is T                                                  // required type
                    && ((a as IFilteredAttribute)?.Enabled ?? true)          // not disabled
                    && ((a as IFilteredAttribute)?.Matches(filter) ?? true)  // filter matches
                ).Cast<T>();
            if (!inherit)
                return attribs;

            Type[] ancestors = ReflectionUtil.GetTypeHeirarchy((Type)provider);

            var disabledAttribs = provider.GetCustomAttributes(inherit).Where(
                a => a is T                                                  // required type
                    && !((a as IFilteredAttribute)?.Enabled ?? true)         // not disabled
                    && ((a as IFilteredAttribute)?.Matches(filter) ?? true)  // filter matches
                );
            T[] result = new T[attribs.Count()];
            int index = 0;
            foreach (T a in attribs)
            {
                var filtered = a as IFilteredAttribute;
                if(disabledAttribs.Where( a => ((a as IFilteredAttribute)?.Matches(filter) )
                    )

                if (filtered?.Enabled ?? true)   // if not disabled
                {
                    if(attribs.Where(!(a as IFilteredAttribute)?.Enabled ?? true
                        .Filter.Contains(filter)
                        (a as IFilteredAttribute)?.Filter.Contains(filter)))

                    result[index++] = (T)a;
                }
            }
            return result;
        }
        */

        /// <summary>
        /// Get attribues with the declaring member.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="provider"></param>
        /// <param name="inherited"></param>
        /// <returns></returns>
        public static IEnumerable<AttributeAndMember<TAttribute,TMember>> GetAttributesWithMember<TAttribute,TMember>(this TMember provider, bool inherited = true)
            where TMember : MemberInfo
            where TAttribute : Attribute
        {
            return provider.GetCustomAttributes<TAttribute>().Select(
                a => new AttributeAndMember<TAttribute, TMember>() { DeclaringMember = provider, Attribute = a }
                );
        }


        public static IEnumerable<MemberAttribute<TMember,TAttribute>> GetMemberAttributes<TMember,TAttribute>(Type target, string id = null, string filter = null)
                where TMember : MemberInfo
                where TAttribute : Attribute
        {
            /*
            var results = new List<MemberAttribute<TMember, TAttribute>>();                                // to hold a list of all handlers for the handlerId
            foreach (var method in target?.GetType().GetMethods())             // all methods
            {
                foreach (var attrib in method.GetCustomAttributes<TAttribute>()      // all attributes of each method
                    .Where(a => a.Enabled && (id == null || a.Id.Equals(id))    // attributes for the specified handler on this method
                     FilterMatches(filter, a.Filter))                // apply the filter
                    )
                {
                    new MemberAttribute<TMember, TAttribute>()
                    {
                        Member = method,
                        Attribute = attrib,
                    };
                }
            }
            return results;
            */
            throw new NotImplementedException();
//            return results.OrderBy(h => h.Attribute.Order);
        }

        public static bool Matches(this IFilteredAttribute attribute, string filter)
        {
            return true;
        }

    }

    public interface IEnabledAttribute
    {
        /// <summary>
        /// If this is false, the attribute should be ignored.
        /// </summary>
        bool Enabled { get; }
    }

    public interface IFilteredAttribute : IEnabledAttribute
    {
        string[] Filter { get; } 
    }

    public class MemberAttribute<TMember,TAttribute>
        where TMember : MemberInfo
        where TAttribute : Attribute
    {
        public virtual TMember Member { get; set; }
        public virtual TAttribute Attribute { get; set; }
    }

}
