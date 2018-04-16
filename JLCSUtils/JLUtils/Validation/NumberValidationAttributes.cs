using JohnLambe.Util.MathUtilities;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace JohnLambe.Util.Validation
{
    // Validation/metadata of numeric values:

    /// <summary>
    /// Metadata or validation and display information for numeric values.
    /// </summary>
    public class NumberValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// Value of some properties to indicate that the number should not be rounded.
        /// </summary>
        public const int NoRounding = -1;

        /// <summary>
        /// Number of decimal places to show when displaying this value.
        /// <see cref="NoRounding"/> for floating point.
        /// </summary>
        public virtual int DecimalPlaces { get; set; } = NoRounding;

        /// <summary>
        /// Number of decimal places that the value should be rounded to on assignment.
        /// <see cref="NoRounding"/> to not round.
        /// </summary>
        public virtual int RoundTo { get; set; } = NoRounding;

        /// <summary>
        /// Specifies if and how digits are grouped.
        /// </summary>
        public virtual DigitGroupingOption DigitGrouping { get; set; } = DigitGroupingOption.Default;

//TODO:        public virtual int[] DigitGroups { get; set; }

        /// <summary>
        /// The lowest valid value (inclusive).
        /// </summary>
        public virtual double MinimumValue { get; set; } = double.MinValue;
        /// <summary>
        /// The highest valid value (inclusive).
        /// </summary>
        public virtual double MaximumValue { get; set; } = double.MaxValue;

        /// <summary>
        /// The value must be higher than this.
        /// If both this and <see cref="MinimumValue"/> are specified, both are applied (so the higher one is redundant).
        /// </summary>
        public virtual double GreaterThan { get; set; } = double.MinValue;
        /// <summary>
        /// The value must be lower than this.
        /// If both this and <see cref="MaximumValue"/> are specified, both are applied (so the lower one is redundant).
        /// </summary>
        public virtual double LessThan { get; set; } = double.MaxValue;


        /// <summary>
        /// The unit in which the value is measured: What a value of 1 represents.
        /// </summary>
        public virtual object Unit { get; set; }


        /// <summary>
        /// Iff true, out of range values are replaced with the closest in-range value.
        /// </summary>
        public virtual bool AdjustToRange { get; set; } = false;

        protected override void IsValid([Nullable] ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value != null)
            {
                if (RoundTo != NoRounding)
                {
                    value = MathUtil.Round(value, RoundTo);
                }
                double numericValue = GeneralTypeConverter.Convert<double>(value);
                if (numericValue < MinimumValue || numericValue <= GreaterThan)
                {
                    if (AdjustToRange)
                        value = MinimumValue;
                    else
                        results.Add(validationContext.DisplayName + " must be " 
                            + (MinimumValue < GreaterThan ? " at least " + MinimumValue : " higher then " + GreaterThan) + " " + Unit);
                }
                else if (numericValue > MaximumValue || numericValue >= LessThan)
                {
                    if (AdjustToRange)
                        value = MaximumValue;
                    else
                        results.Add(validationContext.DisplayName + " must be "
                            + (MaximumValue < LessThan ? " at most " + MinimumValue : " less then " + LessThan) + " " + Unit);
                }
            }
        }
    }

    /// <summary>
    /// Specifies how digits in a number are grouped (for groups separated by commas or spaces, etc.).
    /// </summary>
    public enum DigitGroupingOption  // refactor to objects ?
    {
        /// <summary>
        /// Whether and how digits are grouped is determined by a global/general setting or by the locale.
        /// </summary>
        Default = 0,
        /// <summary>
        /// No digit grouping.
        /// </summary>
        None = 1,
        /// <summary>
        /// Digits are grouped, using the normal digit grouping for the locale or application
        /// or determined by a global/general setting.
        /// </summary>
        Normal,

        /// <summary>
        /// Digits are grouped in threes (western convention), e.g. 1,000,000,000,000.
        /// </summary>
        Thousands,
        /// <summary>
        /// Grouped into lakh crore (10,00,00,00,00,000), crore (1,00,00,000) and lakh (1,00,000) (Indian digit grouping, widely used in South Asia).
        /// The least significant three digits are in one group. The rest are in groups of two digits.
        /// </summary>
        CroreLakh,

        Custom1 = 100,
        Custom2,
        Custom3,
        Custom4,

        // See https://en.wikipedia.org/wiki/Decimal_mark#Digit_grouping
    }

    /// <summary>
    /// Holds a percentage value.
    /// e.g. 0.5 means 50%.
    /// <para><see cref="NumberValidationAttribute.MinimumValue"/>, <see cref="NumberValidationAttribute.MaximumValue"/> and <see cref="NumberValidationAttribute.DecimalPlaces"/> are interpreted relating to the stored value.</para>
    /// </summary>
    public class PercentageValidationAttribute : NumberValidationAttribute
    {
        public override string GeneralDescription => "A percentage value";

        public override void PreProcessForDisplay(bool toDisplay, [Nullable] ref object value, [Nullable] ValidationContext validationContext)
        {
            if (toDisplay)
            {
                value = GeneralTypeConverter.Convert<decimal>(value) * 100;
            }
            else
            {
                value = GeneralTypeConverter.Convert<decimal>(value) / 100m;
            }
        }
    }

    /// <summary>
    /// A currency amount, in the base unit of the currency (e.g. dollar, euro, yen, yuan).
    /// </summary>
    public class CurrencyValidationAttribute : NumberValidationAttribute
    {
        public override string GeneralDescription => "A monetary amount";
    }

}
