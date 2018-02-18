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
    public class ChainableContext : SiExtendedDiContext, IChainableDiResolver
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
            var instanceProducer = _parentContext.Container.GetRegistration(e.UnregisteredServiceType);  // try to get registration from parent
            if(instanceProducer != null)                         // if successful
                e.Register(instanceProducer.Registration);       // register with this context
        }

        #region Child Context

        /// <summary>
        /// <inheritdoc cref="IChainableDiResolver.CreateChildContext"/>
        /// </summary>
        /// <returns></returns>
        public virtual ChainableContext CreateChildContext()
        {
            var newContext = new ChainableContext(this);
            SetupChildContext(newContext);
            return newContext;
        }

        /// <inheritdoc cref="IChainableDiResolver.CreateChildContext"/>
        IDiResolver IChainableDiResolver.CreateChildContext()
        {
            return CreateChildContext();
        }

        /// <summary>
        /// Called on creating a child context, to register items with it.
        /// (Called just after creating it, before returning it from <see cref="CreateChildContext"/>.)
        /// <para>Fires <see cref="OnSetupChildContext"/>.</para>
        /// </summary>
        /// <param name="context">the new (child) context.</param>
        protected virtual void SetupChildContext(ChainableContext context)
        {
            OnSetupChildContext?.Invoke(this, new SetupContextEventArgs(context));
        }

        /// <summary>
        /// Arguments to <see cref="OnSetupChildContext"/>:
        /// To provide handlers with the ability to register items in a DI context.
        /// </summary>
        public class SetupContextEventArgs : EventArgs
        {
            public SetupContextEventArgs(SiExtendedDiContext context)
            {
                this.Context = context;
            }

            /// <summary>
            /// The context to be set up.
            /// </summary>
            public SiExtendedDiContext Context { get; }
            //| TODO: Change to IDiTypeRegistrar and make this class non-implementation-specific ?
        }

        /// <summary>
        /// Fired when a child context of this context is created.
        /// The context passed in the event arguments is the new (child) context.
        /// </summary>
        public event EventHandler<SetupContextEventArgs> OnSetupChildContext;

        #endregion

        public override bool GetValue<T>(string key, Type type, out T value)
        {
            if (base.GetValue<T>(key, type, out value))
                return true;
            else
                return _parentContext.GetValue<T>(key, type, out value);
        }

        protected readonly SiExtendedDiContext _parentContext;
    }
}
