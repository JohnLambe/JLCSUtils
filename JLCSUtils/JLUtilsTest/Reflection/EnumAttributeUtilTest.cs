using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class EnumAttributeUtilTest
    {
        public class CustomEnumMappedValueAttribute : EnumMappedValueAttribute
        {
            public CustomEnumMappedValueAttribute(object value) : base(value)
            {
            }
        }

        public enum TestEnum
        {
            [CustomEnumMappedValueAttribute(10)]
            A,
            [CustomEnumMappedValueAttribute(20)]
            B,
            C
        }

        [TestMethod]
        public void FromEnum()
        {
            Assert.AreEqual(10, EnumAttributeUtil.FromEnum<int, CustomEnumMappedValueAttribute>(TestEnum.A));
            Assert.AreEqual(20, EnumAttributeUtil.FromEnum<int, CustomEnumMappedValueAttribute>(TestEnum.B));
            Assert.AreEqual(0, EnumAttributeUtil.FromEnum<int, CustomEnumMappedValueAttribute>(TestEnum.C));
        }

        [TestMethod]
        public void FromEnum_ToNullable()
        {
            Assert.AreEqual(10, EnumAttributeUtil.FromEnum<int?, CustomEnumMappedValueAttribute>(TestEnum.A));
            Assert.AreEqual(20, EnumAttributeUtil.FromEnum<int?, CustomEnumMappedValueAttribute>(TestEnum.B));
            Assert.AreEqual(null, EnumAttributeUtil.FromEnum<int?, CustomEnumMappedValueAttribute>(TestEnum.C));
        }

        [TestMethod]
        public void FromEnum_ConvertType()
        {
            Assert.AreEqual("10", EnumAttributeUtil.FromEnum<string, CustomEnumMappedValueAttribute>(TestEnum.A));
            Assert.AreEqual(20.0, EnumAttributeUtil.FromEnum<double, CustomEnumMappedValueAttribute>(TestEnum.B));
            Assert.AreEqual(0, EnumAttributeUtil.FromEnum<byte, CustomEnumMappedValueAttribute>(TestEnum.C));
        }

    }
}
