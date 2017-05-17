using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Dialog;
using MvpFramework.Dialog.Dialogs;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace MvpFrameworkTest.Dialog
{
    [TestClass]
    public class ExceptionToDialogMapTest
    {
        [TestMethod]
        public void GetDialogTypeForExceptionType()
        {
            // Arrange:
            var map = new ExceptionToDialogMap();
            map.AddMapping(typeof(System.IO.IOException), typeof(ConfirmationDialogType));
            map.AddMapping(typeof(System.Exception), typeof(ErrorDialogType));

            // Act / Assert:

            Multiple(
                () => Assert.AreEqual(typeof(ErrorDialogType), map.GetDialogTypeForExceptionType(typeof(Exception))),
                () => Assert.AreEqual(typeof(ErrorDialogType), map.GetDialogTypeForExceptionType(typeof(System.NullReferenceException))),

                () => Assert.AreEqual(typeof(ConfirmationDialogType), map.GetDialogTypeForExceptionType(typeof(System.IO.IOException))),
                () => Assert.AreEqual(typeof(ConfirmationDialogType), map.GetDialogTypeForExceptionType(typeof(System.IO.FileNotFoundException)))
            );
        }

        [TestMethod]
        public void GetDialogTypeForExceptionType_ByDialogModel()
        {
            // Arrange:
            var map = new ExceptionToDialogMap();
            map.AddMapping(typeof(System.IO.IOException), typeof(ConfirmationDialog));
            map.AddMapping(typeof(System.Exception), typeof(ErrorDialog));

            // Act / Assert:

            Multiple(
                () => Assert.AreEqual(typeof(ErrorDialog), map.GetDialogTypeForExceptionType(typeof(Exception))),
                () => Assert.AreEqual(typeof(ErrorDialog), map.GetDialogTypeForExceptionType(typeof(System.NullReferenceException))),

                () => Assert.AreEqual(typeof(ConfirmationDialog), map.GetDialogTypeForExceptionType(typeof(System.IO.IOException))),
                () => Assert.AreEqual(typeof(ConfirmationDialog), map.GetDialogTypeForExceptionType(typeof(System.IO.FileNotFoundException)))
            );
        }

        [TestMethod]
        public void GetDialogModelTypeForDialogType()
        {
            // Arrange:
            var map = new ExceptionToDialogMap();

            // Act / Assert:

            Multiple(
                () => Assert.AreEqual(typeof(SystemErrorDialog), map.GetDialogModelTypeForDialogType(typeof(SystemErrorDialogType))),
                () => Assert.AreEqual(typeof(InternalErrorDialog), map.GetDialogModelTypeForDialogType(typeof(InternalErrorDialog)))

            );
        }

        public ExceptionToDialogMap Setup()
        {
            var map = new ExceptionToDialogMap();
            map.AddMapping(typeof(System.IO.IOException), typeof(ConfirmationDialogType));
            map.AddMapping(typeof(System.Exception), typeof(ErrorDialogType));
            map.AddMapping(typeof(System.SystemException), typeof(InternalErrorDialogType));

            return map;
        }

        [TestMethod]
        public void GetMessageDialogModelForException_Subclass()
        {
            // Arrange:
            var map = Setup();

            Exception ex = null;
            try
            {
                string s = null;
                Console.WriteLine(s.Length);   // generate a NullReferenceException
            }
            catch(NullReferenceException ex1)   // subclass of SystemException
            {
                ex = ex1;
            }

            // Act:

            var model = map.GetMessageDialogModelForException(ex);

            // Assert:

            Assert.AreEqual(typeof(InternalErrorDialog), model.GetType());
            AssertContains(ex.Message, model.Message);
        }

        [TestMethod]
        public void GetMessageDialogModelForException()
        {
            // Arrange:
            var map = Setup();

            Exception ex = new Exception("Test");

            // Act:

            var model = map.GetMessageDialogModelForException(ex);

            // Assert:

            Assert.AreEqual(typeof(ErrorDialog), model.GetType());
            Assert.AreEqual(ex.Message, model.Message);
        }

    }
}
