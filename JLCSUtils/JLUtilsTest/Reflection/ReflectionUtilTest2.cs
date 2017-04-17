﻿using JohnLambe.Util.Collections;
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
    public class ReflectionUtilTest2
    {
        #region IsOverride

        [TestMethod]
        public void IsOverride_Method()
        {
            Assert.IsTrue(ReflectionUtil.IsOverride(typeof(Level3).GetMethod("TestMethod")), "Override");

            Assert.IsFalse(ReflectionUtil.IsOverride(typeof(Level1).GetMethod("TestMethod")), "Base virtual declaration");

            Assert.IsFalse(ReflectionUtil.IsOverride(typeof(Level3).GetMethod("NonVirtualMethod")), "Non-virtual");
        }

        [TestMethod]
        public void IsOverride_Property()
        {
            Assert.IsTrue(ReflectionUtil.IsOverride(typeof(Level3).GetProperty("Property")), "Override");

            Assert.IsFalse(ReflectionUtil.IsOverride(typeof(Level1).GetProperty("Property")), "Base virtual declaration");

            Assert.IsFalse(ReflectionUtil.IsOverride(typeof(Level3).GetProperty("NonVirtualProperty")), "Non-virtual");
            Assert.IsFalse(ReflectionUtil.IsOverride(typeof(Level4).GetProperty("NonVirtualProperty")), "Non-virtual, using subclass of the declaring one");  // GetProperty returns an instance with non-public members different to the one in the previouse case.

            Assert.IsTrue(ReflectionUtil.IsOverride(typeof(Level5).GetProperty("WriteOnlyProperty")), "write-only");
        }

        #endregion

        [TestMethod]
        public void GetBaseDefinition()
        {
            Assert.AreEqual(typeof(Level1).GetProperty("Property"), ReflectionUtil.GetBaseDefinition(typeof(Level1).GetProperty("Property")), "base property");

            Assert.AreEqual(typeof(Level1).GetProperty("Property"), ReflectionUtil.GetBaseDefinition(typeof(Level3).GetProperty("Property")), "Override");
            Assert.AreEqual(typeof(Level1).GetProperty("Property"), ReflectionUtil.GetBaseDefinition(typeof(Level4).GetProperty("Property")), "Override");

            Assert.AreEqual(typeof(Level1).GetProperty("Property"), ReflectionUtil.GetBaseDefinition(typeof(Level5).GetProperty("Property")), "Override - two levels down");
        }

        [TestMethod]
        public void GetOverriddenMethod()
        {
            Assert.AreEqual(null, ReflectionUtil.GetOverriddenMethod(typeof(Level1).GetMethod("TestMethod")), "base property");

            Assert.AreEqual(typeof(Level1).GetMethod("TestMethod").GetDescription(), ReflectionUtil.GetOverriddenMethod(typeof(Level3).GetMethod("TestMethod")).GetDescription(), "Override");

            Assert.AreEqual(typeof(Level3).GetMethod("TestMethod").GetDescription(), ReflectionUtil.GetOverriddenMethod(typeof(Level5).GetMethod("TestMethod")).GetDescription(), "Override - two levels down");
        }

        [TestMethod]
        public void GetOverriddenProperty()
        {
            Assert.AreEqual(null, ReflectionUtil.GetOverriddenProperty(typeof(Level1).GetProperty("Property")), "base property");

            Assert.AreEqual(typeof(Level1).GetProperty("Property").GetDescription(), ReflectionUtil.GetOverriddenProperty(typeof(Level3).GetProperty("Property")).GetDescription(), "Override");
            Assert.AreEqual(typeof(Level1).GetProperty("Property").GetDescription(), ReflectionUtil.GetOverriddenProperty(typeof(Level4).GetProperty("Property")).GetDescription(), "Override, using subclass of declaring class");

            Assert.AreEqual(typeof(Level3).GetProperty("Property").GetDescription(), ReflectionUtil.GetOverriddenProperty(typeof(Level5).GetProperty("Property")).GetDescription(), "Override - two levels down");
        }

        #region GetDescription

        [TestMethod]
        public void GetDescription_Null()
        {
            Assert.AreEqual(null, ReflectionUtil.GetDescription(null));
        }

        [TestMethod]
        public void GetDescription_Property()
        {
            Assert.AreEqual("System.Int32 " + typeof(Level3).FullName + ".Property", typeof(Level4).GetProperty("Property").GetDescription());
        }

        /*
        [TestMethod]
        public void GetDescription_Method()
        {
            Assert.AreEqual("System.Int32 " + typeof(Level3).FullName + ".TestMethod(System.Object)", typeof(Level4).GetMethod("TestMethod").GetDescription());
        }
        */

        #endregion
    }


    #region Classes for Test

    public class Level1
    {
        public virtual int TestMethod(object x)
        {
            return 0;
        }

        public int NonVirtualMethod(object x)
        {
            return 0;
        }

        public virtual int Property { get; set; }

        public virtual string WriteOnlyProperty { set { } }

        public int NonVirtualProperty { get; set; }
    }

    public class Level2 : Level1
    {
    }

    public class Level3 : Level2
    {
        public override int TestMethod(object x)
        {
            return 0;
        }

        public new int NonVirtualMethod(object x)
        {
            return 0;
        }

        public override int Property { get; set; }

        public override string WriteOnlyProperty { set { } }

        public new int NonVirtualProperty { get; set; }
    }

    public class Level4 : Level3
    {
    }

    public class Level5 : Level4
    {
        public override int TestMethod(object x)
        {
            return 0;
        }

        public override int Property { get; set; }
    }

    #endregion
}
