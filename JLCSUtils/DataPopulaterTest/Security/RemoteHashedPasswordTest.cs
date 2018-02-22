using JohnLambe.Util.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Security
{
    [TestClass]
    public class RemoteHashedPasswordTest
    {
        public RemoteHashedPasswordTest()
        {
            // this would be done on the server:
            _serverPass.Value = Password;

            // _serverPass.HashMetadata would be transmitted to the client.

            // this would be done on the client:
            _client.HashMetadata = _serverPass.HashMetadata;
        }

        const string Password = "password";
        const string NewPassword = "newPassword";

        protected RemoteHashedPassword _serverPass = new RemoteHashedPassword();
        protected RemoteHashedPasswordClient _client = new RemoteHashedPasswordClient();

        [TestMethod]
        public void TestPassword_Correct()
        {
            Console.WriteLine("Password encoding on server: " + _serverPass.EncodedValue);

            // this would be done on the client:
            var testHash = _client.GetTestPasswordHashString(Password);
            Console.WriteLine("Client's login hash: " + testHash);

            // testHash would be provided to the server.

            // this would be done on the server:
            Assert.IsTrue(_serverPass.TestHash(testHash, true));
        }

        [TestMethod]
        public void TestPassword_Wrong()
        {
            // this would be done on the client:
            var testHash = _client.GetTestPasswordHashString(Password + "a");

            // testHash would be provided to the server.

            // this would be done on the server:
            Assert.IsFalse(_serverPass.TestHash(testHash, true));
        }

        /*
        [TestMethod]
        public void TestPassword_Correct_Binary()
        {
            // this would be done on the client:
            var testHash = _client.GetTestPasswordHash(Password);

            // testHash would be provided to the server.

            // this would be done on the server:
            Assert.IsTrue(_serverPass.TestPassword(testHash, true));
        }
        */

        [TestMethod]
        public void ChangePassword_Success()
        {
            // Act:

            // this would be done on the client:
            var testHash = _client.GetTestPasswordHashString(Password);
            var newPassword = _client.GetNewPasswordString(NewPassword);

            // testHash and newPassword would be provided to the server.

            // this would be done on the server:
            Assert.IsTrue(_serverPass.ChangePasswordByHash(testHash,newPassword, true));

            // Assert:
            Assert.IsFalse(_serverPass.TestPassword(Password), "Old password still worked");
            Assert.IsTrue(_serverPass.TestPassword(NewPassword), "Password has not been set to the new value");   // check that it was changed
        }

        [TestMethod]
        public void ChangePassword_Wrong()
        {
            // Act:

            // this would be done on the client:
            var testHash = _client.GetTestPasswordHashString(Password + ".");
            var newPassword = _client.GetNewPasswordString(NewPassword);

            // testHash and newPassword would be provided to the server.

            // this would be done on the server:
            Assert.IsFalse(_serverPass.ChangePasswordByHash(testHash, newPassword, true));

            // Assert:
            Assert.IsTrue(_serverPass.TestPassword(Password), "Old password should still work");
            Assert.IsFalse(_serverPass.TestPassword(NewPassword), "Password has been set to the new value");   // check that it was not changed
        }

        [TestMethod]
        [TestCategory("Failing")]  // requires unimplemented method
        public void ChangePassword_Success_Binary()
        {
            // Act:

            // this would be done on the client:
            var testHash = _client.GetTestPasswordHash(Password);
            var newPassword = _client.GetNewPassword(NewPassword);

            // testHash and newPassword would be provided to the server.

            // this would be done on the server:
            Assert.IsTrue(_serverPass.ChangePasswordByHash(testHash, newPassword));

            // Assert:
            Assert.IsFalse(_serverPass.TestPassword(Password), "Old password still worked");
            Assert.IsTrue(_serverPass.TestPassword(NewPassword), "Password has not been set to the new value");   // check that it was changed
        }

    }
}