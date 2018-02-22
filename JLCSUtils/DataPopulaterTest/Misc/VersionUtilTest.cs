using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Misc;
using System.Diagnostics;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Misc
{
    [TestClass]
    public class VersionUtilTest
    {
        [TestMethod]
        public void FileVersionToVersion()
        {
            Console.WriteLine(VersionUtil.FileVersionToVersion(_fileVersion));

            Assert.AreEqual(_fileVersion.FileVersion, VersionUtil.FileVersionToVersion(_fileVersion).ToString());
        }

        [TestMethod]
        public void ProductVersionToVersion()
        {
            Assert.AreEqual(_fileVersion.ProductVersion, VersionUtil.ProductVersionToVersion(_fileVersion).ToString());
        }

        protected FileVersionInfo _fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
    }
}
