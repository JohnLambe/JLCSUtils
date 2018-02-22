using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TypeConversion
{
    /// <summary>
    /// Uses System.Convert.
    /// Converts most types to most types.
    /// Can't convert to a nullable type nor an Enum.
    /// </summary>
    public class GeneralConverter : IConverter<object,object>
    {
        public ConversionResult Convert(object source, out object destination, Type requiredType)
        {
            destination = System.Convert.ChangeType(source, requiredType);
            return ConversionResult.Success;   // succeeded if no exception is thrown
        }
    }

    /*
                Console.WriteLine(Convert.ChangeType('1', typeof(byte))); //49  ****
                Console.WriteLine(Convert.ChangeType('1', typeof(int))); //49  ****
                Console.WriteLine(Convert.ChangeType("1", typeof(int)));  //1
                Console.WriteLine(Convert.ChangeType(1, typeof(byte)));  //1
                Console.WriteLine(Convert.ChangeType(123, typeof(string)));  //"123"

                Console.WriteLine(Convert.ChangeType(Enum1.opt2, typeof(string)));  //"opt2"
                Console.WriteLine(Convert.ChangeType(Enum1.opt2, typeof(int)));  //11
    //            Console.WriteLine(Convert.ChangeType("c", typeof(Enum1)));  //InvalidCast  ****

                Console.WriteLine(Convert.ChangeType("true", typeof(bool)));  //True
                Console.WriteLine(Convert.ChangeType("trUE", typeof(bool)));  //True
                //            Console.WriteLine(Convert.ChangeType("1", typeof(bool)));  //InvalidCast ****
                Console.WriteLine(Convert.ChangeType(1, typeof(bool)));  //True
                Console.WriteLine(Convert.ChangeType(-27, typeof(bool)));  //True
                //Console.WriteLine(Convert.ChangeType('T', typeof(bool))); //InvalidCast ****
                Console.WriteLine(Convert.ChangeType(true, typeof(string))); //"True"
                Console.WriteLine(Convert.ChangeType(true, typeof(int))); //1
            //Console.WriteLine(Convert.ChangeType("asd", typeof(bool)));  //throws FormatException
            Console.WriteLine(Convert.ChangeType("false", typeof(bool)));  //False
            Console.WriteLine(Convert.ChangeType("fAlsE", typeof(bool)));  //False

     * */


    /// <summary>
    /// Converts strings (with either the name or numeric value) to enums.
    /// </summary>
    public class ToEnumConverter : IConverter<object,Enum>
    {
        public ToEnumConverter()
        {
            IgnoreCase = true;
        }

        /* Not needed:
        bool IConverter<Enum, string>.Convert(Enum source, out string destination, Type requiredType)
        {
            destination = source.ToString();
            return true;
        }
        */

        public ConversionResult Convert(object source, out Enum destination, Type requiredType)
        {
            //TODO: Could convert string to/from Enum based on a human-readable name or other value in an attribute.

            // Could optimise for numerics before converting to string.

            destination = (Enum)Enum.Parse(requiredType, source.ToString(), IgnoreCase);
            return ConversionResult.Success;   // succeeded if no exception is thrown
        }

        public virtual bool IgnoreCase { get; set; }
    }

    /// <summary>
    /// Converts characters whose value is a digit to numeric types.
    /// </summary>
    public class CharToNumberConverter : IConverter<char, object>
    {
        public ConversionResult Convert(char source, out object destination, Type requiredType)
        {
            if (source >= '0' && source <= '9')   // if a digit
            {
                destination = (byte)( (byte)source - (byte)'0' );
                System.Convert.ChangeType(destination, requiredType);  // can convert to other numeric types
                return ConversionResult.Success;
            }
            else
            {
                destination = null;
                return ConversionResult.Failed;
            }
        }
    }

    public class BoolConverter : IConverter<object,bool>
    {
        public BoolConverter()
        {
            TrueValues = new string[] { "T", "Y", "YES", "TRUE", "ON" };
            FalseValues = new string[] { "F", "N", "NO", "FALSE", "OFF" };
        }

        public ConversionResult Convert(object source, out bool destination, Type requiredType)
        {
            string value = source.ToString().Trim().ToUpper();
            if (TrueValues.Contains(value))
//                value.Equals("T") || value.Equals("Y") || value.Equals("YES") || value.Equals("TRUE") || value.Equals("ON"))
            {
                destination = true;
            }
            else if (FalseValues.Contains(value))
                //value.Equals("F") || value.Equals("N") || value.Equals("NO") || value.Equals("FALSE") || value.Equals("OFF"))
            {
                destination = false;
            }
            else
            {
                double numericValue = (double)System.Convert.ChangeType(source, typeof(double));
                destination = numericValue != 0;
            }
            return ConversionResult.Success;
        }

        public string[] TrueValues { get; set; }
        public string[] FalseValues { get; set; }
    }

}
