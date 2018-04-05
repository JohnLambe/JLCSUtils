using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;
using System.ComponentModel;
using System.Reflection;
using JohnLambe.Util.Documentation;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Specifies that certain attributes of a referenced item should be applied to the attributed item.
    /// Except for <see cref="ValidationAttribute"/>, this requires specific support in all tools.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ReferenceAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="sourceType"><see cref="SourceType"/></param>
        /// <param name="sourcePropertyName"><see cref="SourcePropertyName"/></param>
        /// <param name="source"><paramref name="source"/></param>
        public ReferenceAttribute(Type sourceType, string sourcePropertyName = null, DocumentationSource source = DocumentationSource.AllReferenceable)
        {
            this.SourceType = sourceType;
            this.SourcePropertyName = sourcePropertyName;
            this.Source = source;
            _sourceMember = GetSourceProvider(SourceType, SourcePropertyName);
        }

        /// <summary>
        /// The type on which to referece a property.
        /// </summary>
        public virtual Type SourceType { get; }

        /// <summary>
        /// The name of the property of <see cref="SourceType"/> to reference.
        /// The property can be instance of static, and can be non-public if the code using it has rights to access non-public members by Reflection.
        /// null if a type is referenced.
        /// </summary>
        public virtual string SourcePropertyName { get; }

        /// <summary>
        /// What details to use from the referenced item.
        /// </summary>
        public virtual DocumentationSource Source { get; }

        protected override void IsValid([Nullable(null)] ref object value, [NotNull(null)] ValidationContext validationContext, [NotNull(null)] ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            var sourceObject = _sourceMember.Value;
            if (sourceObject != null)
            {
                foreach (var attribute in sourceObject.GetCustomAttributes<ValidationAttribute>(true))
                {
                    //TODO: Validate attribute
                    /*
                    if (attribute is ValidationAttributeBase)
                    {
                        var att = attribute as ValidationAttributeBase;
                        att.IsValid(ref value, validationContext, results);

                    }
                    else
                    */
                    {
                        var result = attribute.GetValidationResult(value, validationContext);
                        if (result != ValidationResult.Success)
                            results.Add(result);
                    }
                }
            }
            else
            {
                results.Add("INTERNAL ERROR: Invalid property reference: " + (SourceType?.GetType()?.FullName ?? "<null>") + "." + SourcePropertyName + "; Value: " + value);
            }
        }

        [NotNull]
        protected readonly Lazy<MemberInfo> _sourceMember;

        //TODO: Properties to look up documentation-related attributes of source.

        public static Lazy<MemberInfo> GetSourceProvider(Type SourceType, string SourceProperty)
        {
            if (SourceProperty == null)
                return new Lazy<MemberInfo>(() => SourceType);
            else
                return new Lazy<MemberInfo>(() => SourceType.GetProperty(SourceProperty, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
        }
    }


    /// <summary>
    /// Applies a <see cref="DescriptionAttribute"/> from another type or property to the attributed item.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]  // Doesn't allow multiple because DescriptionAttribute doesn't.
    public class ReferenceDescriptionAttribute : DescriptionAttribute
    {
        public ReferenceDescriptionAttribute(Type sourceType, string sourceProperty = null)
        {
            this.SourceType = sourceType;
            this.SourcePropertyName = sourceProperty;
            _source = ReferenceAttribute.GetSourceProvider(SourceType, SourcePropertyName);
        }

        /// <inheritdoc cref="ReferenceAttribute.SourceType"/>
        [NotNull]
        public virtual Type SourceType { get; }

        /// <inheritdoc cref="ReferenceAttribute.SourcePropertyName"/>
        [Nullable]
        public virtual string SourcePropertyName { get; }

        public override string Description
        {
            get { return _source.Value?.GetCustomAttribute<DescriptionAttribute>().Description; }
        }

        public override bool IsDefaultAttribute() => Description != "";

        [NotNull]
        protected readonly Lazy<MemberInfo> _source;
    }

    //TODO: ReferenceDisplayAttribute
}
