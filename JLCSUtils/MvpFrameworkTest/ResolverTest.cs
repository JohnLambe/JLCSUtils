using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MvpFramework;
using DiExtension.SimpleInject;
using DiExtension;

namespace MvpFrameworkTest
{
    [TestClass]
    public class ResolverTest
    {
        public ResolverTest()
        {
            Resolver = new DiMvpResolver(Context);
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

        protected SiDiContext Context = new SiDiContext();
        protected MvpResolver Resolver { get; set; }
    }

    public class TestModel
    {
    }

    public interface IEditPresenter : IPresenter
    {
    }

    public class EditTestPresenter : IEditPresenter
    {
        public object Show()
        {
            throw new NotImplementedException();
        }
    }
}
