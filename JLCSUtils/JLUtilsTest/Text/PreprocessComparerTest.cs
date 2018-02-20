using JohnLambe.Util.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class PreprocessComparerTest
    {
        [TestMethod]
        public void Compare()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(0, _comparer.Compare("a b c", "abc")),
                () => Assert.IsTrue(_comparer.Compare("a b c", "abd") < 0)
                );
        }

        protected PreprocessComparer<string> _comparer = new PreprocessComparer<string>(s => s?.Replace(" ",""));
    }

    [TestClass]
    public class PreprocessStringComparerTest
    {
        [TestMethod]
        public void Compare()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(0, _comparer.Compare("A B c", "abc")),
                () => Assert.IsTrue(_comparer.Compare("a b c", "ABD") < 0)
                );
        }

        protected PreprocessComparer<string> _comparer = new PreprocessStringComparer(s => s?.Replace(" ", ""), StringComparer.InvariantCultureIgnoreCase);
    }

}
