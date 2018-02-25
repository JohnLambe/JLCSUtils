using JohnLambe.Util.Collections;
using JohnLambe.Util.Math;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.DataPopulater
{
    public interface IPropertyPopulaterContext
    {
        object GetRandomInstance(Type type);

        Type RequiredType { get; set; }
    }

    public abstract class PropertyConfigBase
    {
        public virtual ClassConfig Parent { get; set; }

        public virtual string PropertyName { get; set; }

        /*
        public virtual PropertyInfo Property =>
            ReflectionUtil.GetPropertyFromType(Parent?.TargetClass, PropertyName);
//            Parent?.TargetClass?.GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The value to use, or <see cref="NoValue"/> if the property should not be assigned.</returns>
        public abstract object GenerateValue(IPropertyPopulaterContext context);

        public virtual IRandomService RandomService { get; set; }

        public static object NoValue => _noValue;
        private static readonly object _noValue = new object();
    }

    public class RandomChoicePropertyConfig : PropertyConfigBase
    {
        public override object GenerateValue(IPropertyPopulaterContext context)
        {
            var count = Values.Count();
            // Could optimise by evaluating Count() only once
            if (count > 0)
            {
                var index = RandomService.Next(0,count);  // 0-based index ([0..count-1])
                var value = Values.ElementAt(index);
                if (!AllowDuplicates)
                    Values = CollectionUtil.Remove(Values, value);
                return value;
            }
            else
            {
                return NoValue;
            }
        }

        public virtual IEnumerable<object> Values { get; set; }

        public virtual bool AllowDuplicates { get; set; } = true;

        public virtual object DefaultValue { get; set; } = NoValue;
    }

    public class ConstantPropertyConfig : PropertyConfigBase
    {
        public override object GenerateValue(IPropertyPopulaterContext context)
        {
            return Value;
        }

        public object Value { get; set; }
    }

    public class GetInstancePropertyConfig : PropertyConfigBase
    {
        public override object GenerateValue(IPropertyPopulaterContext context)
        {
            return context.GetRandomInstance(ValueType ?? context.RequiredType);
        }

        public virtual Type ValueType { get; set; }
    }

    /// <summary>
    /// Generate a random value in a given range, or a string of values in a given range.
    /// </summary>
    public class RandomRangePropertyConfig : PropertyConfigBase
    {
        public override object GenerateValue(IPropertyPopulaterContext context)
        {
            int length = RandomService.Next(MinimumLength,MaximumLength);

            StringBuilder builder = new StringBuilder(length);
            for(int n = 0; n < length; n++)
            {
                if (MinimumValue is char)
                    builder.Append(RandomService.RandomChar((char)MinimumValue, (char)MaximumValue));
                else
                    builder.Append(RandomService.Next((int)MinimumValue, (int)MaximumValue));
            }
            return builder;
        }

        public object MinimumValue { get; set; }
        public object MaximumValue { get; set; }

        public int MinimumLength { get; set; } = 0;
        public int MaximumLength { get; set; } = 1;
    }
}
