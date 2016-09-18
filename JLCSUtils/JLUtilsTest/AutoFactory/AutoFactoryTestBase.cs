using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.DependencyInjection.AutoFactory;

namespace JohnLambe.Tests.JLUtilsTest.AutoFactory
{
    /// <summary>
    /// Common setup for tests of AutoFactory.
    /// </summary>
    public class AutoFactoryTestBase
    {
        protected AutoFactoryFactory factoryFactory = new AutoFactoryFactory();
    }
}
