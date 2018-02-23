using JohnLambe.Util.Math;
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
    }

    public abstract class PropertyConfigBase
    {
        public virtual ClassConfig Parent { get; set; }

        public virtual string PropertyName { get; set; }

        public virtual PropertyInfo Property => Parent?.TargetClass?.GetProperty(PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public abstract object GenerateValue(IPropertyPopulaterContext context);
        //TODO: ability to indicate that value should not be assigned

        public virtual IRandomService RandomService { get; set; }
    }

    public class RandomChoicePropertyConfig : PropertyConfigBase
    {
        public override object GenerateValue(IPropertyPopulaterContext context)
        {
            return Values.ElementAt(RandomService.Next(Values.Count()));
            // Could optimise by evaluating Count() only once
        }

        public virtual IEnumerable<object> Values { get; set; }
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
            return context.GetRandomInstance(ValueType ?? Property.PropertyType);
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
