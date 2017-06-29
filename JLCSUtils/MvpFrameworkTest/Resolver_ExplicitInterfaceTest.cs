using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvpFramework;
using DiExtension.SimpleInject;
using DiExtension;
using System.Reflection;
using MvpFramework.Binding;
using System.Linq;

namespace MvpFrameworkTest.Resolver_ExplicitView
{
    [TestClass]
    public class Resolver_ExplicitInterfaceTest
    {
        public Resolver_ExplicitInterfaceTest()
        {
            Resolver = new DiMvpResolver(Context);
            Resolver.Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            new RegistrationHelper(Resolver,Context).ScanAssemblies(Resolver.Assemblies);
        }

        [TestMethod]
        public void GetViewForPresenterType()
        {
            // Act:
            var view = Resolver.GetViewForPresenterType<IView>(typeof(TestPresenter));

            // Assert:
            Assert.IsTrue(view is ITestView1);
        }

        [TestMethod]
        public void GetViewForPresenter()
        {
            // Act:
            var view = Resolver.GetViewForPresenter<IView,IPresenter>(new TestPresenter());

            // Assert:
            Assert.IsTrue(view is ITestView1);
        }

        [TestMethod]
        public void GetViewInterfaces()
        {
            var interfaces = Resolver.ResolveInterfacesForViewType(typeof(ATestView)).ToArray();

            Assert.AreEqual(typeof(ITestView1), interfaces[0]);
            Assert.AreEqual(typeof(ITestView2), interfaces[1]);
        }

        protected SiDiContext Context = new SiDiContext();
        protected MvpResolver Resolver { get; set; }
    }


    public interface ITestPresenter : IPresenter
    {
    }

    [Presenter(ViewInterface = typeof(ITestView2))]
    public class TestPresenter : ITestPresenter
    {
        public object Show()
        {
            throw new NotImplementedException();
        }
    }


    public interface ITestView1 : IView
    {
    }
    public interface ITestView2 : IView
    {
    }

    [View(Interfaces = new[] { typeof(ITestView1), typeof(ITestView2) })]
    public class ATestView : ITestView1, ITestView2  // class not resolvable by naming convention
    {
        public void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            throw new NotImplementedException();
        }

        public void RefreshView()
        {
            throw new NotImplementedException();
        }

        public object Show()
        {
            throw new NotImplementedException();
        }
    }

}
