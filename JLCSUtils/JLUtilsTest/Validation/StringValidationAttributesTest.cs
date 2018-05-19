using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class PhoneNumberValidationAttributeTest
    {
        [TestMethod]
        public void ValidatePhoneNumber()
        {
            TestUtil.Multiple(
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353-1-4631234", true)),
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353-1-4631234", false)),
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("00234234234")),
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353  -  1 -  4631234", true)),
                () => Assert.IsFalse(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353  -  1 -  4631234", false)),
                () => Assert.IsFalse(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353 (0) -  1 -  4631234", false)),
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353 ( 0 ) -  1 -  4631234", true)),
                () => Assert.IsTrue(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353 ( 0 ) -  1 -  4631234 ", true)),
                () => Assert.IsFalse(PhoneNumberValidationAttribute.ValidatePhoneNumber("+353-1-4631234a", true))
            );
        }
    }
}
