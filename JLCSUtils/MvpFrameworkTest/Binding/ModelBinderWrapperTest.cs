using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFrameworkTest.Binding
{
    [TestClass]
    public class ModelBinderWrapperTest
    {
        [TestMethod]
        public void GetValue()
        {
            ModelBinderWrapper model = new ModelBinderWrapper("qwerty");

            Assert.AreEqual(6, model.GetValue("Length"));
        }

    }
}
