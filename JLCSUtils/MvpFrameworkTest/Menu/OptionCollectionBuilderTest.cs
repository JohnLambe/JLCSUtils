using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Menu;
using MvpFramework.Binding;
using JohnLambe.Util;

namespace MvpFrameworkTest.Menu
{
    [TestClass]
    public class OptionCollectionBuilderTest
    {
        public OptionCollection BuildOptionsInternal(string filter = "filter")
        {
            var builder = new OptionCollectionBuilder();

            // Act:
            var options = builder.Build(_target, filter);

            return options;
        }

        [TestInitialize]
        public void TestInitialise()
        {
            TestHandlerClass.ResetLog();
        }

        [TestMethod]
        public void BuildOptions()
        {
            var options = BuildOptionsInternal();

            // Assert:
            Assert.AreEqual(options.Children.Count(), 4);
            Assert.AreEqual("Handler 1", options.Children.First().DisplayName);
        }

        [TestMethod]
        public void InvokeHandler_2Args()
        {
            // Arrange:
            var options = BuildOptionsInternal();

            // Act:
            var option = options.Children.Last();
            var args = new MenuItemModel.InvokedEventArgs();
            option.Invoke(args);

            // Assert:
            Assert.IsTrue(TestHandlerClass.Log.StartsWith("Handler4:"));
            Assert.AreEqual("Handler4", TestHandlerClass.Sender.Id);
            Assert.AreEqual(args, TestHandlerClass.Args);
        }

        /// <summary>
        /// Invoke an event handler that takes an arguments object (only).
        /// </summary>
        [TestMethod]
        public void InvokeHandler_1Arg()
        {
            // Arrange:
            var options = BuildOptionsInternal("H3");

            // Act:
            var option = options.Children.First();
            option.Invoke();
            Console.WriteLine("Log: " + TestHandlerClass.Log);

            Assert.IsTrue(TestHandlerClass.Log.StartsWith("Handler3:"));
            Assert.AreEqual(MenuItemModel.InvokedEventArgs.EmptyInvokedEventArgs, TestHandlerClass.Args);  // no arguments passed
                    // It is not actually required to be MenuItemModel.InvokedEventArgs.EmptyInvokedEventArgs. Another empty instance would be valid.

            var args = new MenuItemModel.InvokedEventArgs();
            option.Invoke(args);

            // Assert:
            Assert.IsTrue(TestHandlerClass.Log.StartsWith("Handler3:"));
            Assert.AreEqual(args, TestHandlerClass.Args);  // no arguments passed
        }

        /// <summary>
        /// Invoke an event handler that takes the sender (only) as an argument.
        /// </summary>
        [TestMethod]
        public void InvokeHandler_SenderArg()
        {
            // Arrange:
            var options = BuildOptionsInternal("H2");

            TestHandlerClass.ResetLog();

            // Act:
            var option = options.Children.First();
            option.Invoke();

            Assert.IsTrue(TestHandlerClass.Log.StartsWith("Handler2:"), "Handler not invoked, or wrong handler was invoked");
            Assert.AreEqual("Handler2", TestHandlerClass.Sender.Id, "Wrong Sender passed to handler");

            TestHandlerClass.ResetLog();

            var args = new MenuItemModel.InvokedEventArgs();
            option.Invoke(args);

            // Assert:
            Assert.IsTrue(TestHandlerClass.Log.StartsWith("Handler2:"), "Handler not invoked, or wrong handler was invoked");
            Assert.AreEqual("Handler2", TestHandlerClass.Sender.Id, "Wrong Sender passed to handler");
        }

        protected TestHandlerClass _target = new TestHandlerClass();

        public class TestHandlerClass
        {
            public static string Log = "";
            public static MenuItemModel Sender;
            public static object Args;

            public static void ResetLog()
            {
                Log = "";
                Sender = null;
                Args = null;
            }

            public void AddToLog(string message)
            {
                Console.Out.WriteLine(message);
                Log = Log.NullToBlank() + message + "\n";
            }

            [MvpHandler(SingleFilter = "filter")]
            public void Handler1()
            {
                AddToLog("Handler1");
            }

            [MvpHandler(SingleFilter = "Filter")]  // Filter names are case-sensitive
            public void ExcludedHandler()
            {
            }

            [MvpHandler(SingleFilter = "H2")]
            [MvpHandler(SingleFilter = "filter")]
            public void Handler2(MenuItemModel sender)
            {
                AddToLog("Handler2: " + sender.ToString());
                Sender = sender;
            }

            [MvpHandler(SingleFilter = "filter")]
            [MvpHandler(SingleFilter = "H3")]
            public void Handler3(MenuItemModel.InvokedEventArgs args)
            {
                AddToLog("Handler3: " + args);
                Args = args;
            }

            [MvpHandler(SingleFilter = "filter")]
            public void Handler4(MenuItemModel sender, MenuItemModel.InvokedEventArgs args)
            {
                AddToLog("Handler4: " + sender?.ToString() + "; " + args?.ToString());
                Sender = sender;
                Args = args;
            }
        }
    }
}
