using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.DependencyInjection.ConfigInject
{

    public class ConfigInjectTestObject1
    {
        [Dependency("Config:IntegerProperty1")]
        public virtual int IntProp1 { get; set; }

        //[Dependency("Config:NonExistantProperty2")]
        public virtual int IntProp2 { get; set; } = 200;

        [Dependency("Config:NonExistantProperty3=3300")]
        public virtual int IntProp3 { get; set; } = 300;

        [Dependency("Config:IntegerProperty1=4400")]
        public virtual int IntProp4 { get; set; } = 20;

        [Dependency("Config:StringProperty1")]
        public virtual string StrProp1 { get; set; }

        [OptionalDependency("Config:NonExistantStringProperty")]
        public virtual string StrProp2 { get; set; } = "StrProp2DefaultValue";
    }

    public class UnityTestObject1
    {
        //[Dependency("Config:IntegerProperty1")]
        public virtual int IntProp1 { get; set; }

        //[OptionalDependency("Config:NonExistantProperty2")]
        public virtual int? IntProp2 { get; set; } = 100;
        // Value types are not supported for [OptionalDependency], even when nullable.

        //[Dependency("Config:StringProperty1")]
        public virtual string StrProp1 { get; set; }

        [OptionalDependency("Config:NonExistantStringProperty")]
        public virtual string StrProp2 { get; set; } = "StrProp2DefaultValue";
        // Unity overwrites the default value with null.
    }
}
