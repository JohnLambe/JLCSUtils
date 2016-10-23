using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// A dependency injection context that (optionally) delegates to a parent context for anything that it cannot resolve.
    /// </summary>
    public class ChainableContext : SiExtendedDiContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentContext">Parent context, or null if this is the root.
        /// If this is not a <see cref="ChainableContext"/> and doesn't implement chaining behaviour itself,
        /// it is the root.</param>
        public ChainableContext(SiExtendedDiContext parentContext = null)
        {
            this._parentContext = parentContext;
            if(parentContext != null)
                Container.ResolveUnregisteredType += Container_ResolveUnregisteredType;
        }

        protected virtual void Container_ResolveUnregisteredType(object sender, SimpleInjector.UnregisteredTypeEventArgs e)
        {
            //            _parentContext.GetInstance<object>(e.UnregisteredServiceType);
            var instanceProducer = _parentContext.Container.GetRegistration(e.UnregisteredServiceType);
            e.Register(instanceProducer.Registration);
        }

        /// <summary>
        /// Create a child context of this one.
        /// </summary>
        /// <returns></returns>
        public virtual ChainableContext CreateChildContext()
        {
            return new ChainableContext(this);
        }

        protected readonly SiExtendedDiContext _parentContext;
    }
}
