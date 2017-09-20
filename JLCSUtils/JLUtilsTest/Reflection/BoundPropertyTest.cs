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
    public class BoundPropertyTest
    {
        public BoundPropertyTest()
        {
            _bproperty = new BoundProperty<BoundPropertyTest, int>(this, "TestProperty");
        }

        [TestMethod]
        public void ValueConverted_Int()
        {
            // Act:
            _bproperty.ValueConverted = 123.85;

            // Assert:
            Assert.AreEqual(124, TestProperty);
        }

        [TestMethod]
        public void ValueConverted_String()
        {
            // Act:
            _bproperty.ValueConverted = "567";

            // Assert:
            Assert.AreEqual(567, TestProperty);
            Assert.AreEqual(567, _bproperty.Value);
            Assert.AreEqual(567, _bproperty.ValueConverted);
        }

        [TestMethod]
        public void Value()
        {
            // Act:
            _bproperty.Value = -100;

            // Assert:
            Assert.AreEqual(-100, TestProperty);
        }

        [TestMethod]
        public void BoundPropertyProperties()
        {
            // Act:
            _bproperty.Value = -100;

            // Assert:
            Assert.AreEqual(nameof(TestProperty), _bproperty.Name);
            Assert.AreEqual(true, _bproperty.CanRead);
            Assert.AreEqual(true, _bproperty.CanWrite);
            Assert.AreEqual("TestProperty description", _bproperty.Description);
            Assert.AreEqual(this, _bproperty.Target);            
        }

        [TestMethod]
        public void Null()
        {
            // Act:
            var bproperty = new BoundProperty<object,string>(null,"property");

            // Assert:
            Assert.AreEqual(null, bproperty.Name);
            Assert.AreEqual(false, bproperty.CanRead);
            Assert.AreEqual(false, bproperty.CanWrite);
            Assert.AreEqual(null, bproperty.Description);
            Assert.AreEqual(null, bproperty.Target);
        }


        [System.ComponentModel.Description("TestProperty description")]
        public int TestProperty { get; set; }

        BoundProperty<BoundPropertyTest, int> _bproperty;
    }
}
