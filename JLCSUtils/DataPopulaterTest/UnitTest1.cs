using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.DataPopulater;
using JohnLambe.Util.Xml;
using JohnLambe.Test.DataPopulater;

namespace JohnLambe.Test.DataPopulaterTest
{
    [TestClass]
    public class DataPopulaterTest1
    {
        public DataPopulaterTest1()
        {
            _engine.Config = new DataPopulaterConfig();

            _engine.Config.Classes = new ClassConfig[] {
                new ClassConfig()
                {
                    ClassName = "JohnLambe.Test.DataPopulaterTest.TestClass",
                    Properties = new PropertyConfigBase[]
                    {
                        new ConstantPropertyConfig()
                        {
                            PropertyName = "A",
                            Value = 1234
                        },
                        new RandomChoicePropertyConfig()
                        {
                            PropertyName = "B",
                            Values = new string[] { "Name1", "Name2", "Name3" }
                        },
                    }
                }
            };


            _loader.Namespace = "JohnLambe.DataPopulater";
        }


        [TestMethod]
        public void TestMethod1()
        {
            _engine.Run();

        }

        private void _engine_OnSaveInstnace(object sender, DataPopulaterEngine.SaveInstanceArgs e)
        {
            _lastInstance = e.Instance;
            _instanceCount++;
        }
        protected object _lastInstance;
        protected int _instanceCount;

        [TestMethod]
        public void TestMethod2()
        {
            _engine.Config = (DataPopulaterConfig) _loader.Parse(XmlUtil.LoadFromFile(@"C:\Dev\Microsoft\DataPop\Config.xml"));
            _engine.OnSaveInstnace += _engine_OnSaveInstnace;

            _engine.Run();

        }

        protected DataPopulaterEngine _engine = new DataPopulaterEngine();

        protected XmlObjectLoader _loader = new XmlObjectLoader();
    }

    public class TestClass
    {
        public virtual int A { get; set; }

        public virtual string B { get; set; }

    }

    public class TargetClass1
    {
        public virtual string FirstNames { get; set; }
        public virtual string Surname { get; set; }

        public virtual char Gender { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual TestEnum Enum { get; set; }

        public virtual string X { get; set; }

        public virtual int Y { get; set; }

        public virtual TestClass Nested { get; set; } = new TestClass();
    }


}
