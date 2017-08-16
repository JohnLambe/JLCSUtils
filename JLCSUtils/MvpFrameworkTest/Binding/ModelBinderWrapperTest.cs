using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFrameworkTest.Binding
{
    [TestClass]
    public class ModelBinderWrapperTest
    {
        [TestMethod]
        public void Property_Value()
        {
            ModelBinderWrapper model = new ModelBinderWrapper("qwerty");  // the model is a string

            Assert.AreEqual(6, model.GetProperty("Length").Value);
        }

        [TestMethod]
        public void GetPropertiesByGroup()
        {
            ModelBinderWrapper model = new ModelBinderWrapper(new ModelB());

            string properties = "";
            foreach(var prop in model.GetPropertiesByGroup(null))   // all groups
            {
                Console.WriteLine(prop.Name);
                properties += prop.Name + ";";
            }

            Assert.AreEqual("B1;B2;A1;A2;A3;B3;", properties);   // the default order is: derived class first, then declared order
        }


        [TestMethod]
        public void Property_DisplayName()
        {
            ModelBinderWrapper model = new ModelBinderWrapper(new Model1());

            Assert.AreEqual("Property Name", model.GetProperty("PropertyName").DisplayName);
        }

        [TestMethod]
        public void Property_DisplayName_Attribute()
        {
            ModelBinderWrapper model = new ModelBinderWrapper(new ModelB());

            Assert.AreEqual("Property B3", model.GetProperty("B3").DisplayName);
        }


        public class ModelA
        {
            public string A1 { get; set; }
            [Display(GroupName = "Group1")]
            public string A2 { get; set; }
            [Display(Order = 100)]
            public string A3 { get; set; }
        }
        public class ModelB : ModelA
        {
            public string B1 { get; set; }
            public string B2 { get; set; }
            [Display(Order = 200, Name = "Property B3")]
            public string B3 { get; set; }
        }

        public class Model1
        {
            public string PropertyName { get; set; }
        }
    }
}
