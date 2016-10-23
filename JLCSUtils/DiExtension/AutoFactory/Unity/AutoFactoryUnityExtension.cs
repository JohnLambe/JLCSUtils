using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

using DiExtension.AutoFactory;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
using DiExtension.Unity;

// Integration of AutoFactory with Microsoft Unity.

namespace DiExtension.AutoFactory.Unity
{
    /// <summary>
    /// Unity extension for AutoFactory.
    /// </summary>
    // Registers AutoFactoryBuilderStrategy.
    public class AutoFactoryUnityExtension : UnityContainerExtension
    {
        public AutoFactoryUnityExtension()
        {
        }

        protected override void Initialize()
        {
            var strategy = new AutoFactoryBuilderStrategy(Context);

            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
//            Context.Strategies.Add(strategy, UnityBuildStage.Initialization);
        }
    }


    public class AutoFactoryBuilderStrategy : BuilderStrategyBase
    {
        protected AutoFactoryFactory<string> _factoryFactory = new AutoFactoryFactory<string>();
         
        public AutoFactoryBuilderStrategy(ExtensionContext baseContext) : base(baseContext)
        {
            _factoryFactory.OnResolveAutoInterface += ResolveAutoInterface;
        }

        public override object CustomResolve(IBuilderContext context)
        {
            var key = (NamedTypeBuildKey)context.OriginalBuildKey;
            return _factoryFactory.ResolveEx(key.Type, key.Name);
        }

        public virtual object ResolveAutoInterface(Type interfaceType, string name)
        {
            var resolved = BaseContext.Container.Registrations.Where(r =>
                interfaceType.IsAssignableFrom(r.RegisteredType)   // compatible type
                && (name == null || name.Equals(r.Name))    // no name or name matches
                );
            switch( resolved.Count() )
            {
                case 0:     // none found - we can't resolve it, so let Unity continue as usual
                    return null;
                case 1:     // successfully resolved
                    var resolvedRegistration = resolved.First();
                    if (resolvedRegistration.LifetimeManager == null)   // mapped to type (not instance)
                    {
                        return resolvedRegistration.MappedToType;
                    }
                    else              // mapped to instance
                    {
                        var value = resolvedRegistration.LifetimeManager.GetValue();
                        if (value is Type)
                            return null;            // we don't support an instance of Type `Type` (because returning type implies a type mapping)
                        else
                            return value;
                    }
                default:        // ambiguous
                    throw new Exception("Failed to resolve type " + interfaceType + " for an Auto Factory because there are multiple mappings for it: "
                        + resolved.FormatCollection() );
//                    throw new ResolutionFailedException(interfaceType, name, null, null);
            }
        }
    }

}
