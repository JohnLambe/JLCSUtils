using JohnLambe.Types;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Utility for setting properties to their default value as defined by <see cref="DefaultValueAttribute"/>.
    /// </summary>
    public static class PropertyDefaultUtil
    {
        /// <summary>
        /// Set the values of all writeable properties that have a <see cref="DefaultValueAttribute"/> to
        /// the value specified by that attribute.
        /// <para>
        /// Note: This can try to set private and protected properties (depending on <paramref name="bindingFlags"/>.
        /// This will fail with an exception if it doesn't have rights to do this.
        /// </para>
        /// </summary>
        /// <param name="target">The object to set property values on. If this is null, this does nothing.
        /// </param>
        /// <param name="bindingFlags">Binding flags to filter properties to be set.
        /// The default is all instance properties, including non-public and private properties on base classes.
        /// <para>If <see cref="BindingFlagsExt.InheritedPrivate"/> is included, private properties (and properties with private setters) on base classes are recognised.</para>
        /// <para>If <see cref="BindingFlagsExt.Static"/> is included, static properties on the type of the given object are set. (This is not recommended).</para>
        /// </param>
        /// <param name="inheritAttributes">true iff attributes inherited from the property on a base class are recognised.</param>
        public static void PopulateDefaults(object target, BindingFlagsExt bindingFlags = BindingFlagsExt.Public | BindingFlagsExt.NonPublic | BindingFlagsExt.Instance | BindingFlagsExt.FlattenHierarchy | BindingFlagsExt.InheritedPrivate, bool inheritAttributes = true)
        {
            if (target != null)    // ignore if null
            {
                Type targetType = target.GetType();
                PopulateDefaultsInternal(targetType, target, bindingFlags, inheritAttributes);
            }
        }

        /// <summary>
        /// Same as <see cref="PopulateDefaults(object, BindingFlagsExt, bool)"/>, except that this sets
        /// static properties on a type (which may or may not be static).
        /// </summary>
        /// <param name="targetType">The type to set static properties on. If null, this does nothing.
        /// <param name="bindingFlags">Binding flags to filter properties to be set.
        /// The default is all static properties, including non-public, and private properties on base classes.
        /// <para>If <see cref="BindingFlagsExt.InheritedPrivate"/> is included, private properties (and properties with private setters) on base classes are recognised.</para>
        /// <para><see cref="BindingFlagsExt.Static"/> is treated as being set even if it is not set.</para>
        /// </param>
        /// <param name="inheritAttributes">true iff attributes inherited from the property on a base class are recognised.</param>
        public static void PopulateStaticDefaults(Type targetType, BindingFlagsExt bindingFlags = BindingFlagsExt.Public | BindingFlagsExt.NonPublic | BindingFlagsExt.Static | BindingFlagsExt.FlattenHierarchy | BindingFlagsExt.InheritedPrivate, bool inheritAttributes = true)
        {
            if(targetType != null)
                PopulateDefaultsInternal(targetType, targetType, bindingFlags | BindingFlagsExt.Static, inheritAttributes);
        }

        /// <summary>
        /// Set the values of all writeable properties that have a <see cref="DefaultValueAttribute"/> to
        /// the value specified by that attribute, on a type or instance.
        /// </summary>
        /// <param name="targetType">The type to set property values on. Must not be null.</param>
        /// <param name="target">The object to set property values on. If setting static properties only, this is the same as <paramref name="targetType"/>.
        /// Must not be null.
        /// </param>
        /// <param name="bindingFlags"><see cref="PopulateDefaults(object, BindingFlagsExt, bool)"/></param>
        /// <param name="inheritAttributes">true iff attributes inherited from the property on a base class are recognised.</param>
        private static void PopulateDefaultsInternal([NotNull] Type targetType, [NotNull]object target, BindingFlagsExt bindingFlags = BindingFlagsExt.Public | BindingFlagsExt.NonPublic | BindingFlagsExt.Static | BindingFlagsExt.FlattenHierarchy | BindingFlagsExt.InheritedPrivate, bool inheritAttributes = true)
        {
            if (bindingFlags.HasFlag(BindingFlagsExt.InheritedPrivate))  // if we setting inherited private properties
            {
                // The base class properties are done first, so if a property was matched by both scans, the overriding one would be set last.
                // But the same property should not be set twice, because this could potentially have undesired side effects in logic in its setter.

                foreach (var baseClass in ReflectionUtil.GetTypeHeirarchy(targetType).Where(t => t != targetType))  // for each superclass starting at the highest (not including the given class itself)
                {
                    // Set only the properties defined at this level in the type hierarchy that won't be set by the normal case (scanning targetType), so that properties are not assigned twice:
                    foreach (var property in baseClass.GetProperties(BindingFlags.NonPublic | BindingFlags.DeclaredOnly    // for each non-public property declared at this level
                                | (bindingFlags.BindingFlags() & (BindingFlags.Instance | BindingFlags.Static)))           // 'Instance and/or Static' same as the bindingFlags parameter
                        .Where(p => (p.GetMethod?.IsPrivate ?? false) && (p.SetMethod?.IsPrivate ?? false))                                       // with no non-private accessors
                        )      // if either accessor is non-private, it will be handled in the normal case below, and it could have its default value overridden below this level.
                    {
                        SetPropertyToDefaultInternal(property, target, inheritAttributes, setInheritedPrivate: false);
                        // setInheritedPrivate is false because, in cases where it would set a property, it property should already have been set on the base class in a previous iteration of this loop.
                    }
                }
            }

            // The normal case - all properties (including private if requested) visible on the given object:
            // If a property is not settable here, we look for a private setter on the base declaration of the property
            // (this won't have been handled above because we handled only private properties (no non-private accesssor) above).
            foreach (var property in targetType.GetProperties(bindingFlags.BindingFlags()))   // for each property of the given object (based on the runtime type)
            {
                SetPropertyToDefaultInternal(property, target, inheritAttributes, bindingFlags.HasFlag(BindingFlagsExt.InheritedPrivate));
            }
        }

        /// <summary>
        /// Set the value of the given property on the given instance to the value specified by
        /// the <see cref="DefaultValueAttribute"/> of the property.
        /// Does nothing if the property has no setter or does not have this attribute.
        /// </summary>
        /// <param name="property">The property to set. This does nothing if this is null.</param>
        /// <param name="target">The instance to set the property on. Must not be null.</param>
        /// <param name="inheritAttributes">true iff attributes inherited from the property on a base class are recognised.</param>
        /// <param name="setInheritedPrivate">Iff true and the property overrides one with a private setter,
        /// it is set using that.</param>
        /// <returns>true iff the property was set.</returns>
        public static bool SetPropertyToDefault(PropertyInfo property, object target, bool inheritAttributes = true, bool setInheritedPrivate = true)
        {
            if (target != null)
                return SetPropertyToDefaultInternal(property, target, inheritAttributes, setInheritedPrivate);
            else
                return false;
        }

        /// <summary>
        /// Set the value of the given property on the given instance to the value specified by
        /// the <see cref="DefaultValueAttribute"/> of the property.
        /// Does nothing if the property has no setter or does not have this attribute.
        /// </summary>
        /// <param name="property">The property to set. Must not be null.</param>
        /// <param name="target">The instance to set the property on. Must not be null.</param>
        /// <param name="inheritAttributes">true iff attributes inherited from the property on a base class are recognised.</param>
        /// <param name="setInheritedPrivate">Iff true and the property overrides one with a private setter,
        /// it is set using that.</param>
        /// <returns>true iff the property was set.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SetPropertyToDefaultInternal(PropertyInfo property, object target, bool inheritAttributes = true, bool setInheritedPrivate = true)
        {
            var attribute = property.GetCustomAttribute<DefaultValueAttribute>(inheritAttributes);
            if (attribute != null)               // if this property has a DefaultValueAttribute
            {
                if (property.CanWrite)   // if the property is not settable
                {
                    property.SetValue(target, GeneralTypeConverter.Convert(attribute.Value, property.PropertyType));
                    return true;
                }
                else     // if not settable, does it override a property with a private setter?
                {
                    if (setInheritedPrivate)
                    {
                        var baseProperty = ReflectionUtil.GetBaseDefinition(property);
                        if (baseProperty.CanWrite)       // try to set base class property
                        {
                            baseProperty.SetValue(target, GeneralTypeConverter.Convert(attribute.Value, property.PropertyType));
                            return true;
                        }
                    }
                }
            }
            return false;   // not set
        }

    }
}
