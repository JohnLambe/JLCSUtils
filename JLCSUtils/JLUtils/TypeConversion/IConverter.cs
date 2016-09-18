using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TypeConversion
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource">Type that the converter converts from.</typeparam>
    /// <typeparam name="TDest">Type that the converter converts to.</typeparam>
    public interface IConverter<TSource, TDest>
    {
        /// <summary>
        /// Try to convert the value.
        /// 
        /// </summary>
        /// <returns>The status of the conversion.
        /// See the comments on each value of this type.
        /// Must not return <see cref="ConversionResult.None"/>.
        /// </returns>
        /// <exception cref="Exception">If any except is thrown, the converion is failed.
        /// The caller should behave as if <see cref="ConversionResult.ConversionFailed"/> was returned.
        /// </exception>
        ConversionResult Convert(TSource source, out TDest destination, Type requiredType);
    }

    [Flags]
    public enum ConversionResult
    {
        None = 0,

        /// <summary>
        /// Succeeded.
        /// Not valid in combination with any other options.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Failed
        /// (but other converters should be tried if the Continue bit is set).
        /// Not valid in combination with <see cref="Success"/>.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Set if other converters should be tried.
        /// If this is set without <see cref="Failed"/>, the value returned by this converter should be passed to subsequent ones.
        /// </summary>
        Continue = 4,

        // As a full value:

        /// <summary>
        /// Failed and no other converters should be tried.
        /// Converter determines that the conversion is inherently invalid.
        /// </summary>
        NotConvertible = Failed,

        /// <summary>
        /// Failed, but other converters should be tried:
        /// This converter cannot do the conversion.
        /// </summary>
        ConversionFailed = Failed | Continue
    }
}
