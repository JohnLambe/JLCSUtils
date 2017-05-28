using DiExtension.ConfigInject.Providers;
using JohnLambe.Util.ConfigProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.ConfigProvider
{
    [TestClass]
    public class IconRepositoryTest
    {

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), AllowDerivedTypes = true)]
        public void GetIcon_NotFound()
        {
            _repository.GetIcon("doesnt_exist");
        }
        

        protected Util.ConfigProvider.IconRepository<Image> _repository = new IconRepository<Image>(null,new[] { "" });
    }
}
