using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validation of collections.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
        AllowMultiple = false, Inherited = true)]
    public class CollectionValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Whether the elements of the collection can be null.
        /// </summary>
        public virtual bool ElementsNullable { get; set; } = true;

        /// <summary>
        /// The minimum number of elements in the collection.
        /// </summary>
        public virtual int MinimumSize { get; set; } = 0;

        /// <summary>
        /// The maximum number of elements in the collection.
        /// -1 for unlimited.
        /// </summary>
        public virtual int MaximumSize { get; set; } = -1;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var collection = value as ICollection<object>;
                int index = -1;
                if (!ElementsNullable)
                {
                    index = 0;
                    foreach (var element in collection)
                    {
                        index++;
                        if (element == null)
                            return new ValidationResult("Element cannot be null: Index " + index + ": " + element.ToString()
                                + " in " + value.ToString());
                    }
                }
                // `index` is now -1 or the collection size

                int size = index;
                if (MinimumSize > 0 || MaximumSize > -1)
                {
                    // size is needed:
                    if (index > -1)
                        size = index;
                    else
                        size = collection.Count();

                    if (size < MinimumSize)
                        return new ValidationResult("Collection too small: Required minimum size: " + MinimumSize
                            + "; Actual size: " + size + "; Value: " + value.ToString());
                    if (size > MaximumSize)
                        return new ValidationResult("Collection too big: Maximum size: " + MinimumSize
                            + "; Actual size: " + size + "; Value: " + value.ToString());
                }
            }

            return base.IsValid(value,validationContext);
        }
    }
}
