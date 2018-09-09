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
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider provider)
            where T: Attribute
        {
            //return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(provider);  // the bool parameter defaults to true

            //            return (T) provider.GetCustomAttributes(typeof(T),true).FirstOrDefault();

//            return Attribute.GetCustomAttributes(provider,true).OfType<T>().FirstOrDefault();
//            return provider.GetCustomAttributes(true).OfType<T>().FirstOrDefault();

            return GetAttributes<T>(provider, true).FirstOrDefault();
        }

        /// <summary>
        /// Returns attributes of <paramref name="provider"/> of type <typeparamref name="T"/>,
        /// supporting the <paramref name="inherit"/> parameter if <paramref name="provider"/> is one the types
        /// supported by the static methods of <see cref="Attribute"/>.
        /// Otherwise, it throws an exception if <paramref name="inherit"/> is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="inherit">false to not include attributes from inherited members; true to include them (and throws an exception if this is not supported for the given type of provider);
        /// null to include them if supported.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true)
                where T: Attribute
        {
            // If there is a method that supports the `inherit` parameter, use it:
            if (provider is MemberInfo)
                return (IEnumerable<T>) Attribute.GetCustomAttributes((MemberInfo)provider, inherit).OfType<T>();
            /*
            else if (provider is ParameterInfo)
                return (IEnumerable<T>) Attribute.GetCustomAttributes((ParameterInfo)provider, inherit.ToBool());
            else if (provider is Module)
                return (IEnumerable<T>) Attribute.GetCustomAttributes((Module)provider, inherit.ToBool());
            else if (provider is Assembly)
                return (IEnumerable<T>) Attribute.GetCustomAttributes((Assembly)provider, inherit.ToBool());
            else if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            */
            else //if (!(inherit == InheritOption.False))    // otherwise, use this if `inherit` is null or false (doesn't supprt inherited)
                //                return provider.GetCustomAttributes(inherit).OfType<T>().ToArray();
                return provider.GetCustomAttributes(inherit).OfType<T>();      // if not getting inherited attributes, this can be used
//            else
//                throw new InvalidOperationException("Can't get inherited attributes from " + provider.GetType());

            // see https://github.com/dotnet/corefx/issues/8220
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T: Attribute
        {
            return GetAttributes<T>(provider, inherit);

            /*
            // Allocate a temporary array for the filtered collection of attributes, then copy to an array of the correct type.
            // Three arrays allocated.
            object[] attribs = type.GetCustomAttributes(inherit).Where(a => a is T).ToArray();
            T[] result = new T[attribs.Length];
            attribs.CopyTo(result, 0);
            return result;
            */

            /*
            // The extra call to Count may cause Type.GetCustomAttributes(bool) to be called twice.
            var attribs = provider.GetCustomAttributes(inherit).Where(a => a is T);
            T[] result = new T[attribs.Count()];
            int index = 0;
            foreach (object a in attribs)
            {
                result[index++] = (T)a;
            }
            return result;
            */

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

        
        public static bool IsDefined<T>(this ICustomAttributeProvider provider, bool inherit = true)
        {
            if (provider is MemberInfo)
                Attribute.IsDefined((MemberInfo)provider, typeof(T), inherit);

            return provider.IsDefined(typeof(T), inherit);
            //return provider.GetCustomAttribute<T>() != null;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool inherit = true)
            where T: Attribute
        {
            return GetAttributes<T>(provider, inherit).Any();
        }
        */

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
            return GetAttributes<T>(provider,inherit).Where(a => (a as IEnabledAttribute)?.Enabled ?? true);
            /*
            return provider.GetCustomAttributes(inherit).Where(
                a => a is T
                    && ( (a as IEnabledAttribute)?.Enabled ?? true )
                ).Cast<T>();
            */
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
            return GetAttributes<TAttribute>(provider).Select(
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
            return true;  //TODO
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
