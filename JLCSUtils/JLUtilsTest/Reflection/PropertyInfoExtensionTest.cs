using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;
using System.Reflection;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class PropertyInfoExtensionTest
    {
        #region SetValueConverted

        [TestMethod]
        public void SetValueConverted_IntToString()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("StrProperty");
            var obj = new TestTargetClass();

            // Act:
            pi.SetValueConverted(obj, 123);

            // Assert:
            Assert.AreEqual("123", obj.StrProperty);
        }

        [TestMethod]
        public void SetValueConverted_StringToLong()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("LongProperty");
            var obj = new TestTargetClass();

            // Act:
            pi.SetValueConverted(obj, "10000000000");

            // Assert:
            Assert.AreEqual(10000000000, obj.LongProperty);
        }

        [TestMethod]
        public void SetValueConverted_BoolToString()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("StrProperty");
            var obj = new TestTargetClass();

            // Act:
            pi.SetValueConverted(obj, true);

            // Assert:
            Assert.AreEqual("True", obj.StrProperty);
        }

        [TestMethod]
        public void SetValueConverted_ObjectToString()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("StrProperty");
            var obj = new TestTargetClass();

            // Act:
            pi.SetValueConverted(obj, obj);
            Console.WriteLine(obj.ToString());

            // Assert:
            Assert.AreEqual(obj.ToString(), obj.StrProperty);
        }

        #endregion

        #region GetValueConverted

        [TestMethod]
        public void GetValueConverted_StringToInt()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("StrProperty");
            var obj = new TestTargetClass()
            {
                StrProperty = "5678"
            };

            // Act / Assert:
            Assert.AreEqual(5678,pi.GetValueConverted<int>(obj));
        }

        [TestMethod]
        public void GetValueConverted_LongToString()
        {
            // Arrange:
            PropertyInfo pi = typeof(TestTargetClass).GetProperty("LongProperty");
            var obj = new TestTargetClass()
            {
                LongProperty = 5000000000000
            };

            // Act / Assert:
            Assert.AreEqual("5000000000000", pi.GetValueConverted<string>(obj));
        }

        #endregion

        public class TestTargetClass
        {
            public string StrProperty { get; set; }
            public long LongProperty { get; set; }
        }
    }
}
