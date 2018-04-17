using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validation attribute that compares the attributed property to another property on the same instance.
    /// </summary>
    public class ComparePropertyValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The name of the property to compare to.
        /// This can support nested properties in the format of <see cref="ReflectionUtil.TryGetPropertyValue{T}(object, string, PropertyNullabilityModifier)"/>.
        /// </summary>
        [PropertyName]
        public virtual string PropertyName { get; set; }

        //public virtual bool Not { get; set; }

        public virtual ComparisonOperator Operator { get; set; } = ComparisonOperator.Any;

        //public virtual StringComparisonOperator StringOperator { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            var instance = validationContext.ObjectInstance;
            var property = ReflectionUtil.GetProperty(ref instance, PropertyName);  // get the property to compare to

            if(property == null)
            {
                results.Add("Invalid property name: " + PropertyName);
            }
            
            var propertyValue = property.GetValue(instance);

            if(Operator != ComparisonOperator.Any)
            {
                if(!Operator.Compare(value, propertyValue))
                {
                    results.Add(validationContext.DisplayName + " must be " + Operator.DisplayName() + " " + CaptionUtil.GetDisplayName(property));
                }
            }
        }
    }

    [Flags]
    public enum ComparisonOperator
    {
        LessThan = 1,
        Equal = 2,
        GreaterThan = 4,

        NotEqual = LessThan | GreaterThan,
        LessThanOrEqual = LessThan | Equal,
        GreaterThanOrEqual = GreaterThan | Equal,
        /// <summary>
        /// Always true.
        /// </summary>
        Any = LessThan | Equal | GreaterThan,
        /// <summary>
        /// Always false.
        /// </summary>
        None = 0
    }

    public static class ComparisonOperatorExtension
    {
        public static bool Compare(this ComparisonOperator op, object a, object b)
        {
            if (op == ComparisonOperator.Any)
                return true;
            else if (op == ComparisonOperator.None)
                return false;

            int? compareStatus = ObjectUtil.TryCompare(a,b);
            if (compareStatus == null)
            {
                if (op == ComparisonOperator.NotEqual)
                    return true;
                else if (op == ComparisonOperator.Equal)
                    return false;
                else
                    throw new InvalidOperationException("Can't compare " + (a?.GetType().FullName ?? "null") + " to " + (b?.GetType().FullName ?? "null"));
            }
            else
            {
                bool valid = false;
                if (op.HasFlag(ComparisonOperator.Equal))
                    valid = valid || compareStatus == 0;
                if (op.HasFlag(ComparisonOperator.LessThan))
                    valid = valid | compareStatus < 0;
                if (op.HasFlag(ComparisonOperator.GreaterThan))
                    valid = valid | compareStatus > 0;
                return valid;
            }
        }

        public static string DisplayName(this ComparisonOperator op)
        {
            switch(op)
            {
                case ComparisonOperator.LessThan:
                    return "less than";
                case ComparisonOperator.LessThanOrEqual:
                    return "less than or equal to";
                case ComparisonOperator.Equal:
                    return "equal to";
                case ComparisonOperator.GreaterThan:
                    return "greater than";
                case ComparisonOperator.GreaterThanOrEqual:
                    return "greater than or equal to";
                case ComparisonOperator.NotEqual:
                    return "not equal to";
                default:
                    return "";
            }
        }
    }

    public enum StringComparisonOperator
    {
        Contains = 1,
        StartsWith,
        EndsWith,
    }

    public static class StringComparisonOperatorExtension
    {
        public static bool Compare(this StringComparisonOperator op, string a, string b)
        {
            switch(op)
            {
                case StringComparisonOperator.Contains:
                    return a.Contains(b);
                case StringComparisonOperator.StartsWith:
                    return a.StartsWith(b);
                case StringComparisonOperator.EndsWith:
                    return a.EndsWith(b);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
