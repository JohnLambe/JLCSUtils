using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Validation;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class DataSizeAttributeTest
    {
        [TestMethod]
        public void ParseSize()
        {
            TestUtil.Multiple(
                // binary-based:
                () => Assert.AreEqual(12345, DataSizeValidationAttribute.ParseSize("12345 B")),
                () => Assert.AreEqual(872 * 1024, DataSizeValidationAttribute.ParseSize("872 KiB")),
                () => Assert.AreEqual(1024 * 1024, DataSizeValidationAttribute.ParseSize("1M")),
                () => Assert.AreEqual((long)(4.7 * Math.Pow(1024, 3)), DataSizeValidationAttribute.ParseSize("4.7GiB")),
                () => Assert.AreEqual((long)(8 * Math.Pow(1024, 4)), DataSizeValidationAttribute.ParseSize("08TiB ")),
                () => Assert.AreEqual((long)(50 * Math.Pow(1024, 5)), DataSizeValidationAttribute.ParseSize("50 P")),

                () => Assert.AreEqual(8500, DataSizeValidationAttribute.ParseSize("8500")),
                () => Assert.AreEqual(50 * 1024, DataSizeValidationAttribute.ParseSize("50", true, 'K')),

                // decimal-based:
                () => Assert.AreEqual(50 * 1000000, DataSizeValidationAttribute.ParseSize("50", false, 'M')),

                () => Assert.AreEqual(32000000000, DataSizeValidationAttribute.ParseSize("32GB")),
                () => Assert.AreEqual(2L * (long)Math.Pow(1000, 6), DataSizeValidationAttribute.ParseSize("2.00 EB")),
                () => Assert.AreEqual(6780000, DataSizeValidationAttribute.ParseSize("6.78 M",false)),

                // wrong capitalisation:
                () => Assert.AreEqual(2450000, DataSizeValidationAttribute.ParseSize("2.45mb", false)),
                () => Assert.AreEqual(7450000000, DataSizeValidationAttribute.ParseSize("7.45 gB", false)),
                () => Assert.AreEqual(3.5 * 1024, DataSizeValidationAttribute.ParseSize("3.5 kIB", false)),

                // extra SPACEs:
                () => Assert.AreEqual(1.5 * 1024 * 1024 * 1024, DataSizeValidationAttribute.ParseSize("  1.5   G  "))
                );
        }
    }
}
