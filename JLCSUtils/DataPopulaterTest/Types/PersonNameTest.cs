using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Types;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Types
{
    [TestClass]
    public class PersonNameTest
    {
        [TestMethod]
        public void SimpleName_Parse_2Names()
        {
            _name.SimpleName = "First Surname";

            Assert.AreEqual("First", _name.FirstNames);
            Assert.AreEqual("Surname", _name.Surname);
        }

        [TestMethod]
        public void SimpleName_Parse()
        {
            _name.SimpleName = "First Middle Surname";

            Assert.AreEqual("First Middle", _name.FirstNames);
            Assert.AreEqual("Surname", _name.Surname);
        }

        [TestMethod]
        public void SimpleName_Get()
        {
            _name.FirstNames = "Joe A.";
            _name.Surname = "Bloggs";

            Assert.AreEqual(_name.SimpleName, "Joe A. Bloggs");
        }

        [TestMethod]
        public void SimpleName_Set()
        {
            _name.SimpleName = "Jane ä Doe";

            Assert.AreEqual("Jane ä Doe", _name.SimpleName);
            Assert.AreEqual("Jane ä", _name.FirstNames);
            Assert.AreEqual("Doe", _name.Surname);
        }

        [TestMethod]
        public void SimpleName_Set_SurnameFirst()
        {
            _name.SimpleName = "Doe, Jane ä";

            Assert.AreEqual("Jane ä Doe", _name.SimpleName);
            Assert.AreEqual("Jane ä", _name.FirstNames);
            Assert.AreEqual("Doe", _name.Surname);
        }

        [TestMethod]
        public void FirstNames()
        {
            _name.FirstNames = "Joe Andrew Michael";
            _name.Surname = "Bloggs";

            Assert.AreEqual("Joe Andrew Michael", _name.FirstNames);
            Assert.AreEqual("Joe", _name.FirstName);
            Assert.AreEqual("Andrew Michael", _name.MiddleNames);
            Assert.AreEqual('A', _name.MiddleInitial);
        }

        [TestMethod]
        public void FirstName_Set()
        {
            _name.FirstNames = "Joe Andrew Michael";
            _name.Surname = "Bloggs";

            _name.FirstName = "Séan";   // includes a non-ASCII character

            Assert.AreEqual("Séan Andrew Michael", _name.FirstNames);
            Assert.AreEqual("Séan", _name.FirstName);
            Assert.AreEqual("Andrew Michael", _name.MiddleNames);
            Assert.AreEqual('A', _name.MiddleInitial);
        }

        [TestMethod]
        public void FullName()
        {
            _name.Surname = "Bloggs";
            Assert.AreEqual("Bloggs", _name.FullName);

            _name.FirstNames = "Joseph Andrew Michael";
            Assert.AreEqual("Joseph Andrew Michael Bloggs", _name.FullName);

            _name.Title = "Mr";
            Assert.AreEqual("Mr Joseph Andrew Michael Bloggs", _name.FullName);

            _name.Qualifications = "B.A.";
            Assert.AreEqual("Mr Joseph Andrew Michael Bloggs, B.A.", _name.FullName);
        }

        [TestMethod]
        public void FullName_Set()
        {
            _name.FullName = "Jane ä Doe";

            Assert.AreEqual("Jane ä Doe", _name.SimpleName);
            Assert.AreEqual("Jane ä", _name.FirstNames);
            Assert.AreEqual("Doe", _name.Surname);
        }

        [TestMethod]
        public void FormalFullName()
        {
            _name.Surname = "Bloggs";
            Assert.AreEqual("Bloggs", _name.FormalFullName);

            _name.FirstNames = "Joseph Andrew Michael";
            Assert.AreEqual("Bloggs, Joseph Andrew Michael", _name.FormalFullName);

            _name.Title = "Mr";
            Assert.AreEqual("Bloggs, Mr Joseph Andrew Michael", _name.FormalFullName);

            _name.Qualifications = "B.A.";
            Assert.AreEqual("Bloggs, Mr Joseph Andrew Michael, B.A.", _name.FormalFullName);
        }

        [TestMethod]
        public void FormalFullName_Set()
        {
            _name.FormalFullName = "Doe, Jane ä";

            Assert.AreEqual("Jane ä Doe", _name.SimpleName);
            Assert.AreEqual("Jane ä", _name.FirstNames);
            Assert.AreEqual("Doe", _name.Surname);
        }

        protected PersonName _name = new PersonName();
    }
}
