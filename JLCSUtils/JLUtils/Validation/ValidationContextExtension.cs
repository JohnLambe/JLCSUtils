using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Extension methods and related items for using <see cref="ValidationContext"/> with this framework.
    /// </summary>
    public static class ValidationContextExtension
    {
        /// <summary>
        /// Key for the item in <see cref="ValidationContext.Items"/> that holds
        /// the <see cref="GetSupportedFeatures(ValidationContext)"/> value.
        /// </summary>
        private const string Key_SupportedFeatures = "SupportedFeatures";

        /// <summary>
        /// Gets a value indicating which features are supported by the system.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        //| This is implemented as an extension method because ValidationContext is sealed (otherwise, we would have subclassed it).
        public static ValidationFeatures GetSupportedFeatures(this ValidationContext context)
        {
            return (ValidationFeatures)context.Items[Key_SupportedFeatures];
        }

        /// <summary>
        /// Set the supported features.
        /// <seealso cref="GetSupportedFeatures(ValidationContext)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="features"></param>
        //| This is not an extension method, since most consumers of ValidationContext should not use it.
        //| It is set by the framework (and potentially by third party extensions of this framework, which is why it is public) only.
        public static void SetSupportedFeatures(ValidationContext context, ValidationFeatures features)
        {
            context.Items[Key_SupportedFeatures] = features;
        }

        /// <summary>
        /// Value of the <see cref="ValidationContext.ObjectInstance"/> when validating .
        /// </summary>
        public static readonly object NoObject = new object();
    }


    /// <summary>
    /// Features that may be supported by a validation system.
    /// </summary>
    [Flags]
    public enum ValidationFeatures
    {
        None = 0,

        All = Warnings | Modification | ValidateWithoutObject,

        /// <summary>
        /// The system that the item is being validated for supports warning messages in <see cref="ValidationResultEx"/>.
        /// If this is false, warnings must not be returned. Systems that are unaware of the feature will treat them as errors.
        /// </summary>
        Warnings = 1,

        /// <summary>
        /// The system that the item is being validated for supports changing the value being validated.
        /// If false, changing the value will not work. The modified value may be passed to later validators, but will not replace the actual underlying value.
        /// </summary>
        Modification = 2,

        /// <summary>
        /// Validators may be passed a context with no object instance, to validate just a value
        /// for assignment to a property.
        /// <seealso cref="ValidationContextExtension.NoObject"/>
        /// </summary>
        ValidateWithoutObject
    }

}
