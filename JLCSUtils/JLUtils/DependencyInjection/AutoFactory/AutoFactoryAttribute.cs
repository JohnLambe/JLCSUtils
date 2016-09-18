using System;

namespace JohnLambe.Util.DependencyInjection.AutoFactory
{
    /// <summary>
    /// Flags as interface for use with AutoFactory.
    /// To be used on interfaces for which AutoFactoryFactory can create implementations.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Interface,
        Inherited=false,    // because the AutoFactory base classes would not implement any derived interface
        AllowMultiple=false
        )]
    class AutoFactoryAttribute : Attribute
    {
        //| We could add a property to reference the factory class that implements the attributed interface,
        //| thereby allowing consumers of the library to add their own interfaces.
        // Type FactoryClassType { get; set; }
    }

}
