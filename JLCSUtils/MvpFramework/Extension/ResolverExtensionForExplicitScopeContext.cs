using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.SimpleInject;
using JohnLambe.Util.Types;

namespace MvpFramework.Extension
{
    /// <summary>
    /// Resolver extension that uses an <see cref="ExplicitScopeLifestyleManager"/> when a child context is needed.
    /// </summary>
    public abstract class ResolverExtensionForExplicitScopeContext : ResolverExtensionBase
    {
        /// <summary>
        /// </summary>
        /// <param name="scopeManager">
        /// The scope manager for the lifestyle used for presenters.
        /// null if this is not used.
        /// </param>
        public ResolverExtensionForExplicitScopeContext([Nullable] ExplicitScopeLifestyleManager scopeManager = null)
        {
            this.ScopeManager = scopeManager;
        }

        public override ResolverExtensionStatus StartInjection(ResolverExtensionContext context)
        {
            if (context.UseChildContext)
            {
                ScopeManager?.StartScope();
            }

            return ResolverExtensionStatus.Default;
        }

        public override ResolverExtensionStatus EndInjection(ResolverExtensionContext context)
        {
            if (context.UseChildContext)
            {
                ScopeManager?.EndScope();
            }

            return ResolverExtensionStatus.Default;
        }

        protected ExplicitScopeLifestyleManager ScopeManager { get; }
    }
}
