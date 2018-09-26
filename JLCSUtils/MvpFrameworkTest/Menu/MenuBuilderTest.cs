using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Menu(null, "#MainMenu", "Main Menu")]
//[assembly: Menu("#MainMenu", "#Menu1", "Menu 1")]
//[assembly: Menu("#Menu1", "#SubMenu", "Sub Menu")]

namespace MvpFrameworkTest.Menu
{
    [TestClass]
    public class MenuBuilderTest
    {
        public MenuBuilderTest()
        {
            InvokeResult = "";
        }

        [TestMethod]
        public void BuildMenu()
        {
            // Arrange:
            var builder = new MenuBuilder();

            // Act:
            var menuModel = builder.BuildMenu();
            var menu = menuModel.GetRootMenu("#MainMenu");
            Console.Out.WriteLine(menu.MenuHierarchyText());

            // Assert:
            Assert.AreEqual(3, menu.Children.Count(), "Wrong number of menu items");

            var presenter2MenuItem = menu.Children.ElementAt(1);
            Assert.AreEqual("Presenter2", presenter2MenuItem.DisplayName, "DisplayName doesn't match");
            Assert.AreEqual("Presenter2Icon", presenter2MenuItem.IconId, "Icon not populated");
            Assert.AreEqual(KeyboardKey.F8, presenter2MenuItem.HotKey, "HotKey not populated");
        }

        [TestMethod]
        public void BuildMenuAndInvokeItem()
        {
            // Arrange:
            var diContext = new DiExtension.SimpleInject.SiDiContext();
            var builder = new MenuBuilder(new DiMvpResolver(diContext), diContext);

            // Act:
            var menuModel = builder.BuildMenu();
            var menu = menuModel.GetRootMenu("#MainMenu");
            Console.Out.WriteLine(menu.MenuHierarchyText());

            menu.Children.First().Invoke();

            // Assert:

            Assert.AreEqual(3, menu.Children.Count(), "Wrong number of menu items");
            Assert.AreEqual("PresenterInMenu", InvokeResult, "Presenter does not appear to have been invoked");
        }

        [TestMethod]
        public void BuildMenuAndInvokeItem_Static()
        {
            // Arrange:
            var diContext = new DiExtension.SimpleInject.SiDiContext();
            var builder = new MenuBuilder(new DiMvpResolver(diContext), diContext);

            // Act:
            var menuModel = builder.BuildMenu();
            var menu = menuModel.GetMenuItem("#NonPresenterMenuItem");
            Console.Out.WriteLine(menu.MenuHierarchyText());

            menu.Invoke();

            // Assert:

//            Assert.AreEqual(2, menu.Children.Count(), "Wrong number of menu items");
            Assert.AreEqual("NonPresenterMenuItem.MenuExecute", InvokeResult, "Handler does not appear to have been invoked");
        }

        [TestMethod]
        public void BuildMenuAndInvokeItem_DualHandler()
        {
            // Arrange:
            var diContext = new DiExtension.SimpleInject.SiDiContext();
            var builder = new MenuBuilder(new DiMvpResolver(diContext), diContext);

            // Act:
            var menuModel = builder.BuildMenu();
            var menu = menuModel.GetMenuItem("#DualHandlerMenuItem");
            Console.Out.WriteLine(menu.MenuHierarchyText());

            //var menu = menuModel.GetMenuItem("#NonPresenterMenuItem");
            //menu = menu.Children.First();
            Console.Out.WriteLine(menu.MenuHierarchyText());

            menu.Invoke();

            // Assert:

            //            Assert.AreEqual(2, menu.Children.Count(), "Wrong number of menu items");
            Assert.AreEqual("DualHandlerMenuItem.MenuExecute\nDualHandlerMenuItem.Show\n", InvokeResult, "Handler not invoked or wrong handler invoked");
        }

        [TestMethod]
        public void GetNestedMenuItem()
        {
            // Arrange:
            var diContext = new DiExtension.SimpleInject.SiDiContext();
            var builder = new MenuBuilder(new DiMvpResolver(diContext), diContext);

            // Act:
            var menuModel = builder.BuildMenu();
            var menu = menuModel.GetMenuItem("#DualHandlerMenuItem");
            Console.Out.WriteLine(menu.MenuHierarchyText());

            // Assert:

            Assert.AreEqual("DualHandlerMenuItem", menu.DisplayName);
        }

        public static string InvokeResult;
    }


    [MenuItem("#MainMenu")]
    public class PresenterInMenu : IPresenter
    {
        public object Show()
        {
            Console.Out.WriteLine("PresenterInMenu invoked");
            MenuBuilderTest.InvokeResult = "PresenterInMenu";
            return "PresenterInMenu";
        }
    }

    [MenuItem("#MainMenu", "Presenter2", IconId = "Presenter2Icon", HotKey = KeyboardKey.F8)]
    public class Presenter2InMenu : IPresenter
    {
        public object Show()
        {
            throw new NotImplementedException();
        }
    }

    [MenuItem("#MainMenu", "NonPresenterMenuItem", Id = "#NonPresenterMenuItem", Order = 2000)]
    public class NonPresenterMenuItem
    {
        public static bool MenuExecute()
        {
            Console.Out.WriteLine("PresenterInMenu invoked");
            MenuBuilderTest.InvokeResult = "NonPresenterMenuItem.MenuExecute";
            return true;
        }
    }

    [MenuItem("#NonPresenterMenuItem", "DualHandlerMenuItem", Id = "#DualHandlerMenuItem", Order = 3000)]
    public class DualHandlerMenuItem : IPresenter
    {
        public object Show()
        {
            Console.Out.WriteLine("DualHandlerMenuItem invoked");
            MenuBuilderTest.InvokeResult += "DualHandlerMenuItem.Show\n";
            return null;
        }

        public static bool MenuExecute()
        {
            Console.Out.WriteLine("DualHandlerMenuItem.MenuExecute() invoked");
            MenuBuilderTest.InvokeResult += "DualHandlerMenuItem.MenuExecute\n";
            return true;
        }
    }


}
