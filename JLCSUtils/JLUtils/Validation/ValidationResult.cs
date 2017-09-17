using JohnLambe.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

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
            ValidationAttribute attribute = null,
            object newValue = null)
            : base(errorMessage ?? " ", memberNames)
        {
            this.Type = type;
            this.NewValue = newValue;
            this.Attribute = attribute;
        }

        public virtual ValidationResultType Type { get; set; }

        /// <summary>
        /// Value that the value was changed to, by a validator.
        /// </summary>
        public virtual object NewValue { get; set; }

        /// <summary>
        /// The attribute that caused this validation message/result (if known).
        /// May be null. Validation results can be created without attributes.
        /// </summary>
        public virtual ValidationAttribute Attribute { get; }

        /// <summary>
        /// True iff validated successfully (even if there are warnings).
        /// </summary>
        public virtual bool IsValid
            => Type.IsValid();
    }


    /// <summary>
    /// Collection of validation results, with a summary.
    /// </summary>
    //| This does not inherit from ValidationResult (or ValidationResultEx) because
    //| - it's Message property is non-virtual (we would have to recalculate and assign it on every modification)
    //| - Success is indicated by a specific instance reference (ValidationResult.Success).
    //| It implements ICollection<ValidationResult>, which is used in some methods of System.ComponentModel.DataAnnotations.Validator.
    public class ValidationResults : ICollection<ValidationResult>
    {
        public ValidationResults(ValidationFeatures supportedFeatures = ValidationFeatures.All, ValidationResultType resultType = ValidationResultType.Error)
        {
            this.SupportedFeatures = supportedFeatures;
            this.ResultType = resultType;
        }

        public ValidationResults(ValidationResult result, ValidationFeatures supportedFeatures = ValidationFeatures.All, ValidationResultType resultType = ValidationResultType.Error)
            : this(supportedFeatures,resultType)
        {
            Add(result);
        }

        public virtual void Add(ValidationResult result)
        {
            var resultEx = result as ValidationResultEx;
            if (resultEx != null)
            {
                if (resultEx.Type == ValidationResultType.Updated)
                    NewValue = resultEx.NewValue;
            }
            _results.Add(result);
        }

        public virtual void Add(string message)
        {
            _results.Add(new ValidationResult(message));
        }

        public virtual ValidationAttribute Attribute { get; set; }

        public virtual void Fail()
        {            
            Add(Attribute?.ErrorMessage ?? "Validation failed");
            //TODO: message: Populate formatting placeholders in the string.
            //  Attribute?.FormatErrorMessage(  );
            //TODO: Message specified by resources.
        }

        public virtual ValidationResultType ResultType { get; set; }

        public virtual ValidationFeatures SupportedFeatures { get; set; }

        /// <summary>
        /// A single <see cref="ValidationResult"/> that summarises the collection.
        /// </summary>
        public virtual ValidationResult Result
        {
            get
            {
                return MergeResults(_results);
            }
        }

        /// <summary>
        /// Return a <see cref="ValidationResult"/> as a summary of all errors/warnings in a collection of <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public virtual ValidationResult MergeResults(ICollection<ValidationResult> results)
        {
            ValidationResultType mostSevereType = ValidationResultType.None;

            if (_results.Count == 0)
            {
                return ValidationResult.Success;
            }
            else if (_results.Count == 1
                && SupportedFeatures.HasFlag(ValidationFeatures.All) && ResultType == ValidationResultType.Error  // with these settings, any ValidationResult can be used as the summary result
                )
            {   //TODO: Could optimise by returning the actual object in some other cases
                return _results.First();
            }
            else
            {
                object newValue = null;

                StringBuilder message = new StringBuilder();
                foreach (var r in _results)
                {
                    ValidationResultType thisResultType = r is ValidationResultEx ?
                        ((ValidationResultEx)r).Type
                        : ValidationResultType.Error;
                    if (thisResultType > mostSevereType)
                        mostSevereType = thisResultType;
                    message.AppendLine(r.ErrorMessage);
                    if(r is ValidationResultEx)
                    {
                        newValue = (r as ValidationResultEx).NewValue ?? newValue;
                    }
                }

                // Apply ResultType:
                if (ResultType < mostSevereType)
                    mostSevereType = ResultType;

                // Adjust for SupportedFeatures:
                if (!SupportedFeatures.HasFlag(ValidationFeatures.Warnings) && mostSevereType < ValidationResultType.Error)  // if no Errors and warnings are not supported
                {
                    return ValidationResult.Success;
                }

                return new ValidationResultEx(message.ToString()) { Type = mostSevereType, NewValue = newValue };
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

        public virtual object NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                Modified = true;
            }
        }
        private object _newValue;

        public virtual bool Modified { get; protected set; } = false;  //TODO: Not populated yet

        /// <summary>
        /// True iff there are no errors in these results. (There may be warnings.)
        /// </summary>
        public virtual bool IsValid
            => !Results.Any(r => !r.IsValid());  // none invalid

        /// <summary>
        /// Throws an exception if there is a validation *error* (not on just warnings).
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

        #region ICollection<ValidationResult>

        public virtual void Clear()
        {
            _results.Clear();
        }

        public virtual bool Contains(ValidationResult item)
        {
            return _results.Contains(item);
        }

        public void CopyTo(ValidationResult[] array, int arrayIndex)
        {
            _results.CopyTo(array,arrayIndex);
        }

        public virtual bool Remove(ValidationResult item)
        {
            return _results.Remove(item);
        }

        public IEnumerator<ValidationResult> GetEnumerator() => _results.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _results.GetEnumerator();

        public virtual int Count => _results.Count;

        public virtual bool IsReadOnly => false;

        #endregion

        /// <summary>
        /// The collection of results. Never null, but may be empty.
        /// </summary>
        public virtual ICollection<ValidationResult> Results => _results;

        /// <summary><see cref="Results"/></summary>
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

/*
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
//                throw new ValidationException(Result,null,ToString());  //TODO
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
*/

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
