using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Io;

namespace JohnLambe.Tests.JLUtilsTest.Io
{
    [TestClass]
    public class PathUtilsTest
    {
        [TestMethod]
        public void AppendBeforeExtension()
        {
            const string OriginalFilename = @"f:\directory name\old filename.txt.dat";
            Assert.AreEqual(@"f:\directory name\old filename.txt-inserted.dat", PathUtils.AppendBeforeExtension(OriginalFilename, "-inserted"));
        }

        [TestMethod]
        public void ChangeDirectory()
        {
            Assert.AreEqual(@"D:\newdir\file.txt", PathUtils.ChangeDirectory(@"C:\dir1\subdir\file.txt", @"D:\newdir"));
        }

        [TestMethod]
        public void ChangeFilenameWithoutExtension()
        {
            const string OriginalFilename = @"\\machine\share\dir\old filename.txt";
            Assert.AreEqual(@"\\machine\share\dir\filename with space.txt", PathUtils.ChangeFilenameWithoutExtension(OriginalFilename, "filename with space"));
        }

        [TestMethod]
        public void ChangeFilename()
        {
            const string OriginalFilename = @"Z:\directory name.ext\_asd\old filename.txt.dat";
            
            Assert.AreEqual(@"Z:\directory name.ext\_asd\new filename.doc", PathUtils.ChangeFilename(OriginalFilename, "new filename.doc"));
            
            Assert.AreEqual(@"Z:\directory name.ext\_asd\new filename with no extension", PathUtils.ChangeFilename(OriginalFilename, "new filename with no extension"), "No extension in new filename");
        }
    }
}
