using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class ClassMultitonTest
    {
        [TestMethod]
        public void ScanAndGet()
        {
            _multiton.Scan<TestClassMultitonAtribute>(Assembly.GetExecutingAssembly());

            Assert.AreEqual("Instance1", _multiton.Get(10).Value);
            Assert.AreEqual("Instance3", _multiton.Get(30).Value);

            int x = _multiton.Get(20);
            Assert.AreEqual(20, x);
        }

        [TestMethod]
        public void Cast()
        {
            TestClassMultiton.Init();

            Assert.AreEqual("Instance1", TestClassMultiton.Instance.Get(10).Value);
            Assert.AreEqual("Instance3", ((TestClassMultiton)30).Value);

            TestClassMultiton y = 20;
            Assert.AreEqual("Instance2", y.Value);

            int x = y;
            Assert.AreEqual(20, x);
        }

        protected ClassMultiton<int, TestClassMultiton> _multiton = new IdClassMultiton<int, TestClassMultiton>();
    }

    public class TestClassMultitonAtribute : Attribute
    {
    }

    public abstract class TestClassMultiton : ClassMultitonMember<int, TestClassMultiton>
    {
        public virtual string Value => GetType().Name;

        public static void Init()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TestClassMultiton(int value)
            => Instance.Get(value);

        static TestClassMultiton()
        {
            Instance.Scan<TestClassMultitonAtribute>(Assembly.GetExecutingAssembly());
        }
    }

    [TestClassMultitonAtribute]
    public class Instance1 : TestClassMultiton
    {
        public override int Id => 10;
    }

    [TestClassMultitonAtribute]
    public class Instance2 : TestClassMultiton
    {
        public override int Id => 20;
    }

    [TestClassMultitonAtribute]
    public class Instance3 : TestClassMultiton
    {
        public override int Id => 30;
    }
}
