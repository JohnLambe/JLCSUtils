using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util;

namespace JohnLambe.Util.Validation
{
    [Flags]
    public enum ComparisonOperator
    {
        /// <summary>
        /// true if the first operand has a lower value than the second.
        /// </summary>
        LessThan = 1,

        /// <summary>
        /// true if the operands are equal (according to their comparison logic).
        /// </summary>
        Equal = 2,

        /// <summary>
        /// true if the first operand has a higher value than the second.
        /// </summary>
        GreaterThan = 4,

        // The remaining options are combinations of the above:

        /// <summary>
        /// true if the operands are NOT equal (according to their comparison logic).
        /// </summary>
        NotEqual = LessThan | GreaterThan,

        /// <summary>
        /// true if the first operand has a value that is lower than or equal to the value of the second operand (according to their comparison logic).
        /// </summary>
        LessThanOrEqual = LessThan | Equal,

        /// <summary>
        /// true if the first operand has a value that is higher than or equal to the value of the second operand (according to their comparison logic).
        /// </summary>
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

            int? compareStatus = ObjectUtil.TryCompare(a, b);
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
            switch (op)
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
}
