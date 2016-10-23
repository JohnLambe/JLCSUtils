using DiExtension.AutoFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Di.AutoFactory
{
    public class ClassToBeCreatedByFactory
    {
        public ClassToBeCreatedByFactory(int param)
        {
            // ...
        }

        // ...
    }

    public class ClassThatNeedsFactory
    {
        public ClassThatNeedsFactory(IFactory<ClassToBeCreatedByFactory,int> factory)
        {
            TheFactory = factory;
        }

        public ClassToBeCreatedByFactory UseFactory(int param)
        {
            return TheFactory.Create(param);
        }

        public readonly IFactory<ClassToBeCreatedByFactory,int> TheFactory;
    }
}
