using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvpFramework;
using MvpFramework.Binding;
using System.Linq;
using MvpFramework.Menu;

namespace MvpFrameworkTest
{
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public void OptionCollectionBuilder()
        {
            // Act:
            var target = new TestClassForOptionSet();
            var optionCollection = new OptionCollectionBuilder().Build(target, "filter1");
            optionCollection.Children.First().Invoke();

            Assert.AreEqual("Handler 1b display name", optionCollection.Children.First().DisplayName);
            Assert.AreEqual(3, optionCollection.Children.Count());
            Assert.AreEqual("Method1;", target.Output);
        }

        [TestMethod]
        public void HandlerResolverTest()
        {
            // Arrange:
            var resolver = new HandlerResolver();
            var target = new TestClassForOptionSet();

            // Act:
            var handler = resolver.GetHandler(target,"A");
            handler.Invoke();
            Console.WriteLine(target.Output);

            // Assert:
            Assert.AreEqual("Method3;Method1;Method3;",target.Output);
        }

        [TestMethod]
        public void AddOption()
        {
            // Arrange:
            var target = new TestClassForOptionSet();
            var optionCollection = new OptionCollectionBuilder().Build(target, "filter1");

            // Act:
            //            var newItem = optionCollection.NewOption("new");
            var newItem = optionCollection.AddOption(
                new MvpFramework.Menu.MenuItemModel("new")
                {
                    DisplayName = "New Item",
                    Order = 5
                }
                );
            Console.Out.WriteLine();

            // Assert:
            Assert.AreEqual(4, optionCollection.Children.Count());
            Assert.AreEqual(newItem, optionCollection.Children.ToArray()[2]);
        }
    }

    public class TestClassForOptionSet
    {
        /// <summary>
        /// Methods of this class append to this.
        /// Used to test which ones fired, and in what order.
        /// </summary>
        public string Output;

        [MvpHandler("Handler1", DisplayName = "Handler 1 display name", Order = 50, Filter = new string[] { "filter1" })]
        [MvpHandler("Handler1b", DisplayName = "Handler 1b display name", Order = -100, Filter = new string[] { "asd", "filter1" })]
        [MvpHandler("A")]
        public virtual void Method1()
        {
            Output += "Method1;";
        }

        [MvpHandler("Handler2", DisplayName = "Handler 2 display name", Filter = new string[] { "filter1" })]
        public virtual void Method2()
        {
            Output += "Method2;";
        }

        [MvpHandler("A", Order = -10)]
        [MvpHandler("Handler3", DisplayName = "Handler 3 display name", Filter = new string[] { "a", "b" })]
        [MvpHandler("A")]
        public virtual void Method3()
        {
            Output += "Method3;";
        }

        [MvpHandler("Handler4", DisplayName = "Handler 4 display name")]
        public virtual void Method4()
        {
            Output += "Method4;";
        }
    }
}
