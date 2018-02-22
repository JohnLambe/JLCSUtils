using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension.Attributes;
using DiExtension;

namespace JohnLambe.Tests.JLUtilsTest.Di.AutoRegistration
{
    public interface IRegisteredInterface
    {

    }

    [DiRegisterType(ForType = typeof(IRegisteredInterface))]
    public class ToBeRegisteredByType : IRegisteredInterface
    {
    }

    [DiRegisterInstance(Name = "RegisteredByName")]
    public class ToHaveRegisteredInstance
    {
    }

}
