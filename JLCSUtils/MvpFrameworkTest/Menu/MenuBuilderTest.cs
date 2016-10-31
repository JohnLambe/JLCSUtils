using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFrameworkTest.Menu
{
    [TestClass]
    public class MenuBuilderTest
    {
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

            Assert.AreEqual(2, menu.Children.Count());
        }

    }

    [Menu(null, "#MainMenu", "Main Menu")]

    [MenuItem("#MainMenu")]
    public class PresenterInMenu : IPresenter
    {
        public void Show()
        {
            throw new NotImplementedException();
        }
    }

    [MenuItem("#MainMenu", "Presenter2")]
    public class Presenter2InMenu : IPresenter
    {
        public void Show()
        {
            throw new NotImplementedException();
        }
    }

}
