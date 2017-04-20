using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework.Binding;
using DiExtension.Attributes;
using MvpFramework.WinForms;

namespace MvpFrameworkTest.Binding.MvpEvent
{
    [TestClass]
    public class MvpEventAttributeTest
    {
    }


    public interface ITestView : IView
    {
        [MvpEvent("TestEventId")]
        event EventHandler TestEvent;
    }

    public class TestView : ViewBase, ITestView
    {
        public event EventHandler TestEvent;

        public void FireEvent()
        {
            TestEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TestPresenter : PresenterBase<ITestView, object>
    {
        public TestPresenter(ITestView view, object model = null, [Inject(null)] IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }

        [MvpHandler("TestEventId")]  // Mapped to ITestView.TestEvent
        public void Handler()
        {
            Called = true;
        }

        public bool Called { get; protected set; }
    }

}
