using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Specifies whether a property should be initialized by <see cref="CollectionInitializer"/>.
    /// For use on properties of type <see cref="ICollection{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InitializeCollectionAttribute : Attribute
    {
        /// <summary/>
        /// <param name="enabled"><see cref="Enabled"/></param>
        public InitializeCollectionAttribute(bool enabled = true)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// true to enable automatic population of the attributed property with an empty collection.
        /// </summary>
        public virtual bool Enabled { get; set; }
    }


    /// <summary>
    /// Creates empty collections matching given collection types.
    /// </summary>
    public static class CollectionInitializer
    {
        /// <summary>
        /// Initialize all applicable properties on the given instance.
        /// <para>
        /// See <see cref="IsAutoInitializeProperty"/> for what properties are initialized.
        /// </para>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use to scan for properties to be populated.</param>
        public static void InitializeInstance([NotNull] object instance, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var prop in instance.GetType().GetProperties(bindingFlags).Where(p => IsAutoInitializeProperty(p)))
            {
                InitializeProperty(prop, instance);
            }
        }

        /// <summary>
        /// Indicates whether a property should be initialized by <see cref="InitializeInstance"/>.
        /// <para>
        /// Properties are initialized if they are of type <see cref="ICollection{T}"/>,
        /// and either 
        /// have an <see cref="InitializeCollectionAttribute"/> when <see cref="InitializeCollectionAttribute.Enabled"/> set to true,
        /// or have an <see cref="InversePropertyAttribute"/>, and no <see cref="InitializeCollectionAttribute"/>.
        /// </para>
        /// </summary>
        /// <param name="property"></param>
        /// <returns>true iff <paramref name="property"/> is initialized by <see cref="InitializeInstance"/>.</returns>
        public static bool IsAutoInitializeProperty([NotNull] PropertyInfo property)
        {
            return GenericTypeUtil.Compare(property.PropertyType, typeof(ICollection<>))
                && property.CanWrite
                && (property.GetCustomAttribute<InitializeCollectionAttribute>()?.Enabled ?? property.IsDefined<InversePropertyAttribute>());
            // if there is no InitializeCollectionAttribute, initialize if there is an InversePropertyAttribute.
        }

        /// <summary>
        /// Assigns the given property to an empty collection (compatible with its type),
        /// if it is null. (If it is write-only, it is assigned regardless of its current value.)
        /// </summary>
        /// <param name="property">The property to assign.
        /// Must be a settable property of a closed generic type matching <see cref="ICollection{T}"/>.
        /// </param>
        /// <param name="target">The object on which to assign this property. Can be null if the property is static.</param>
        public static void InitializeProperty([NotNull] PropertyInfo property, [Nullable] object target)
        {
            if(!property.CanRead || property.GetValue(target) == null)
                property.SetValue(target, CreateCollection(property.PropertyType));
        }

        /// <summary>
        /// Creates an empty collection containing of the given type.
        /// </summary>
        /// <param name="collectionType">Must be a closed generic matching <see cref="ICollection{T}"/>.</param>
        /// <returns>Collection implementing the generic interface specified by <paramref name="collectionType"/> (and <see cref="ICollection"/>).</returns>
        public static ICollection CreateCollection([NotNull] Type collectionType)
        {
            return CreateCollectionOfType(collectionType.GetGenericArguments()[0]);
        }

        /// <summary>
        /// Creates an empty collection containing the given type.
        /// </summary>
        /// <param name="elementType">The type of elements in the collection.</param>
        /// <returns>Collection implementing <see cref="ICollection{T}"/> where T is <paramref name="elementType"/> (and <see cref="ICollection"/>).</returns>
        public static ICollection CreateCollectionOfType([NotNull] Type elementType)
        {
            return typeof(List<>).MakeGenericType(elementType).Create<ICollection>();
        }
    }
}
