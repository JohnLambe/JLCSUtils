using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Security;

namespace JohnLambe.Tests.JLUtilsTest.Security
{
    [TestClass]
    public class HashedPasswordTest
    {
        [TestMethod]
        public void TestPassword()
        {
            const string password = "asdfgh";

            var pass = new HashedPassword();
            pass.Value = password;

            // Assert:
            Assert.AreEqual(true, pass.TestPassword(password));
            Assert.AreEqual(false, pass.TestPassword("asdfgH"));  // one character different by letter case

            Assert.IsFalse(pass.TestPassword(null));
            Assert.IsFalse(pass.TestPassword(""));
        }

        /// <summary>
        /// Test that copying the Encoded Value copies the password.
        /// </summary>
        [TestMethod]
        public void EncodedValue()
        {
            const string password = "Password";

            var pass = new HashedPassword();
            pass.Value = password;

            Console.WriteLine(pass.EncodedValue);

            var pass2 = new HashedPassword();
            pass2.EncodedValue = pass.EncodedValue;

            Console.WriteLine(pass.EncodedValue);

            // Assert:
            Assert.AreEqual(true, pass2.TestPassword(password));
            Assert.AreEqual(pass.EncodedValue, pass2.EncodedValue);
        }

        /// <summary>
        /// Check that assigning the same password to two instances results in a different hash (due to a different salt being generated).
        /// </summary>
        [TestMethod]
        public void DifferentSalt()
        {
            const string password = "asdfgh";

            var pass = new HashedPassword();
            pass.Value = password;

            var pass3 = new HashedPassword();
            pass3.Value = password;

            // Assert:
            Assert.AreNotEqual(pass.EncodedValue, pass3.EncodedValue, "Same salt");
        }

        [TestMethod]
        public void ChangePassword()
        {
            const string password = "asdfgh";
            const string newPassword = "SDF344";

            var pass = new HashedPassword();
            pass.Value = password;

            // Act:
            Assert.IsTrue(pass.ChangePassword(password, newPassword));

            // Assert:
            Assert.IsTrue(pass.TestPassword(newPassword));
            Assert.IsFalse(pass.TestPassword(password));
        }

        /// <summary>
        /// Test ChangePassword with a wrong value for the existing password.
        /// </summary>
        [TestMethod]
        public void ChangePassword_Wrong()
        {
            const string password = "asdfgh";
            const string newPassword = "SDF344";

            var pass = new HashedPassword();
            pass.Value = password;

            // Act:
            Assert.IsFalse(pass.ChangePassword(" " + password, newPassword));   // fails to set password

            // Assert:
            Assert.IsFalse(pass.TestPassword(newPassword));
            Assert.IsTrue(pass.TestPassword(password));            // still has the old password
        }

        /// <summary>
        /// Test that it is initialised to a blank password, and setting <see cref="HashedPassword.EncodedValue"/> to null sets a blank password.
        /// </summary>
        [TestMethod]
        public void EncodedValue_Null()
        {
            var pass = new HashedPassword();

            Assert.IsTrue(pass.TestPassword(null));
            Assert.IsTrue(pass.TestPassword(""));

            pass.EncodedValue = null;
            Assert.IsTrue(pass.TestPassword(null));
            Assert.IsTrue(pass.TestPassword(""));
        }

        /// <summary>
        /// Test that setting <see cref="HashedPassword.Value"/> to null sets a blank password.
        /// </summary>
        [TestMethod]
        public void Value_Null()
        {
            // Arrange:
            var pass = new HashedPassword();
            pass.Value = null;

            // Act/Assert:
            Assert.IsTrue(pass.TestPassword(null));
            Assert.IsTrue(pass.TestPassword(""));
        }

    }
}
