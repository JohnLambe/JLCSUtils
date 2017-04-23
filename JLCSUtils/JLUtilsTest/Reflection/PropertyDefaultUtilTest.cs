using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class PropertyDefaultUtilTest
    {
        [TestMethod]
        public void PopulateDefaults_SimpleCase()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation();

            // Act:
            PropertyDefaultUtil.PopulateDefaults(target);

            // Assert:

            Assert.AreEqual("StringValue", target.StringProperty2, "Common case");

            // Not to be set:
            Assert.AreEqual("initial value", target.NoDefault, "No default attribute");
            Assert.AreEqual(0.0m, TestClassForDefaultPopulation.StaticProperty, "Static property should not be set");
            Assert.AreEqual("C", target.ReadOnlyProperty, "Read-only property cannot be set");  // it can't set this, but shouldn't throw an exception
        }

        /// <summary>
        /// More complex cases.
        /// </summary>
        [TestMethod]
        public void PopulateDefaults()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation();

            // Act:
            PropertyDefaultUtil.PopulateDefaults(target);

            // Assert:

            Assert.AreEqual(123.456, target.DoubleProperty, "From base class, not overridden");
            Assert.AreEqual(150, target.ByteProperty, "Default overridden");
            Assert.AreEqual(1234567890123456789, target.LongProperty, "Default defined only on overridden property");
            Assert.AreEqual('a', target.CharProperty, "Property overridden but default is unchanged");

            Assert.AreEqual(typeof(string), target.TypeProperty, "Property is assigned a default value that is overridden by the attribute");
            Assert.AreEqual(true, target.BoolProperty, "nullable primitive type");

            Assert.AreEqual("1001", target.StringProperty, "Default value is different type to property");
            Assert.AreEqual(null, target.StringProperty3, "Default value is null");
        }

        /// <summary>
        /// Setting private and protected properties.
        /// </summary>
        [TestMethod]
        public void PopulateDefaults_NonPublic()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation_NonPublic();

            // Act:
            PropertyDefaultUtil.PopulateDefaults(target);

            // Assert:

            Assert.AreEqual(5000, target.GetPrivatePropertyValue(), "private property");
            Assert.AreEqual("Protected", target.ProtectedProperty, "protected setter");
            Assert.AreEqual("Private", target.PrivateSetterProperty, "private setter");
        }

        /// <summary>
        /// The default value of a property with a private setter is overridden on a subclass that overrides the property
        /// (with an overridden getter).
        /// </summary>
        [TestMethod]
        public void PopulateDefaults_OverriddenPrivate()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation_NonPublic_Subclass();

            // Act:
            PropertyDefaultUtil.PopulateDefaults(target);

            // Assert:

            Assert.AreEqual("Overridden", target.PrivateSetterProperty, "private setter");
        }

        [TestMethod]
        public void PopulateDefaults_Static()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation();

            // Act:
            PropertyDefaultUtil.PopulateDefaults(target, BindingFlagsExt.Static | BindingFlagsExt.Public);

            // Assert:

            Assert.AreEqual(10.5m, TestClassForDefaultPopulation.StaticProperty, "Static property should not be set");

            // Not to be set:
            Assert.AreEqual(null, target.StringProperty2, "Common case");
            Assert.AreEqual("initial value", target.NoDefault, "No default attribute");
        }

        [TestMethod]
        public void PopulateStaticDefaults()
        {
            // Act:
            PropertyDefaultUtil.PopulateStaticDefaults(typeof(TestClassForDefaultPopulation));

            // Assert:
            Assert.AreEqual(10.5m, TestClassForDefaultPopulation.StaticProperty, "Static property should not be set");
        }

        [TestMethod]
        public void PopulateStaticDefaults_InheritedPrivate()
        {
            // Act:
            PropertyDefaultUtil.PopulateStaticDefaults(typeof(TestClassForDefaultPopulation_NonPublic), BindingFlagsExt.Static | BindingFlagsExt.Public);

            // Assert:
            Assert.AreEqual(102, TestClassForDefaultPopulation.StaticPrivateProperty, "Static private");
        }

        [TestMethod]
        public void SetPropertyToDefault_NotInheritedPrivate()
        {
            // Arrange:
            var target = new TestClassForDefaultPopulation();

            // Act:
            Assert.IsFalse(PropertyDefaultUtil.SetPropertyToDefault(target.GetType().GetProperty("PrivateSetterProperty"), target, true, false));
            Assert.IsTrue(PropertyDefaultUtil.SetPropertyToDefault(target.GetType().GetProperty("LongProperty"), target, true, false));

            // Assert:
            Assert.AreEqual(null, target.PrivateSetterProperty, "Shouldn't be set");
            Assert.AreEqual(1234567890123456789, target.LongProperty, "Default defined only on overridden property");
        }


        public class TestClassForDefaultPopulationBase
        {
            [DefaultValue(123.456)]
            public double DoubleProperty { get; set; }

            [DefaultValue('a')]
            public virtual char CharProperty { get; set; }

            [DefaultValue(50)]
            public virtual byte ByteProperty { get; set; }

            public virtual long LongProperty { get; set; }

            [DefaultValue("Protected")]
            public string ProtectedProperty { get; protected set; } = "X";

            [DefaultValue("Private")]
            public virtual string PrivateSetterProperty { get; private set; }

            [DefaultValue(102)]
            public static int StaticPrivateProperty { get; set; } = 101;
        }

        public class TestClassForDefaultPopulation : TestClassForDefaultPopulationBase
        {
            public string NoDefault { get; set; } = "initial value";

            [DefaultValue(1001)]        // assigning integer to String property
            public string StringProperty { get; set; }

            [DefaultValue("StringValue")]
            public virtual string StringProperty2 { get; set; }

            [DefaultValue(typeof(string))]
            public virtual Type TypeProperty { get; set; } = typeof(int);

            [DefaultValue(true)]
            public virtual bool? BoolProperty { get; set; }

            [DefaultValue(null)]  // to overwrite initial value with null
            public virtual string StringProperty3 { get; set; } = "Z";

            [DefaultValue("B")]   // this can't be assigned
            public virtual string ReadOnlyProperty { get; } = "C";

            // Overrides:

            public override char CharProperty { get; set; }

            [DefaultValue(150)]
            public override byte ByteProperty { get; set; }

            [DefaultValue(1234567890123456789)]
            public override long LongProperty { get; set; }

            [DefaultValue(10.5)]
            public static decimal StaticProperty { get; set; }
        }

        public class TestClassForDefaultPopulation_NonPublic : TestClassForDefaultPopulationBase
        {
            [DefaultValue(5000)]
            private int PrivateProperty { get; set; }

            public int GetPrivatePropertyValue() => PrivateProperty;
        }

        public class TestClassForDefaultPopulation_NonPublic_Subclass : TestClassForDefaultPopulation_NonPublic
        {
            [DefaultValue("Overridden")]
            public override string PrivateSetterProperty
            {
                get
                {
                    return base.PrivateSetterProperty;
                }
            }
        }
    }
}