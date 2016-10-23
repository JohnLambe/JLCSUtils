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
using DiExtension;
using DiExtension.ConfigInject;

// Integration with Microsoft Unity.

namespace DiExtension.Unity
{
    /// <summary>
    /// </summary>
    // Registers the BuilderStrategy.
    public class DiUnityExtension : UnityContainerExtension
    {
        public DiUnityExtension(ExtendedDiContext diContext)
        {
            this._diContext = diContext;
        }

        protected override void Initialize()
        {
            var strategy = new DiBuilderStrategy(Context,_diContext);

            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
            //            Context.Strategies.Add(strategy, UnityBuildStage.Initialization);
        }

        protected ExtendedDiContext _diContext;
    }


    public class DiBuilderStrategy : BuilderStrategyBase
    {
        public DiBuilderStrategy(ExtensionContext baseContext, ExtendedDiContext diContext)
            : base(baseContext)
        {
            this._diContext = diContext;
        }

        public override object CustomResolve(IBuilderContext context)
        {
            var key = context.OriginalBuildKey;
            if (key.Name != null && key.Name.StartsWith(_diContext.ConfigNamePrefix))   // named string
            {
                object value;
                //                var name = key.Name.RemovePrefix(_diContext.ConfigNamePrefix);
                //                if (_diContext.ProviderChain.GetValue(name, key.Type, out value))
                if (_diContext.GetValue(key.Name, key.Type, out value))
                    return value;
            }
            return null;
        }

        protected ExtendedDiContext _diContext;
    }

}
