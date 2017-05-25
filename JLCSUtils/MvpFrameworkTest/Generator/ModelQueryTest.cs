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
    public class ModelQueryTest
    {
        [TestMethod]
        public void ApplyQuery()
        {
            var result = ModelQuery.ApplyQuery(_data.AsQueryable<TestModel>());

            foreach(var item in result)
            {
                Console.WriteLine(item);
            }

            //TODO

        }

        public class TestModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public override string ToString()
            {
                return Id + "; " + (Title ?? "") + "; " + (Content ?? "");
            }

            [ListGenerator]
            public static IQueryable GenerateList(IQueryable<TestModel> all)
            {
                return all.Select(x => new { x.Id, x.Title })
                    .OrderBy(x => x.Id);
            }
        }

        protected TestModel[] _data = new[]
        {
            new TestModel() { Id = 120, Title = "Item 120", Content = "Content 120" },
            new TestModel() { Id = 1, Title = "~Item 1", Content = "Content 1" },
            new TestModel() { Id = 50, Title = "Title" },
        };
    }
}
