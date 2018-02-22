using DiExtension;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.DependencyInjection
{
    public class TestBase
    {
        public TestBase()
        {
            Console.WriteLine(ToString() + ".ctor");
        }
    }

    [DiRegisterInstance(Name = "Test1")]  //TODO?: Support registration without name
    public class Test1 : TestBase
    {
        public Test1()
        {
            Instance = this;
        }

        [Dependency("T3")]
        public object Test3Ref { get; set; }

        [Dependency]
        public TestBase TestBaseRef { get; set; }

        public static Test1 Instance = null;
    }

    [DiRegisterType(Priority = -100, ForType = typeof(TestBase))]
    public class Test2Registered : TestBase
    {
        public Test2Registered()
        {
        }
    }

    [DiRegisterInstance(Priority = 100, Name = "T3")]
    public class Test3 : TestBase
    {
        public Test3()
        {
            Instance = this;
        }

        public static Test3 Instance = null;
    }
}
