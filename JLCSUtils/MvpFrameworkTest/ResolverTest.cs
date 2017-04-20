using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvpFramework;
using DiExtension.SimpleInject;
using DiExtension;
using System.Reflection;

namespace MvpFrameworkTest
{
    [TestClass]
    public class ResolverTest
    {
        public ResolverTest()
        {
            Resolver = new DiMvpResolver(Context);
            Resolver.Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
        }

        [TestMethod]
        public void ResolvePresenterType()
        {
            // Act:

            Type t = Resolver.ResolvePresenterType(typeof(IEditPresenter), typeof(TestModel));

            // Assert:

            Assert.AreEqual(typeof(EditTestPresenter), t);
        }

        [TestMethod]
        public void ResolvePresenter()
        {
            // Arrange:
            Context.RegisterType(typeof(EditTestPresenter));
            Assert.IsTrue(Context.GetInstance<EditTestPresenter>() != null);

            // Act:
            var model = new TestModel();
            var t = Resolver.GetPresenterForModel<IEditPresenter, TestModel>(model);

            // Assert:
            Assert.IsTrue(t != null, "Returned null");
            Assert.IsTrue(t is EditTestPresenter, "Wrong type");
        }

        [TestMethod]
        public void ResolvePresenterForAction()
        {
            // Arrange:

            // Act:
            var model = new TestModel();
            var t = Resolver.GetPresenterForModel<IViewPresenter, TestModel>(model);

            // Assert:
            Assert.IsTrue(t != null, "Returned null");
            Assert.IsTrue(t is IViewPresenter, "Wrong type - doesn't implement interface");
            Assert.IsTrue(t is AViewPresenter, "Wrong type");
        }

        protected SiDiContext Context = new SiDiContext();
        protected MvpResolver Resolver { get; set; }
    }

    public class TestModel
    {
    }

    public interface IEditPresenter : IPresenter  // Action interface
    {
    }

    public class EditTestPresenter : IEditPresenter
    {
        public object Show()
        {
            throw new NotImplementedException();
        }
    }


    public interface IViewPresenter : IPresenter   // Action interface
    {
    }

    [PresenterForAction(typeof(TestModel), typeof(IViewPresenter))]
    public class AViewPresenter : IViewPresenter   // class not resolvable by naming convention
    {
        public object Show()
        {
            throw new NotImplementedException();
        }
    }
}
