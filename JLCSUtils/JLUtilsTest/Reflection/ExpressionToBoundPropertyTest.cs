using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class ExpressionToBoundPropertyTest
    {
        [TestMethod]
        public void GetPropertyNameExpressionTest_Simple()
        {
            Expression<Func<string,int>> expression = x => x.Length;
            Assert.AreEqual("Length", ExpressionToBoundProperty.GetPropertyNameExpression(expression));
        }

        [TestMethod]
        public void GetPropertyNameExpressionTest_MultipleLevels()
        {
            Expression<Func<TestClass1, bool, int>> expression = (x,b) => x.Property.Property.Ver.Major;

            Assert.AreEqual("Property.Property.Ver.Major", ExpressionToBoundProperty.GetPropertyNameExpression(expression));
        }

        /// <summary>
        /// The top-level item in the Expression is a cast, which should be ignored.
        /// </summary>
        [TestMethod]
        public void GetPropertyNameExpressionTest_WithCastTopLevel()
        {
            Expression<Func<TestClass1, bool, byte>> expression = (x, b) => (byte)x.Property.Property.Ver.Major;
            Assert.AreEqual("Property.Property.Ver.Major", ExpressionToBoundProperty.GetPropertyNameExpression(expression));
        }

        /// <summary>
        /// There is a cast in the expression at a level below the final property call.
        /// </summary>
        [TestMethod]
        public void GetPropertyNameExpressionTest_WithCast()
        {
            Expression<Func<char, TestClass1, long>> expression = (c, x) => ((string)x.Property.ObjectProperty).Length;
                // There is also an implcit cast at the top level, from int to long.
            Assert.AreEqual("Property.ObjectProperty.Length", ExpressionToBoundProperty.GetPropertyNameExpression(expression));
        }

        public class TestClass1
        {
            public TestClass1 Property { get; }
            public Version Ver { get; }
            public object ObjectProperty { get; }
        }
    }
}
