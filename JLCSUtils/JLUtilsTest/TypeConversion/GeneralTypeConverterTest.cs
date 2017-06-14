using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.TypeConversion
{
    [TestClass]
    public class GeneralTypeConverterTest
    {
        #region Nullable

        [TestMethod]
        public void ConvertNullToNullable()
        {
            Assert.AreEqual(null, GeneralTypeConverter.Convert<int?>(null));

            Assert.AreEqual(null, GeneralTypeConverter.Convert<int?>(""));
        }

        [TestMethod]
        public void ConvertToNullable()
        {
            Assert.AreEqual(15, GeneralTypeConverter.Convert<int?>("15"));
        }

        [TestMethod]
        public void ConvertFromNullable()
        {
            int? x = 234;
            Assert.AreEqual(234, GeneralTypeConverter.Convert<int>(x));
        }

        [TestMethod]
        public void ConvertNullableToNullable()
        {
            int? x = 234;
            Assert.AreEqual(234, GeneralTypeConverter.Convert<float?>(x));
        }

        [TestMethod]
        public void Convert_Nullable()
        {
            int? x = 456;

            Assert.AreEqual(456, GeneralTypeConverter.Convert<long>(x));

            // To nullable:
            Assert.AreEqual((short?)456, GeneralTypeConverter.Convert<short?>(x));
        }

        #endregion

        [TestMethod]
        public void Convert_StringToInt()
        {
            Assert.AreEqual("556", GeneralTypeConverter.Convert<string>(556.0));

            Assert.AreEqual(556, GeneralTypeConverter.Convert<int>("556"));
        }

        [TestMethod]
        public void Convert_Enum()
        {
            var value = ConsoleKey.BrowserStop;

            Assert.AreEqual("BrowserStop", GeneralTypeConverter.Convert<string>(value));

            // From string:
            Assert.AreEqual(ConsoleKey.BrowserStop, GeneralTypeConverter.Convert<ConsoleKey>("BrowserStop"));

            Assert.AreEqual(ConsoleKey.Clear, GeneralTypeConverter.Convert<ConsoleKey>(" Clear  "), "Has spaces");

            // Doesn't do case-insensitive conversion:            Assert.AreEqual(ConsoleKey.Clear, GeneralTypeConverter.Convert<ConsoleKey>(" CLEAR  "));

            // int:
            Assert.AreEqual((int)ConsoleKey.BrowserStop, GeneralTypeConverter.Convert<int>(ConsoleKey.BrowserStop));

            // Smaller integer type:
            Assert.AreEqual((byte)ConsoleKey.BrowserStop, GeneralTypeConverter.Convert<byte>(ConsoleKey.BrowserStop));

            // Convert to floating point:
            Assert.AreEqual((float)ConsoleKey.BrowserStop, GeneralTypeConverter.Convert<float>(ConsoleKey.BrowserStop));
        }

        [TestMethod]
        public void Convert_FloatingPoint()
        {
            Multiple(
                () => Assert.AreEqual(123.5m, GeneralTypeConverter.Convert<decimal>(123.5)),

                () => Assert.AreEqual((float)-50000, GeneralTypeConverter.Convert<float>(-50000)),

                () => Assert.AreEqual((double)47.54, GeneralTypeConverter.Convert<double>("47.54")),

                () => Assert.AreEqual((double)47.00054, GeneralTypeConverter.Convert<double>("  47.00054 "), "with spaces"),

                () => Assert.AreEqual((int)3456, GeneralTypeConverter.Convert<int>((float)3456), "floating point to integer"),

                () => Assert.AreEqual((int)3456, GeneralTypeConverter.Convert<int>((float)3456.1), "floating point to integer - rounded down"),

                () => Assert.AreEqual((int)3457, GeneralTypeConverter.Convert<int>((float)3456.9), "floating point to integer - rounded up")
            );
        }

        [TestMethod]
        public void StringToGuid()
        {
            Multiple(
                () => Assert.AreEqual(Guid.Empty, GeneralTypeConverter.Convert<Guid>(Guid.Empty.ToString())),
                () => Assert.AreEqual(new Guid("5061a89d-6d4a-4559-9ef7-f4c9ea7f95da"), GeneralTypeConverter.Convert<Guid>("5061a89d-6d4a-4559-9ef7-f4c9ea7f95da")),
                () => Assert.AreEqual(new Guid("d0701409-4769-4720-a39f-3b057d62cbba"), GeneralTypeConverter.Convert<Guid>("d070140947694720a39f3b057d62cbba"), "no hyphens"),
                () => Assert.AreEqual(new Guid("d0701409-4769-4720-a39f-3b057d62cbba"), GeneralTypeConverter.Convert<Guid>("d070140947694720a39f3b057d62cbba".ToUpper()), "no hyphens, capital letters")
            );
        }

        [TestMethod]
        public void BytesToGuid()
        {
            Multiple(
                () => Assert.AreEqual(Guid.Empty, GeneralTypeConverter.Convert<Guid>(new Guid("00000000-0000-0000-0000-000000000000").ToByteArray())),
                () => Assert.AreEqual(new Guid("5061a89d-6d4a-4559-9ef7-f4c9ea7f95da"), GeneralTypeConverter.Convert<Guid>(new Guid("5061a89d-6d4a-4559-9ef7-f4c9ea7f95da").ToByteArray()))
            );
        }

        #region Object

        [TestMethod]
        public void Convert_ObjectToString()
        {
            Assert.AreEqual(ToString(), GeneralTypeConverter.Convert<string>(this));
        }

        /// <summary>
        /// Converting an object to an integer by calling its ToString() method, then converting the result.
        /// </summary>
        [TestMethod]
        public void Convert_ObjectToInt()
        {
            Assert.AreEqual(-100, GeneralTypeConverter.Convert<int>(new TestObject()));
        }

        public class TestObject
        {
            public override string ToString() => "-100";
        }

        #endregion

        //TODO: null

    }
}