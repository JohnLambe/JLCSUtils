using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Collections;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;
using System.Collections.Generic;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class DictionaryUtilTest
    {
        [TestMethod]
        public void ObjectTryGetValue()
        {
            Multiple(
                () => Assert.AreEqual(0, DictionaryUtil.ObjectTryGetValue<string, int>(null, "a"), "no dictionary"),

                () =>
                {
                    var d = new Dictionary<int, string>();
                    d.SetValue(10, "test");
                    Assert.AreEqual("test", DictionaryUtil.ObjectTryGetValue<int, string>(d, 10), "key exists");
                    Assert.AreEqual(null, DictionaryUtil.ObjectTryGetValue<int, string>(d, 20), "key does not exist");
                },

                () => TestUtil.AssertThrows<Exception>(() => DictionaryUtil.ObjectTryGetValue<string, string>(5, "key"))    // invalid type for dictionary
                );
        }

        [TestMethod]
        public void ObjectTrySetValue()
        {
            Multiple(
                () =>
                {
                    object d = null;
                    d = DictionaryUtil.ObjectTrySetValue<string, int>(d, "a", 5);

                    Assert.IsTrue(d is Dictionary<string,int>);   // dictionary created
                    Assert.AreEqual(5, DictionaryUtil.ObjectTryGetValue<string,int>(d,"a"));
                },

                () =>
                {
                    // Dictionary exists:
                    var d = new Dictionary<int, string>();
                    d.SetValue(10, "test");

                    Assert.AreEqual("test", DictionaryUtil.ObjectTryGetValue<int, string>(d, 10), "key exists");
                    Assert.AreEqual(null, DictionaryUtil.ObjectTryGetValue<int, string>(d, 20), "key does not exist");

                    // Overwrite value:
                    d.SetValue(10, "test2");
                    Assert.AreEqual("test2", DictionaryUtil.ObjectTryGetValue<int, string>(d, 10), "overwriting value");
                },

                () => 
                {
                    object d = new Dictionary<int, string>();
                    TestUtil.AssertThrows<Exception>(() => DictionaryUtil.ObjectTrySetValue<string, string>(d, "key", "value"));    // invalid type for dictionary
                }
                );
        }
    }
}
