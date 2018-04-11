using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ObjectExtensionTest
    {
        #region NotNull

        const string TestMessage = "test message";

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void NotNull_NullSimple()
        {
            string x = null;

            x.NotNull();
        }

        [TestMethod]
        public void NotNull_NotNull()
        {
            string x = "asd";

            Console.WriteLine(x.NotNull().ToUpper());
            x.NotNull();
        }

        [TestMethod]
        public void NotNull_Null()
        {
            string x = null;

            TestUtil.Multiple(
                () => TestUtil.AssertThrowsContains(typeof(NullReferenceException), TestMessage,
                    () => x.NotNull(TestMessage) ),

                () => TestUtil.AssertThrowsContains(typeof(NullReferenceException), TestMessage,
                    () => Console.WriteLine(x.NotNull(TestMessage)) ),

                () => TestUtil.AssertThrowsContains(typeof(NullReferenceException), TestMessage,
                    () => x.NotNull(TestMessage).ToUpper() )
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNotNull_NullSimple()
        {
            string x = null;

            x.ArgNotNull();
        }

        [TestMethod]
        public void ArgNotNull_NotNull()
        {
            string x = "asd";

            Console.WriteLine(x.ArgNotNull().ToUpper());
            x.ArgNotNull();
        }

        [TestMethod]
        public void ArgNotNull_Null()
        {
            string x = null;

            TestUtil.Multiple(
                () => TestUtil.AssertThrowsContains(typeof(ArgumentNullException), TestMessage,
                    () => x.ArgNotNull(TestMessage)),

                () => TestUtil.AssertThrowsContains(typeof(ArgumentNullException), TestMessage,
                    () => Console.WriteLine(x.ArgNotNull(TestMessage))),

                () => TestUtil.AssertThrowsContains(typeof(ArgumentNullException), TestMessage,
                    () => x.ArgNotNull(TestMessage).ToUpper())
            );
        }

        #endregion

    }
}
