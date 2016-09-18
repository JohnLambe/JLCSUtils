using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ObjectCopierTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            ObjectCopier copier = new ObjectCopier();
            Source src = new Source()
            {
                A = TestEnum.Flag2 | TestEnum.Flag4,
                Prop2 = "test"
            };

            var dest = copier.CreateCopy<CopierTest>(src);
            Console.Out.WriteLine(dest);

            var codeGen = new JavaClassCodeGen<IClassMapping>();
            Console.Out.WriteLine(codeGen.Generate(copier.GetMetadata(typeof(Source), typeof(CopierTest))));

        }
    }


    [Flags]
    enum TestEnum
    {
        Flag2 = 2,
        Flag3 = 4,
        Flag4 = 8
    }

    [MapType(SourceType = typeof(Source))]
    class CopierTest
    {
        [Map("A", Flag = TestEnum.Flag3)]
        public virtual bool Prop1 { get; set; }

        public virtual string Prop2 { get; set; }
        public virtual string DestOnly { get; set; }
    }


    class Source
    {
        public TestEnum A { get; set; }
        public virtual string Prop2 { get; set; }
        public virtual string SourceOnly { get; set; }
    }

}
