// by John Lambe - Public Domain.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Io;
using System.IO;

namespace JohnLambe.Tests.JLUtilsTest.Io
{
    [TestClass]
    public class FilingUtilsTest
    {
        [TestMethod]
        public void OpenAppendOrCreate_Create()
        {
            string filename = @"c:\test\file.dat";  //TODO

            if(File.Exists(filename))
                File.Delete(filename);

            var stream = FilingUtils.OpenAppendOrCreate(filename,true);
            try
            {
                Assert.IsNotNull(stream, "Returned null");
                stream.WriteByte(65);
                stream.Close();
            }
            finally
            {
                stream.Dispose();
            }

            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(filename)),"Didn't create directory");
            Assert.IsTrue(File.Exists(filename),"Didn't create file");
            Assert.AreEqual("A",File.ReadAllText(filename),"Wrong file contents");
        }

        [TestMethod]
        public void OpenAppendOrCreate_Append()
        {
            string filename = @"c:\test\file2.dat";  //TODO

            File.WriteAllText(filename, "test");

            var stream = FilingUtils.OpenAppendOrCreate(filename, true);
            try
            {
                Assert.IsNotNull(stream, "Returned null");
                stream.WriteByte((byte)'X');
                stream.Close();
            }
            finally
            {
                stream.Dispose();
            }

            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(filename)), "Didn't create directory");
            Assert.IsTrue(File.Exists(filename), "Didn't create file");
            Assert.AreEqual("testX", File.ReadAllText(filename), "Wrong contents"); // maybe didn't seek to end
        }
    }
}
