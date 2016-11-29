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
            Assert.AreEqual(false, pass.TestPassword("asdfgH"));
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
            pass.ChangePassword(password, newPassword);

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
            pass.ChangePassword(" " + password, newPassword);

            // Assert:
            Assert.IsFalse(pass.TestPassword(newPassword));
            Assert.IsTrue(pass.TestPassword(password));
        }
    }
}
