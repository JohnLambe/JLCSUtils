using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvpFramework;
using DiExtension.SimpleInject;
using DiExtension;
using System.Reflection;
using MvpFramework.Binding;
using System.Linq;

namespace MvpFrameworkTest.Resolver_Presenter
{
    /// <summary>
    /// Multiple presenter interfaces.
    /// </summary>
    [TestClass]
    public class Resolver_Presenter
    {
        public Resolver_Presenter()
        {
            Resolver = new DiMvpResolver(Context);
            Resolver.Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            new RegistrationHelper(Resolver, Context).ScanAssemblies(Resolver.Assemblies);
        }

        [TestMethod]
        public void ResolveInterfaceForPresenterType()
        {
            // Act:
            var presenterInterface = Resolver.ResolveInterfaceForPresenterType(typeof(TestPresenter));

            // Assert:
            Assert.AreEqual(typeof(ITestPresenter1),presenterInterface);
        }

        [TestMethod]
        public void ResolveInterfacesForPresenterType()
        {
            var interfaces = Resolver.ResolveInterfacesForPresenterType(typeof(TestPresenter)).ToArray();

            Assert.AreEqual(typeof(ITestPresenter1), interfaces[0]);
            Assert.AreEqual(typeof(ITestPresenter2), interfaces[1]);
            Assert.AreEqual(2, interfaces.Length);
        }

        [TestMethod]
        public void GetViewForPresenter()
        {
            // Act:
            var view = Resolver.GetViewForPresenter<IView, IPresenter>(new TestPresenter());

            // Assert:
            Assert.IsTrue(view is ITestView1);
        }

        protected SiDiContext Context = new SiDiContext();
        protected MvpResolver Resolver { get; set; }
    }


    public interface ITestPresenter1 : IPresenter
    {
    }

    public interface ITestPresenter2 : IPresenter
    {
    }

    [Presenter(ViewInterface = typeof(ITestView1), Interfaces = new[] { typeof(ITestPresenter1), typeof(ITestPresenter2) })]
    public class TestPresenter : ITestPresenter1, ITestPresenter2
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
