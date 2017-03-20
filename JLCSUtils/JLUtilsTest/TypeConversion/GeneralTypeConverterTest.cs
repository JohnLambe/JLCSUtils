using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;

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

        #endregion

        [TestMethod]
        public void Convert()
        {
            Assert.AreEqual("556", GeneralTypeConverter.Convert<string>(556.0));

            Assert.AreEqual(556, GeneralTypeConverter.Convert<int>("556"));
        }

    }
}
