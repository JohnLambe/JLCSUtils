
namespace DiExtension.AutoFactory
{
    // Interfaces for factories with a 'Create' method with up to 5 parameters:

    //| These are used rather than Create<T>(params T[]), for type safety
    //| (so that it can be identified earlier if the types don't match the actual contructor).

    /// <summary>
    /// Interface for a factory class, supported by <see cref="AutoFactoryFactory"/>.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    [AutoFactory]
    public interface IFactory<TInterface>
        where TInterface : class
    {
        TInterface Create();
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam>
        where TInterface : class
    {
        TInterface Create(TParam param);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2>
        where TInterface : class
    {
        TInterface Create(TParam1 param1, TParam2 param2);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3>
        where TInterface : class
    {
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3, TParam4>
        where TInterface : class
    {
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    [AutoFactory]
    public interface IFactory<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TInterface : class
    {
        TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }
}
