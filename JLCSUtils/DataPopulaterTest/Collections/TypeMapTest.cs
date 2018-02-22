using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class TypeMapTest
    {
        [TestMethod]
        public void Enum()
        {
            TypeMap map = new TypeMap();
            map.Add(typeof(Enum), typeof(int));

            Assert.AreEqual(typeof(int), map[typeof(System.EnvironmentVariableTarget)]);

            map.Add(typeof(object), typeof(string));    // check that it still resolves to the same value after adding a more general mapping

            Assert.AreEqual(typeof(int), map[typeof(System.EnvironmentVariableTarget)]);
            Assert.AreEqual(typeof(string), map[typeof(char)]);    // unmatched by the original mapping
        }

        [TestMethod]
        public void Interface()
        {
            TypeMap map = new TypeMap();
            map.Add(typeof(IComparable), typeof(bool));

            Assert.AreEqual(typeof(bool), map[typeof(string)]);

            map.Add(typeof(object), typeof(decimal));    // check that it still resolves to the same value after adding a more general mapping

            Assert.AreEqual(typeof(bool), map[typeof(string)]);
            Assert.AreEqual(typeof(decimal), map[GetType()]);  // unmatched by the original mapping
        }

    }
}
