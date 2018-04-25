using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util;
using JohnLambe.Util.Validation;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
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
            switch (op)
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
