using System;
using System.Collections.Generic;

using System.Reflection;

using JohnLambe.Util.Math;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class MapTypeAttribute : Attribute
    {
        public virtual Type SourceType { get; set; }
    }


    /// <summary>
    /// Property mapping for ObjectCopier.
    /// </summary>
    public class MapAttribute : Attribute
    {
        public MapAttribute()
        {
        }

        public MapAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of source property.
        /// Overrides the name of the destination property.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// If not null, Flag enum constant (or equivalent integer value) in the source property.
        /// There can be more than one set bit.
        /// The result is a boolean - true if any bit in this mask is set.
        /// When used, the source property must be an enum or integer type.
        /// </summary>
        public virtual object Flag { get; set; }

        /// <summary>
        /// Bit mask.
        /// May be an enum constant that provides the bit mask.
        /// The source property is ANDed with this.
        /// When used, the source property must be an enum or integer type.
        /// Must not be used with Flag.
        /// </summary>
        public virtual object Mask { get; set; }

        /// <summary>
        /// Iff true, the value is shifted to make the move the first set bit
        /// in Mask to bit 0.
        /// Valid only when Mask is not null.
        /// </summary>
        public virtual bool Shift { get; set; }
    }


    /// <summary>
    /// Copies all compatible, similarly named properties from one object to another.
    /// </summary>
    public class ObjectCopier
    {

        public virtual void Copy(object source, object destination)
        {
            Copy(source, destination, true);
        }

        /// <summary>
        /// Copy properties of an object to another (given) object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="ignoreErrors">Iff true, errors on assigning properties are ignored.
        /// Otherwise an exception is raised on the first one.</param>
        /// <returns></returns>
        public virtual void Copy(object source, object destination, bool ignoreErrors)
        {
            var mapping = GetMapping(source.GetType(), destination.GetType());
            mapping.Copy(source, destination);
        }

        /// <summary>
        /// Creates the destination object, then copies to it using Copy. 
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The copied object.</returns>
        public virtual T CreateCopy<T>(object source)
        {
            object destination = typeof(T).GetConstructor(new Type[] { }).Invoke(new Object[] { });
            Copy(source, destination);
            return (T)destination;
        }

        public virtual IClassMapping GetMetadata(Type sourceType, Type destinationType)
        {
            return GetMapping(sourceType, destinationType);
        }
                
        #region Creating mappings

        /// <summary>
        /// Explicitly create a mapping (for use in subsequent copy operations).
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        public virtual void CreateMapping(Type sourceType, Type destinationType = null)
        {
            GetMapping(sourceType, destinationType);
        }

        /// <summary>
        /// Get a mapping for the given classes, creating it if there isn't one already.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        protected virtual CopyMappings GetMapping(Type sourceType, Type destinationType)
        {
            CopyMappings mapping = null;
            if(!_mappings.TryGetValue(sourceType, out mapping))
            {
                mapping = GenerateMapping(sourceType, destinationType);
            }
            return mapping;
        }

        protected virtual CopyMappings GenerateMapping(Type sourceType, Type destinationType)
        {
            object NoProperty = new object();  // assigned to value to indicate that there is no matching destination property
            var mapping = new CopyMappings();

            foreach (var destinationProperty in destinationType.GetProperties())
            {
                if (destinationProperty.CanWrite)
                {
                    object value = NoProperty;

                    if (value == NoProperty)
                    {
                        int mask = 0;
                        bool shift = false;
                        bool isFlag = false;

                        var attribute = destinationProperty.GetCustomAttribute(typeof(MapAttribute)) as MapAttribute;
                        string destinationPropertyName = null;
                        if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                            destinationPropertyName = attribute.Name;
                        else
                            destinationPropertyName = destinationProperty.Name;
                        var sourceProperty = sourceType.GetProperty(destinationPropertyName);
                        if (sourceProperty != null && sourceProperty.CanRead)
                        {
                            if (attribute != null)
                            {
                                if (attribute.Flag != null)
                                {
                                    mask = (int)attribute.Flag;
                                    isFlag = true;
                                }
                                else if (attribute.Mask != null)
                                {
                                    mask = (int)attribute.Mask;
                                    shift = attribute.Shift;
                                }
                            }
                            if (mask == 0)
                            {
                                mapping.PropertyCopyMappings.Add(new SimplePropertyMapping()
                                {
                                    Source = sourceProperty,
                                    Destination = destinationProperty
                                });
                            }
                            else if (isFlag)
                            {
                                mapping.PropertyCopyMappings.Add(new FlagPropertyMapping()
                                {
                                    Source = sourceProperty,
                                    Destination = destinationProperty,
                                    Mask = mask
                                });
                            }
                            else
                            {
                                mapping.PropertyCopyMappings.Add(new MaskPropertyMapping()
                                {
                                    Source = sourceProperty,
                                    Destination = destinationProperty,
                                    Mask = mask,
                                    Shift = MathUtil.LowSetBit((uint)mask)
                                });
                            }
                        }
                    }
                }
            }
            return mapping;
        }

        #endregion

        #region Property mapping classes

        protected class CopyMappings : IClassMapping
        {
            public CopyMappings()
            {
                PropertyCopyMappings = new List<PropertyMapping>();
            }

            public Type SourceType { get; set; }
            public List<PropertyMapping> PropertyCopyMappings { get; protected set; }
            public IEnumerable<IPropertyMapping> PropertyMappings
            {
                get { return PropertyCopyMappings; }
            }

            public void Copy(object source, object destination)
            {
                foreach (var mapping in PropertyCopyMappings)
                {
                    mapping.CopyProperty(source, destination);
                }
            }
        }

        protected abstract class PropertyMapping : IPropertyMapping
        {
            public PropertyInfo Source { get; set; }

            public PropertyInfo Destination { get; set; }

            public abstract void CopyProperty(object source, object destination);
            /*
                        public virtual void CopyProperty(object source, object destination)
                        {
                            object value = Source.GetValue(source);
                            if(Mask != 0)
                            {
                                value = (((int)value) & Mask) >> Shift;
                            }
                            Destination.SetValue(destination,value);
                        }
             */
        }

        protected abstract class MaskedPropertyMapping : PropertyMapping
        {
            /// <summary>
            /// If not 0: Bit mask of source property. 
            /// Source property must be an integer.
            /// </summary>
            public int Mask { get; set; }
        }

        /// <summary>
        /// Mapping where the destination is boolean and the source is a bitfield.
        /// </summary>
        protected class FlagPropertyMapping : MaskedPropertyMapping
        {
            public override void CopyProperty(object source, object destination)
            {
                int value = (int)Source.GetValue(source);
                Destination.SetValue(destination, (((int)value) & Mask) != 0);
            }
        }

        /// <summary>
        /// Mapping where source is one or more bits from an integer/enum type, possibly shifted.
        /// </summary>
        protected class MaskPropertyMapping : MaskedPropertyMapping
        {
            /// <summary>
            /// If Mask is not 0, the value is shifted right by this after ANDing with Mask.
            /// </summary>
            public int Shift { get; set; }

            public override void CopyProperty(object source, object destination)
            {
                object value = Source.GetValue(source);
                value = (((int)value) & Mask) >> Shift;
                Destination.SetValue(destination, value);
            }
        }

        /// <summary>
        /// Mapping where source and destination are assignment-compatible simple types.
        /// </summary>
        protected class SimplePropertyMapping : PropertyMapping
        {
            public override void CopyProperty(object source, object destination)
            {
                object value = Source.GetValue(source);
                Destination.SetValue(destination, value);
            }
        }

        #endregion

        protected Dictionary<Type, CopyMappings> _mappings = new Dictionary<Type, CopyMappings>();
    }

}
