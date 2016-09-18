using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.DependencyInjection;
using JohnLambe.Util.DependencyInjection.SimpleInject;
using JohnLambe.Util.DependencyInjection.Attributes;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    public class TestInjectedObject1
    {
        public const int OptionalDependencyDefaultValue = 101;

        public TestInjectedObject1()
        {
        }

        [Inject("GlobalValue")]
        public string GlobalValue1 { get; set; }
            // Simple item injected from the DI context, looked up with the name in the attribute.

        [Inject(InjectAttribute.CodeName)]              // The Key is the property name.
        public string GlobalValue2 { get; set; }

        [Inject("RegisteredObject.Property1")]
        public string SubProperty { get; set; }
        // Looks up 'RegisteredObject' in the DI context, then 'Property1' of it.

        [Inject(InjectAttribute.CodeName)]
        public object RegisteredObject { get; set; }
        // Key is the property name.

        [Inject(InjectAttribute.CodeName, Required = false)]
        public int? UnresolvedOptionalDependency { get; set; } = OptionalDependencyDefaultValue;
        // unresolved. Should have its initial value.

        [Inject(InjectAttribute.CodeName, Required = false)]
        public object UnresolvedOptionalNullableDependency { get; set; }
        // unresolved. Should be null (which is its initial value).

        [Inject(InjectAttribute.CodeName, Required = false)]
        public int ResolvedOptionalDependency { get; set; } = OptionalDependencyDefaultValue;
        // Injected just like a Required one.

//        [Inject(Name="ResolvedOptionalDependency", Required = false)]
//        public int? ResolvedOptionalDependency2 { get; set; } = OptionalDependencyDefaultValue;

        public string NotInjected { get; set; }

        [Inject]   // Resolved by type (default)
        public TestRegisteredObject1 RegisteredObjectResolvedByType { get; set; }

    }


    public class TestRegisteredObject1
    {
        public string Property1 { get; set; } = "(Property 1)";

        public string Property2 { get; set; }
    }

}
