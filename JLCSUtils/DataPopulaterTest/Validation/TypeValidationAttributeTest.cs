using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;
using System.ComponentModel.DataAnnotations;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class TypeValidationAttributeTest
    {
        [TestMethod]
        public void IsValid_AbstractClass()
        {
            var attrib = new TypeValidationAttribute()
            {
                IsAbstract = true,
                IsClass = true
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(GetType())),
                () => Assert.IsFalse(attrib.IsValid(typeof(int))),
                () => Assert.IsTrue(attrib.IsValid(typeof(System.Attribute)))
            );
        }

        [TestMethod]
        public void IsValid_Implements()
        {
            var attrib = new TypeValidationAttribute()
            {
                Implements = typeof(IComparable),
                IsPrimitive = false,
                IsNumericType = false,
                IsValueType = false,
                IsEnum = false
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(GetType())),
                () => Assert.IsFalse(attrib.IsValid(typeof(int))),  // IComparable but not Primitive
                () => Assert.IsTrue(attrib.IsValid(typeof(string)))
            );
        }

        [TestMethod]
        public void IsValid_IsAssignableTo()
        {
            var attrib = new TypeValidationAttribute()
            {
                AssignableTo = GetType()
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid(GetType())),
                () => Assert.IsTrue(attrib.IsValid(typeof(TestSubClass))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.Attribute))),
                () => Assert.IsFalse(attrib.IsValid(typeof(int))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.StringSplitOptions)))
            );
        }

        [TestMethod]
        public void IsValid_Namespace()
        {
            var attrib = new TypeValidationAttribute()
            {
                Namespace = typeof(System.Text.ASCIIEncoding).Namespace,
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(GetType())),
                () => Assert.IsFalse(attrib.IsValid(typeof(TestSubClass))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.Attribute))),
                () => Assert.IsFalse(attrib.IsValid(typeof(long))),
                () => Assert.IsTrue(attrib.IsValid(typeof(System.Text.DecoderFallback)))
            );
        }

        [TestMethod]
        public void IsValid_IsEnum()
        {
            var attrib = new TypeValidationAttribute()
            {
                IsEnum = false   // not an enum
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid(GetType())),
                () => Assert.IsTrue(attrib.IsValid(typeof(TestSubClass))),
                () => Assert.IsTrue(attrib.IsValid(typeof(System.Attribute))),
                () => Assert.IsTrue(attrib.IsValid(typeof(long))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.StringSplitOptions)))
            );
        }

        [TestMethod]
        public void IsValid_TypeOrSubclassOf()
        {
            var attrib = new TypeValidationAttribute()
            {
                TypeOrSubclassOf = GetType()
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid(GetType())),
                () => Assert.IsTrue(attrib.IsValid(typeof(TestSubClass))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.Attribute))),
                () => Assert.IsFalse(attrib.IsValid(typeof(long))),
                () => Assert.IsFalse(attrib.IsValid(typeof(System.Text.DecoderFallback)))
            );
        }

    }

    public class TestSubClass : TypeValidationAttributeTest
    {
    }
}
