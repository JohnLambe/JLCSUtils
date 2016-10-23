using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension
{
    /// <summary>
    /// Base class for attributes that specify something to be registered with the DI container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public abstract class DiRegisterAttributeBase : Attribute
    {
        /// <summary>
        /// Default value for the <see cref="Priority"/> property.
        /// </summary>
        public const int DefaultPriority = 20000;

        /// <summary>
        /// Name of the registered item in the DI container.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The (requested) type that resolves to the item being registered.
        /// </summary>
        public virtual Type ForType { get; set; }

        /// <summary>
        /// Items are regsitered in ascending order of this value.
        /// </summary>
        public virtual int Priority { get; set; } = DefaultPriority;
    }

    /// <summary>
    /// Registers a type mapping to the attributed class.
    /// </summary>
    public class DiRegisterTypeAttribute : DiRegisterAttributeBase
    {
    }

    /// <summary>
    /// Registers an instance of this type with the DI container.
    /// </summary>
    public class DiRegisterInstanceAttribute : DiRegisterAttributeBase
    {
    }
}
