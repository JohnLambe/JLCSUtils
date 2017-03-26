
namespace DiExtension.AutoFactory
{
    // Interfaces for factories with a 'Create' method with up to 5 parameters:
    // The first generic type parameter is the declared type returned by the factory (typically an interface or base class).
    // The rest of them are the parameters to the factory's Create method, in the order in which they appear in that method.

    //| These are used rather than Create<T>(params T[]), for type safety
    //| (so that it can be identified earlier if the types don't match the actual contructor).

    /// <summary>
    /// Interface for a factory class, supported by <see cref="AutoFactoryFactory"/>.
    /// </summary>
    /// <typeparam name="TInterface">The type returned by the factory (usually an interface type).</typeparam>
    [AutoFactory]
    public interface IFactory<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create();
    }

    /// <summary>
    /// Interface for a factory class.
    /// </summary>
    /// <typeparam name="TInterface">The type returned by the factory (usually an interface type).</typeparam>
    /// <typeparam name="TParam">The type of the first parameter to the <see cref="Create(TParam)"/> method.</typeparam>
    [AutoFactory]
    public interface IFactory<TInterface, TParam>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create(TParam param);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create(TParam1 param1, TParam2 param2);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3, TParam4>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TInterface : class
    {
        /// <summary>
        /// Create an instance.
        /// Each factory interface can have its own semantic rules.
        /// Some may allow returning null.
        /// Some may require that a new instance is returned (rather than returning an existing instance, maybe cached or multiton).
        /// </summary>
        /// <returns>An instance of the type returned by the factory, or null.</returns>
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }
}
