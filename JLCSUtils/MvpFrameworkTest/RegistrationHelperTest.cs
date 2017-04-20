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
using DiExtension.Attributes;

namespace MvpFrameworkTest
{
    [TestClass]
    public class RegistrationHelperTest
    {
        public RegistrationHelperTest()
        {
            var mvpResolver = new DiMvpResolver(Context);
            Context.Container.RegisterSingleton<MvpResolver>(mvpResolver);
            Context.Container.RegisterSingleton(typeof(IDiResolver), Context);
            Context.RegisterType(typeof(IResolverExtension), typeof(NullResolverExtension));

            Context.Container.RegisterSingleton(typeof(IControlBinderFactory), new ControlBinderFactory());

            RegHelper = new RegistrationHelper(mvpResolver, Context);
        }

        /// <summary>
        /// Resolve a PresenterFactory to test automatic registration.
        /// <para>
        /// This tests the automatic registration which should be equivalent to the registration done in <see cref="GetPresenterFactory_Manual"/>.
        /// </para>
        /// </summary>
        [TestMethod]
        public void GetPresenterFactory()
        {
            // Arrange:
            RegHelper.ScanAssemblies();   // scan this assembly

            // Act:

            var factory = Context.GetInstance<IPresenterFactory<ITest1Presenter,object>>();
            Console.Out.WriteLine(factory);

            // Assert:

            // Invoke the resolved factory:
            var presenter = factory.Create("test");

            Assert.IsNotNull(factory);
            Assert.IsTrue(presenter is ITest1Presenter);
            //TODO test that view was injected
        }

        /// <summary>
        /// Manually (i.e. without scanning) register everything required, then resolve a Presenter.
        /// </summary>
        [TestMethod]
        public void GetPresenterFactory_Manual()
        {
            // Arrange:

            /*
            // General container setup:
            Context.Container.RegisterSingleton(typeof(IDiResolver), Context);
            Context.RegisterType(typeof(MvpResolver), typeof(DiMvpResolver));
            Context.RegisterType(typeof(IResolverExtension), typeof(NullUiManager));
            */

            // Registration of this presenter:
            Context.RegisterType(typeof(ITest1Presenter), typeof(Test1Presenter));
            Context.RegisterType(typeof(IPresenterFactory<ITest1Presenter,object>), typeof(PresenterFactory<ITest1Presenter,object>));

            Context.RegisterType(typeof(ITest1View), typeof(Test1View));

            // Act:
            var factory = Context.GetInstance<IPresenterFactory<ITest1Presenter,object>>();
            Console.Out.WriteLine(factory);

            var presenter = factory.Create("test");


            // Assert:

            Assert.IsNotNull(factory);
            Assert.IsTrue(presenter is ITest1Presenter);
            //TODO test that view was injected
        }

        /// <summary>
        /// Resolve a Presenter after automatic registration.
        /// </summary>
        [TestCategory("Failing")]
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

    [Presenter]
    public class Test1Presenter : PresenterBase<ITest1View,object>, ITest1Presenter
    {
        public Test1Presenter(Test1View view, [MvpParam] object model = null, [Inject] IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }

        public override object Show()
        {
            throw new NotImplementedException();
        }
    }

    public interface ITest1View : IWindowView
    {
    }

    [View]
    public class Test1View : ITest1View
    {
        public event ViewVisibilityChangedDelegate ViewVisibilityChanged;

        public void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
//            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public void RefreshView()
        {
//            throw new NotImplementedException();
        }

        public void Show()
        {
//            throw new NotImplementedException();
        }

        public object ShowModal()
        {
            throw new NotImplementedException();
        }
    }

}
