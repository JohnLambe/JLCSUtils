using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

using JohnLambe.Util.Text;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class StringBuilderExtensionTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="StrUtilTest.ReplaceSubstring"/>
        [TestMethod]
        public void ReplaceSubstring()
        {
            Multiple(
                () =>
                {
                    StringBuilder sb = new StringBuilder("qwertyuiop");
                    TestUtil.AssertValueEqual("qweABCDyuiop", sb.ReplaceSubstring(3, 2, "ABCD").ToString());
                },

                () =>
                {
                    StringBuilder sb = new StringBuilder("qwertyuiop");
                    TestUtil.AssertValueEqual("qweABCDrtyuiop", sb.ReplaceSubstring(3, 0, "ABCD").ToString(), "0-length section being replaced");
                },

                () =>
                {
                    StringBuilder sb = new StringBuilder("qwertyuiop");
                    AssertThrows<ArgumentException>(
                        () => sb.ReplaceSubstring(3, -1, "ABCD"));
                },

                () =>
                {
                    StringBuilder sb = new StringBuilder("qwertyuiop");
                    AssertThrows<IndexOutOfRangeException>(
                        () => sb.ReplaceSubstring(-0x1000000, 5, "ABCD"));
                }

                //TODO: null; out-of-range
            );
        }
    }
}
