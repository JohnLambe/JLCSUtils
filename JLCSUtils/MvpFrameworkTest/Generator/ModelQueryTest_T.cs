using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework.Generator;
using MvpFramework.Binding;

namespace MvpFrameworkTest.Generator
{
    [TestClass]
    public class ModelQueryTest_T
    {
        [TestMethod]
        public void ApplyQueryT()
        {
            // Act:
            IQueryable<TestModel1> result = ModelQuery.ApplyQueryT(_data.AsQueryable<TestModel1>());

            // Assert:

            string resultsString = "";
            foreach (var item in result)
            {
                Console.WriteLine(item);
                resultsString = resultsString + item.ToString() + "\n";
            }

            Assert.AreEqual("1; ~Item 1; Content 1\n120; Item 120; Content 120\n", resultsString);
        }

        [TestMethod]
        public void ApplyQuery_NotFound_Ignore()
        {
            var all = new object[] { "a" }.AsQueryable<object>();
            Assert.AreEqual(all, ModelQuery.ApplyQuery(all,null,null,true));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), AllowDerivedTypes = true)]
        public void ApplyQuery_NotFound()
        {
            var all = new object[] { "a" }.AsQueryable<object>();
            Assert.AreEqual(all, ModelQuery.ApplyQuery(all));
        }

        public class TestModel1
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public override string ToString()
            {
                return Id + "; " + (Title ?? "") + "; " + (Content ?? "");
            }

            [ListGenerator]
            public static IQueryable<TestModel1> GenerateList(IQueryable<TestModel1> all)
            {
                return all.Where(x => x.Id != 50)
                    .OrderBy(x => x.Id);
            }
        }

        protected TestModel1[] _data = new[]
        {
            new TestModel1() { Id = 120, Title = "Item 120", Content = "Content 120" },
            new TestModel1() { Id = 1, Title = "~Item 1", Content = "Content 1" },
            new TestModel1() { Id = 50, Title = "Title" },
        };
    }
}
