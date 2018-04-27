using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;
using JohnLambe.Util.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace JLUtilsEFTest.Db.Ef
{
    [TestClass]
    public class ClonerTest
    {
        public TestBase Test(TestBase src)
        {
            // Act:
            var dest = Cloner.Clone(src);
            Console.Out.WriteLine(src.GetState() + "\n" + dest.GetState());

            // Assert:
            Assert.AreEqual(src.GetState(), dest.GetState());
            Assert.IsTrue(src != dest);   // not the same instance

            return dest;
        }


        [TestMethod]
        public void BaseClassPrivateField()
        {
            Test(new Sub());
        }

        public abstract class TestBase
        {
            public virtual string GetState() => "";
            public override string ToString() => "{" + GetState() + "}";
        }

        public class Base : TestBase
        {
            private Guid guid = Guid.NewGuid();
            private Guid guid2 = Guid.NewGuid();

            protected string GetBaseState() => guid.ToString() + "; " + guid2.ToString();
        }

        public class Sub : Base
        {
            private Guid guid = Guid.NewGuid();   // same name as base class field

            public override string GetState() => GetBaseState() + "; " + guid.ToString();
        }


        [TestMethod]
        public void ProtectedProperty()
        {
            Test(new B1());
        }

        public class B1 : TestBase
        {
            protected Guid Guid1 { get; set; } = Guid.NewGuid();

            public override string GetState() => Guid1.ToString();
        }


        [TestMethod]
        public void ReadOnlyProperty()
        {
            Test(new C1());
        }

        public class C1 : TestBase
        {
            private readonly string X = Guid.NewGuid().ToString();
            protected Guid Y { get; } = Guid.NewGuid();

            public override string GetState() => X + "; " + Y.ToString();
        }


        /// <summary>
        /// Multiple types and visibilities.
        /// </summary>
        [TestMethod]
        public void Types()
        {
            Test(new D2());
        }

        public class D1 : TestBase
        {
            public static int _counter = 1;

            public int A = _counter++;
            protected decimal B = _counter++ + 0.1m;
        }

        public class D2 : D1
        {
            public D2()
            {
                D++;
            }

            protected byte C = (byte)_counter++;
            public double D = _counter++ + 0.25678;
            private object E = new Sub();    // reference

            public virtual StringBuilder Reference { get; set; } = new StringBuilder("initial ");    // reference

            public override string GetState() => A.ToString() + "; " + B.ToString() + "; " + C.ToString() + "; " + D.ToString() + "; " + E.ToString() + "; " + Reference.ToString();
        }

        public class D3 : D2
        {
            public Complex1 C { get; set; } = new Complex1();
        }

        [ComplexType]
        public class Complex1
        {
            public string Value { get; set; } = "A";
        }


        /// <summary>
        /// Test that only a shallow copy is made.
        /// </summary>
        [TestMethod]
        public void NotDeep()
        {
            var src = new D2();
            var copy = Test(src);

            src.Reference.Append("modified");  // modify the referenced object

            Assert.AreEqual(((D2)src).Reference, ((D2)copy).Reference);
            Assert.AreEqual("initial modified", ((D2)copy).Reference.ToString());
        }

        [TestMethod]
        public void ComplexType()
        {
            D3 src = new D3();
            D3 copy = (D3)Test(src);

            Assert.AreEqual(src.C.Value, copy.C.Value);

            src.C.Value = "modified";   // modify the original after copying

            Assert.AreEqual(src.C.Value, copy.C.Value, "shallow copy");   // test that this is a deep copy
        }

    }
}
