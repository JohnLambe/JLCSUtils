using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Types;

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
        /// Key for the item in <see cref="ValidationContext.Items"/> that holds
        /// the <see cref="GetState(ValidationContext)"/> value.
        /// </summary>
        private const string Key_State = "State";

        /// <summary>
        /// Gets a value indicating which features are supported by the system.
        /// </summary>
        /// <param name="context">The validation context. If null, <see cref="ValidationFeatures.None"/> is returned.</param>
        /// <returns>The supported features value, or <see cref="ValidationFeatures.None"/> if <paramref name="context"/> is null.</returns>
        //| This is implemented as an extension method because ValidationContext is sealed (otherwise, we would have subclassed it).
        public static ValidationFeatures GetSupportedFeatures([Nullable] this ValidationContext context)
        {
            return (ValidationFeatures)(context?.Items.TryGetValue(Key_SupportedFeatures) ?? ValidationFeatures.None);
        }

        /// <summary>
        /// Set the supported features.
        /// <seealso cref="GetSupportedFeatures(ValidationContext)"/>
        /// </summary>
        /// <param name="context">The validation context. Must not be null.</param>
        /// <param name="features"></param>
        //| This is not an extension method, since most consumers of ValidationContext should not use it.
        //| It is set by the framework (and potentially by third party extensions of this framework, which is why it is public) only.
        public static void SetSupportedFeatures(ValidationContext context, ValidationFeatures features)
        {
            context.ArgNotNull(nameof(context)).Items[Key_SupportedFeatures] = features;
        }

        /// <summary>
        /// Get the state (<see cref="ValidationState"/>) value.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationState GetState([Nullable] this ValidationContext context)
        {
            return (ValidationState)(context?.Items.TryGetValue(Key_State) ?? ValidationState.Default);
        }

        /// <summary>
        /// Set the validation state.
        /// <seealso cref="GetSupportedFeatures(ValidationContext)"/>
        /// </summary>
        /// <param name="context">The validation context. Must not be null.</param>
        /// <param name="state"></param>
        //| This is not an extension method, since most consumers of ValidationContext should not use it.
        //| It is set by the framework (and potentially by third party extensions of this framework, which is why it is public) only.
        public static void SetState(ValidationContext context, ValidationState state)
        {
            context.ArgNotNull(nameof(context)).Items[Key_State] = state;
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
        ValidateWithoutObject = 4
    }

    /// <summary>
    /// State or mode settings of the validation engine.
    /// </summary>
    [Flags]
    public enum ValidationState
    {
        Default = 0,
        /// <summary>
        /// Validation is being done immediately after a user has entered or modified the data.
        /// Some validation rules may be applied only at the time on entering the information: e.g. a date may be required to be in the future at the time it is entered.
        /// </summary>
        LiveInput = 1,
        /// <summary>
        /// A whole object is being validated, not just a property.
        /// </summary>
        WholeObject = 2,
        /// <summary>
        /// A non-final validation. The user will continue editing the item after this, and a final validation (without this flag) will
        /// be done before saving/submitting the data. (Some items may be allowed to be invalid or blank at this stage.)
        /// </summary>
        ContinuingInput = 4
    }

}
