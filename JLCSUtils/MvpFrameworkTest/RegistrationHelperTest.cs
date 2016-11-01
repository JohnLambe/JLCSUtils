using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework;
using DiExtension;
using DiExtension.SimpleInject;
using MvpFramework.Binding;

namespace MvpFrameworkTest
{
    [TestClass]
    public class RegistrationHelperTest
    {
        public RegistrationHelperTest()
        {
            RegHelper = new RegistrationHelper(new DiMvpResolver(Context), Context);
        }

        [TestMethod]
        public void GetPresenterFactory()
        {
            // Arrange:
            RegHelper.ScanAssemblies();   // scan this assembly

            // Act:

            var factory = Context.GetInstance<IPresenterFactory<ITest1Presenter>>();
            Console.Out.WriteLine(factory);

            var presenter = factory.Create();
            

            Assert.IsNotNull(factory);
            Assert.IsTrue(presenter is ITest1Presenter);
            //TODO test that view was injected
        }

        [TestMethod]
        public void GetPresenterFactory_Manual()
        {
            // Arrange:

            // General container setup:
            Context.Container.RegisterSingleton(typeof(IDiResolver), Context);
            Context.RegisterType(typeof(MvpResolver), typeof(DiMvpResolver));
            Context.RegisterType(typeof(IResolverExtension), typeof(NullUiManager));

            // Registration of this presenter:
            Context.RegisterType(typeof(ITest1Presenter), typeof(Test1Presenter));
            Context.RegisterType(typeof(IPresenterFactory<ITest1Presenter>), typeof(PresenterFactory<ITest1Presenter>));


            // Act:
            var factory = Context.GetInstance<IPresenterFactory<ITest1Presenter>>();
            Console.Out.WriteLine(factory);

            var presenter = factory.Create();


            // Assert:

            Assert.IsNotNull(factory);
            Assert.IsTrue(presenter is ITest1Presenter);
            //TODO test that view was injected
        }

        [TestMethod]
        public void GetPresenter()
        {
            RegHelper.ScanAssemblies();   // scan this assembly

            var presenter = Context.GetInstance<ITest1Presenter>();
            Console.Out.WriteLine(presenter);

            Assert.IsTrue(presenter is ITest1Presenter);
        }

        public RegistrationHelper RegHelper;
        public SiDiContext Context = new SiDiContext();
    }

    public interface ITest1Presenter : IPresenter
    {

    }

    public class Test1Presenter : ITest1Presenter
    {
        public void Show()
        {
            throw new NotImplementedException();
        }
    }

    public class Test1View : IView
    {
        public void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }

}
