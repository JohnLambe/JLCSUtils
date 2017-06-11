using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using JohnLambe.Util.GraphicUtil;
using JohnLambe.Util.Collections;

namespace JohnLambe.Tests.JLUtilsTest.GraphicUtil
{
    [TestClass]
    public class ImageExtensionTest
    {
        /// <summary>
        /// Convert an image to bytes and back.
        /// </summary>
        [TestMethod]
        public void ToAndFromBytes()
        {
            var image = Properties.Resources.user;

            var bytes = image.ToBytes();
            var reconstructedImage = ImageUtil.FromBytes(bytes);
            var bytes2 = reconstructedImage.ToBytes();

            Assert.IsTrue(bytes.SequenceEqual(bytes2));
        }
    }
}
