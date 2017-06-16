using JohnLambe.Util.Reflection;
using JohnLambe.Util.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// Base class for attributes for defining mappings between exceptions and related types.
    /// </summary>
    public abstract class ExceptionMappingAttribute : Attribute
    {
    }

    /// <summary>
    /// Associates a class (such as a dialog type) with an exception type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = true,            // Mutlitple exceptions can be mapped to the same type.
        Inherited = false)]              // Not inherited because the exception is mapped to a specific level in the inheritance heirarchy.
    public class MappedExceptionAttribute : ExceptionMappingAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="exceptionClass"><see cref="ExceptionClass"/></param>
        public MappedExceptionAttribute(Type exceptionClass)
        {
            this.ExceptionClass = exceptionClass;
        }

        /// <summary>
        /// The exception type being mapped.
        /// Must be <see cref="System.Exception"/> or a subclass of it.
        /// </summary>
        [TypeValidation(TypeOrSubclassOf = typeof(Exception))]
        public virtual Type ExceptionClass
        {
            get { return _exceptionClass; }
            set
            {
                if (!value.IsTypeOrSubclassOf(typeof(Exception)))
                    throw new ArgumentException(GetType().Name + "." + nameof(ExceptionClass) + " must be a subclass of Exception ");
                _exceptionClass = value;
            }
        }
        private Type _exceptionClass;
    }

    /// <summary>
    /// Associates an exception type with a dialog type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false,           // Exception can be mapped to only one dialog type
        Inherited = false)]              // Not inherited because the mapping is at a specific level in the inheritance heirarchy.
    public class MappedDialogTypeAttribute : ExceptionMappingAttribute
    {
        public MappedDialogTypeAttribute(Type dialogType)
        {
            this.DialogType = dialogType;
        }

        public virtual Type DialogType { get; set; }
    }


    /// <summary>
    /// Associates a <see cref="MessageDialogModel{TResult}"/> subtype with a dialog type,
    /// as a default type to use for that dialog type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false,           // Exception can be mapped to only one dialog type
        Inherited = false)]              // Not inherited because the mapping is at a specific level in the inheritance heirarchy.
    public class DefaultDialogModelTypeAttribute : Attribute
    {
        public DefaultDialogModelTypeAttribute(Type dialogType)
        {
            this.DialogType = dialogType;
        }

        /// <summary>
        /// The associated <see cref="MessageDialogModel{TResult}"/> type.
        /// null to explicitly have no mapping.
        /// </summary>
        public virtual Type DialogType { get; set; }
    }
}
