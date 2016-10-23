using System;

namespace DiExtension.AutoFactory
{
    // Classes that implement each of the IFactory&lt;TInterface,...&gt; interfaces:

    // No arguments to `Create`:
    public class AutoFactory<TInterface> : AutoFactoryBase<TInterface>, IFactory<TInterface>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create()
        {
            return CreateInstance();
        }
    }

    public class AutoFactory<TInterface, TParam1> : AutoFactoryBase<TInterface>, IFactory<TInterface, TParam1>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create(TParam1 param1)
        {
            return CreateInstance(param1);
        }
    }

    public class AutoFactory<TInterface, TParam1, TParam2> : AutoFactoryBase<TInterface>, IFactory<TInterface, TParam1, TParam2>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create(TParam1 param1, TParam2 param2)
        {
            return CreateInstance(param1, param2);
        }
    }

    public class AutoFactory<TInterface, TParam1, TParam2, TParam3> : AutoFactoryBase<TInterface>, IFactory<TInterface, TParam1, TParam2, TParam3>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateInstance(param1, param2, param3);
        }
    }

    public class AutoFactory<TInterface, TParam1, TParam2, TParam3, TParam4> : AutoFactoryBase<TInterface>, IFactory<TInterface, TParam1, TParam2, TParam3, TParam4>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateInstance(param1, param2, param3, param4);
        }
    }

    public class AutoFactory<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5> : AutoFactoryBase<TInterface>, IFactory<TInterface, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TInterface : class
    {
        public AutoFactory(Type targetClass, object targetInstance = null) : base(targetClass, targetInstance) { }

        public TInterface Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreateInstance(param1, param2, param3, param4, param5);
        }
    }
}
