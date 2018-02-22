using JohnLambe.Util.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Types
{
    [TestClass]
    public class GuidUtilTest
    {
        [TestMethod]
        public void CompactForm()
        {
            Multiple(
                () => Assert.AreEqual("c0c24105e06b44eca4c13d538785ae9a", GuidUtil.CompactForm(new Guid("c0c24105-e06b-44ec-a4c1-3d538785ae9a"))),
                () => Assert.AreEqual("00000000000000000000000000000000", GuidUtil.CompactForm(new Guid())),
                () => Assert.AreEqual(null, null)
            );
        }

        [TestMethod]
        public void GetVariant()
        {
            Multiple(
                () => Assert.AreEqual(0, GuidUtil.GetVariant(new Guid())),
                () => Assert.AreEqual(1, GuidUtil.GetVariant(new Guid("593d4105-e06b-e4ec-a4c1-3d538785ae9a"))), // Variant 1, type 0xE (undefined type)
                () => Assert.AreEqual(2, GuidUtil.GetVariant(new Guid("c0c24105-e06b-34ec-c4c1-3d538785ae9a"))),
                () => Assert.AreEqual(-1, GuidUtil.GetVariant(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")))
            );
        }

        [TestMethod]
        public void GetVersion()
        {
            Multiple(
                () => Assert.AreEqual(-1, GuidUtil.GetVersion(new Guid())),
                () => Assert.AreEqual(0xE, GuidUtil.GetVersion(new Guid("593d4105-e06b-e4ec-a4c1-3d538785ae9a"))), // Variant 1, type 0xE (undefined type)
                () => Assert.AreEqual(3, GuidUtil.GetVersion(new Guid("c0c24105-e06b-34ec-c4c1-3d538785ae9a"))),
                () => Assert.AreEqual(-1, GuidUtil.GetVersion(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")))
            );
        }

    }
}
