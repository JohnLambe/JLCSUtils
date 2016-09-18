using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;


namespace JohnLambe.Util.DependencyInjection.Unity
{
    /// <summary>
    /// Base class for Unity BuilderStrategy.
    /// Provides the CustomResolve for a simpler interface for providing a value when Unity doesn't have one.
    /// </summary>
    public class BuilderStrategyBase : BuilderStrategy
    {
        protected ExtensionContext BaseContext { get; private set; }

        public BuilderStrategyBase(ExtensionContext baseContext)
        {
            this.BaseContext = baseContext;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            Console.WriteLine("PreBuildUp: "
                + (context.Existing == null ? "<null>" : context.Existing.ToString())
                + "; Key:" + context.OriginalBuildKey);

            if (context.Existing == null)
            {
                var resolved = CustomResolve(context);
                if (resolved != null)
                {
                    context.Existing = resolved;

                    context.BuildComplete = true;
                }
            }
        }

        /// <summary>
        /// Called when Unity does not have a value to inject.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The value to inject, or null to not change the default behaviour.</returns>
        public virtual object CustomResolve(IBuilderContext context)
        {
            return null;
        }
    }
}
