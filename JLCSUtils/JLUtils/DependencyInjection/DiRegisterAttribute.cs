using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public abstract class DiRegisterAttributeBase : Attribute
    {
        public const int DefaultPriority = 20000;

        /// <summary>
        /// Name of the registered item in the DI container.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The (requested) type that resolves to the item being registered.
        /// </summary>
        public virtual Type ForType { get; set; }

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
