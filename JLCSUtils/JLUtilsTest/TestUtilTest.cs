using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest
{
    /// <summary>
    /// Unit tests for <see cref="TestUtil"/>.
    /// </summary>
    [TestClass]
    public class TestUtilTest
    {

        //TODO

        [TestMethod]
        public void AssertEqualWithPrecision()
        {
            TestUtil.Multiple(
                () => TestUtil.AssertThrows<AssertFailedException>( () => TestUtil.AssertEqualWithPrecision(10.1236m, 10.1234m, 3) ),
                () => TestUtil.AssertEqualWithPrecision(-5.728m, -5.734m, 2)
            );
        }

    }
}
