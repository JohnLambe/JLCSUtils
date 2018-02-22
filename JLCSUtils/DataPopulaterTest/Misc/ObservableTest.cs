using JohnLambe.Util.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Misc
{
    [TestClass]
    public class ObservableTest
    {
        [TestMethod]
        public void FireValueChanged()
        {
            // Arrange:
            //            Observable<int> x = (Observable<int>)100;
            Observable<int> x = new Observable<int>(100);
            x.ValueChanged += X_ValueChanged;

            // Act:
            x.Value = 50;

            // Assert:
            Assert.AreEqual(100, _changedArgs.OldValue);
            Assert.AreEqual(50, _changedArgs.NewValue);

            Assert.AreEqual(x.Value, 50);
        }

        [TestMethod]
        public void FireValueChanging()
        {
            // Arrange:
            CancellableObservable<int> x = new CancellableObservable<int>(100);
            x.ValueChanged += X_ValueChanged;
            x.ValueChanging += X_ValueChanging;

            // Act:
            x.Value = 50;

            // Assert:
            Assert.AreEqual(100, _changingArgs.OldValue);
            Assert.AreEqual(51, _changingArgs.NewValue);

            Assert.AreEqual(100, _changedArgs.OldValue);
            Assert.AreEqual(51, _changedArgs.NewValue);

            Assert.AreEqual(x.Value, 51);
            Assert.AreEqual((int)x, 51);
        }

        [TestMethod]
        public void Cancel()
        {
            // Arrange:
            CancellableObservable<int> x = new CancellableObservable<int>(100);
            x.ValueChanged += X_ValueChanged;
            x.ValueChanging += X_ValueChanging;
            _cancel = true;

            // Act:
            x.Value = 50;

            // Assert:
            Assert.AreEqual(100, _changingArgs.OldValue);
            Assert.AreEqual(51, _changingArgs.NewValue);

            Assert.AreEqual(null, _changedArgs);   // ValueChanged not fired

            Assert.AreEqual((int)x, 100);
        }

        private void X_ValueChanging(object sender, ValueChangedEventArgs<int> e)
        {
            _changingArgs = e;
            e.NewValue++;
            e.Cancel = _cancel;
        }
        protected ValueChangedEventArgs<int> _changingArgs;
        protected bool _cancel = false;

        private void X_ValueChanged(object sender, ValueChangedEventArgs<int> e)
        {
            _changedArgs = e;
        }
        protected ValueChangedEventArgs<int> _changedArgs;
    }
}
