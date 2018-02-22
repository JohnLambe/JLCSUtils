using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;
using JohnLambe.Util.Diagnostic;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class ReflectionUtilTest_Hiding
    {
        #region Property

        [TestMethod]
        public void IsHiding_Property()
        {
            Multiple(
                () => Assert.IsTrue(typeof(HidingTest).GetProperty("Property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingTestBase).GetProperty("Property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingTest).GetProperty("property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingTest).GetProperty("PrivateProperty").IsHiding(), "base class property is private"),
                () => Assert.IsFalse(typeof(HidingTest).GetProperty("VirtualProperty").IsHiding(), "overridden correctly"),
                () => Assert.IsFalse(typeof(HidingTest).GetProperty("VirtualProperty").IsHiding(), "overridden correctly"),
                () => Assert.IsTrue(typeof(HidingTest).GetProperty("VirtualProperty2", BindingFlags.NonPublic | BindingFlags.Instance).IsHiding(), "hides virtual")
            );
        }

        public class HidingTestBase
        {
            public int Property1 { get; set; }
            private int PrivateProperty { get; }

            public virtual string VirtualProperty { get; }
            protected virtual object VirtualProperty2 { get; }
        }

        public class HidingTest : HidingTestBase
        {
            public new int Property1 { get; set; }
            public int property1 { get; set; }

            public int PrivateProperty { get; }

            public override string VirtualProperty { get; }
            protected new virtual object VirtualProperty2 { get; }
        }

        #endregion

        #region Field

        [TestMethod]
        public void IsHiding_Field()
        {
            Multiple(
                () => Assert.IsTrue(typeof(HidingFieldTest).GetField("Property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingFieldTestBase).GetProperty("Property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingFieldTest).GetField("property1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingFieldTest).GetField("PrivateProperty").IsHiding(), "base class property is private"),
                () => Assert.IsFalse(typeof(HidingFieldTest).GetField("PrivateField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsHiding(), "base is private"),
                () => Assert.IsTrue(typeof(HidingFieldTest).GetField("HiddenField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsHiding(), "field hiding field")
            );
        }

        public class HidingFieldTestBase
        {
            public virtual int Property1 { get; set; }
            private int PrivateProperty { get; }

#pragma warning disable CS0169   // Suppress 'Field never used'.  This is read by reflection.
            private string PrivateField;
#pragma warning restore CS0067

            protected DateTime HiddenField;
        }

        public class HidingFieldTest : HidingFieldTestBase
        {
            public new int Property1;   // field that hides property
            public int property1;

            public int PrivateProperty;

            public string PrivateField;

            protected new DateTime HiddenField;
        }

        #endregion

        #region Field

        [TestMethod]
        public void IsHiding_Method()
        {
            Multiple(
                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method2").IsHiding()),
                () => Assert.IsFalse(typeof(HidingMethodTest).GetField("Method1").IsHiding()),
                () => Assert.IsFalse(typeof(HidingMethodTest).GetProperty("Method2").IsHiding()),
                () => Assert.IsFalse(typeof(HidingMethodTestBase).GetMethod("Method1").IsHiding()),

                () => Assert.IsTrue(typeof(HidingMethodTest).GetMethod("Method3", new Type[] { }).IsHiding()),
                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method3", new Type[] { typeof(byte) }).IsHiding()),

                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method4").IsHiding()),
                () => Assert.IsTrue(typeof(HidingMethodTest).GetMethod("Method5", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).IsHiding())
            );
            //TODO: different visibilities
        }

        [TestMethod]
        public void IsHidingNonStrict_Method()
        {
            Multiple(
                // On base class:
                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method1").IsHidingNonStrict()),
                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method2").IsHidingNonStrict()),

                // Field / Property named same as method:
                () => Assert.IsTrue(typeof(HidingMethodTest).GetField("Method1").IsHidingNonStrict()),
                () => Assert.IsTrue(typeof(HidingMethodTest).GetProperty("Method2").IsHidingNonStrict()),

                // Common non-hiding case:
                () => Assert.IsFalse(typeof(HidingMethodTest).GetField("property1").IsHidingNonStrict()),

                () => Assert.IsFalse(typeof(HidingMethodTest).GetMethod("Method4").IsHidingNonStrict()),
                () => Assert.IsTrue(typeof(HidingMethodTest).GetMethod("Method5", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).IsHidingNonStrict())
            );
        }

        public class HidingMethodTestBase
        {
            public int Method1() => 1;
            public int Method2() => 1;
            public int Method3() => 1;
            public virtual int Method4() => 1;
            public virtual int Method5() => 1;
        }

        public class HidingMethodTest : HidingMethodTestBase
        {
            public new int Method1;

            public new int Method2 { get; }

            public new string Method3() => "";

            public string Method3(byte a) => "";  // overload

            public int property1;

            public override int Method4() => 1;
            private new int Method5() => 1;
        }

        #endregion

        // There are many permutations not tested.

    }

}
