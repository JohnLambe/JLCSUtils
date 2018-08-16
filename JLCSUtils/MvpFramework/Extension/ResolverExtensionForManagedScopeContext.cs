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
    public abstract class ResolverExtensionForManagedScopeContext : ResolverExtensionBase
    {
        /// <summary>
        /// </summary>
        /// <param name="scopeManager">
        /// The scope manager for the lifestyle used for presenters.
        /// null if this is not used.
        /// </param>
        public ResolverExtensionForManagedScopeContext([Nullable] ManagedScopeLifestyleManager scopeManager = null)
        {
            this.ScopeManager = scopeManager;
        }

        public override ResolverExtensionStatus PresenterFactoryCreated(ResolverExtensionPresenterFactoryContext context)
        {
            context.SetData(DiScopeKey, ScopeManager.CurrentScope);  // Keep a reference to the Scope of the factory
            
            return base.PresenterFactoryCreated(context);
        }

        public override ResolverExtensionStatus StartInjection(ResolverExtensionContext context)
        {
            if (context.UseChildContext)
            {
                ScopeManager?.StartScope();
            }
            else
            {
                ScopeManager?.StartScope(context.FactoryExtensionContext.GetData<DiScope>(DiScopeKey));
            }

            return ResolverExtensionStatus.Default;
        }

        public override ResolverExtensionStatus EndInjection(ResolverExtensionContext context)
        {
            //if (context.UseChildContext)
            ScopeManager?.EndScope();

            return ResolverExtensionStatus.Default;
        }

        protected ManagedScopeLifestyleManager ScopeManager { get; }

        /// <summary>
        /// Key in <see cref="ResolverExtensionPresenterFactoryContext.ExtensionData"/>.
        /// </summary>
        protected const string DiScopeKey = "DiScope";
    }
}
