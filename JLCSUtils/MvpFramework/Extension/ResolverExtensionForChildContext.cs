using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension;

namespace MvpFramework.Extension
{
    /// <summary>
    /// Resolver extension that uses child (nested) contexts from the DI system.
    /// </summary>
    public class ResolverExtensionForChildContext : ResolverExtensionBase
    {
        public override ResolverExtensionStatus StartInjection(ResolverExtensionContext context)
        {
            var currentDiResolver = context.DiResolver;
            if (currentDiResolver is IChainableDiResolver)
            {
                var childContext = GetUseChildContext(context);
                if (childContext)
                    currentDiResolver = ((IChainableDiResolver)currentDiResolver).CreateChildContext();
            }
            context.DiResolver = currentDiResolver;

            return ResolverExtensionStatus.Default;
        }
    }
}
