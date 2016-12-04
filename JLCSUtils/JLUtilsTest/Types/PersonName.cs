using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Types;

namespace JohnLambe.Tests.JLUtilsTest.Types
{
    [TestClass]
    public class PersonNameTest
    {
        [TestMethod]
        public void SimpleName_Parse_2Names()
        {
            _name.SimpleName = "First Surname";

            Assert.AreEqual(_name.FirstNames,"First");
            Assert.AreEqual(_name.Surname,"Surname");
        }

        [TestMethod]
        public void SimpleName_Parse()
        {
            _name.SimpleName = "First Middle Surname";

            Assert.AreEqual(_name.FirstNames, "First Middle");
            Assert.AreEqual(_name.Surname, "Surname");
        }

        [TestMethod]
        public void SimpleName_Get()
        {
            _name.FirstNames = "Joe A.";
            _name.Surname = "Bloggs";

            Assert.AreEqual(_name.SimpleName, "Joe A. Bloggs");
        }

        [TestMethod]
        public void FirstNames()
        {
            _name.FirstNames = "Joe Andrew Michael";
            _name.Surname = "Bloggs";

            Assert.AreEqual(_name.FirstNames, "Joe Andrew Michael");
            Assert.AreEqual(_name.FirstName, "Joe");
            Assert.AreEqual(_name.MiddleNames, "Andrew Michael");
            Assert.AreEqual(_name.MiddleInitial, 'A');
        }

        [TestMethod]
        public void FirstName_Set()
        {
            _name.FirstNames = "Joe Andrew Michael";
            _name.Surname = "Bloggs";

            _name.FirstName = "Joseph";

            Assert.AreEqual(_name.FirstNames, "Joseph Andrew Michael");
            Assert.AreEqual(_name.FirstName, "Joseph");
            Assert.AreEqual(_name.MiddleNames, "Andrew Michael");
            Assert.AreEqual(_name.MiddleInitial, 'A');
        }

        protected PersonName _name = new PersonName();
    }
}
