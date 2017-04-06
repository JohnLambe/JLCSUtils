using JohnLambe.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    public class ValidationResultEx : ValidationResult
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.DataAnnotations.ValidationResult
        //     class by using a System.ComponentModel.DataAnnotations.ValidationResult object.
        //
        // Parameters:
        //   validationResult:
        //     The validation result object.
        protected ValidationResultEx(ValidationResult validationResult) : base(validationResult)
        { }

        public ValidationResultEx(
            string errorMessage,
            IEnumerable<string> memberNames = null,
            ValidationResultType type = ValidationResultType.Error,
            object newValue = null)
            : base(errorMessage, memberNames)
        {
            this.Type = type;
            this.NewValue = newValue;
        }

        public virtual ValidationResultType Type { get; set; }

        public virtual object NewValue { get; set; }

        public virtual bool IsValid
            => Type.IsValid();
    }


    public class ValidationResultCollection
    {
        public ValidationResultCollection(ValidationFeatures supportedFeatures = ValidationFeatures.All, ValidationResultType resultType = ValidationResultType.Error)
        {
            this.SupportedFeatures = supportedFeatures;
            this.ResultType = resultType;
        }

        public virtual void Add(ValidationResult result)
        {
            _results.Add(result);
        }

        public virtual void Add(string message)
        {
            _results.Add(new ValidationResult(message));
        }

        public virtual void Fail()
        {
            //TODO
        }

        public virtual ValidationResultType ResultType { get; set; }

        public virtual ValidationFeatures SupportedFeatures { get; set; }

        public virtual ValidationResult Result
        {
            get
            {
                return MergeResults(_results);
            }
        }

        public virtual ValidationResult MergeResults(ICollection<ValidationResult> results)
        {
            ValidationResultType mostSevereType = ValidationResultType.None;

            if (_results.Count == 0)
            {
                return ValidationResult.Success;
            }
            else if (_results.Count == 1)
            {
                return _results.First();
            }
            else
            {
                StringBuilder message = new StringBuilder();
                foreach (var r in _results)
                {
                    ValidationResultType thisResultType = r is ValidationResultEx ?
                        ((ValidationResultEx)r).Type
                        : ValidationResultType.Error;
                    if (thisResultType > mostSevereType)
                        mostSevereType = ((ValidationResultEx)r).Type;
                    message.AppendLine(r.ErrorMessage);
                }
                return new ValidationResultEx(message.ToString()) { Type = mostSevereType };
            }
            //TODO: Apply ResultType
            //TODO: SupportedFeatures
        }

        protected IList<ValidationResult> _results = new List<ValidationResult>();
    }

    public enum ValidationResultType
    {
        None = 0,

        Updated = 10,
        /// <summary>
        /// Non-warning informational message.
        /// </summary>
        Message = 20,
        Warning = 60,
        SevereWarning = 80,
        /// <summary>
        /// A value is invalid.
        /// </summary>
        Error = 100
    }

    public class ValidationResults
    {
        public virtual ICollection<ValidationResult> Results { get; }
             = new List<ValidationResult>();

        public virtual object NewValue { get; protected set; }

        public virtual bool Modified { get; protected set; } = false;

        /// <summary>
        /// True iff there are no errors in these results. (There may be warnings.)
        /// </summary>
        public virtual bool IsValid
            => !Results.Any(r => r is ValidationResultEx ? ((ValidationResultEx)r).IsValid : true);

        /// <summary>
        /// Throws an exception if there is a validation error (not on just warnings).
        /// </summary>
        /// <exception cref="ValidationException">If invalid.</exception>
        public virtual void ThrowIfInvalid()
        {
            if (!IsValid)
            {

                throw new ValidationException(ToString());  //TODO
//                throw new UserErrorException(ToString());  //TODO
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var r in Results)
            {
                result.AppendLine(r.ToString());
            }
            //TODO: Append overall result
            return result.ToString();
        }
    }


    /// <summary>
    /// Extension methods for using <see cref="ValidationResult"/> with this framework.
    /// </summary>
    public static class ValidationResultExtension
    {
        /// <summary>
        /// True iff validation succeeded (there may be warnings).
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool IsValid(this ValidationResult r)
        {
            if (r is ValidationResultEx)
                return ((ValidationResultEx)r).IsValid;
            else
                return r == ValidationResult.Success;
        }

        /// <summary>
        /// True if the given value does NOT indicate an error (it may be a warning).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValid(this ValidationResultType type)
        {
            switch (type)
            {
                case ValidationResultType.Error:
                    return false;
                default:
                    return true;
            }
        }
    }


}
