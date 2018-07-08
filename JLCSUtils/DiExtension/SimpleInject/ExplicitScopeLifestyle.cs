using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;


namespace DiExtension.SimpleInject
{
    /// <summary>
    /// Lifestyle for SimpleInjector with scopes started and ended by explicit calls.
    /// </summary>
    public class ExplicitScopeLifestyleManager
    {
        public ExplicitScopeLifestyleManager()
        {
            StartScope();  // create the root scope
        }

        /// <summary>
        /// Start a new scope.
        /// Each calls to this must be matched by a call to <see cref="EndScope"/>.
        /// </summary>
        public virtual void StartScope()
        {
            Scopes.Push(new Dictionary<object, object>());
        }

        /// <summary>
        /// End the current scope (returning to the previous one).
        /// </summary>
        public virtual void EndScope()
        {
            if (Scopes.Count <= 1)       // if in the root scope
                throw new InvalidOperationException(nameof(ExplicitScopeLifestyleManager) + ": No open scope");   // throw exception and don't close it
            Scopes.Pop();
        }

        public virtual Func<object> Invoke(Func<object> instanceCreator)
        {
            object instance = null;   // the last resolved instance (cached)
            object instanceScope = null;  // the scope that this instance was resolved from
            object key = new object();  // key of this registration in the Scope dictionary. Also used for synchronization.

            return () =>
            {
                lock (key)
                {
                    if (CurrentScope != instanceScope)  // if the scope has changed
                    {   // get the instance from the current scope (if there is one):
                        CurrentScope.TryGetValue(key, out instance);
                        instanceScope = CurrentScope;
                    }
                    if (instance == null)
                    {
                        instance = instanceCreator.Invoke();
                        instanceScope = CurrentScope;
                        CurrentScope.Add(key, instance);
                    }
                    return instance;
                }
            };
        }

        /// <summary>
        /// Stack of scopes.
        /// Added and removed by <see cref="StartScope"/> and <see cref="EndScope"/>.
        /// There must always be at least one.
        /// <para>
        /// The dictionary in each scope holds all instances resolved in the scope. The key of the dictionary is the `key` variable in the <see cref="Invoke(Func{object})"/> method.
        /// </para>
        /// </summary>
        protected Stack<Dictionary<object, object>> Scopes { get; } = new Stack<Dictionary<object, object>>();

        protected Dictionary<object, object> CurrentScope => Scopes.Peek();
    }

    /// <summary>
    /// Lifestyle for SimpleInjector with scopes started and ended by explicit calls,
    /// with .
    /// </summary>
    public class SiExplicitScopeLifestyleManager : ExplicitScopeLifestyleManager
    {
        /// <summary>
        /// Create this and the <see cref="SimepleInjector.Lifestyle"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        public SiExplicitScopeLifestyleManager(Container container, string name = null)
        {
            this.Container = container;

            Lifestyle = Lifestyle.CreateCustom(
                name: name ?? "CustomScopedLifestyle",
                lifestyleApplierFactory: instanceCreator =>
                {
                    return Invoke(instanceCreator);
                });
        }

        /// <summary>
        /// Create an instance without a <see cref="SimepleInjector.Lifestyle"/>.
        /// </summary>
        public SiExplicitScopeLifestyleManager()
        {
        }

        /// <summary>
        /// Casts to the <see cref="SimpleInjector.Lifestyle"/>.
        /// </summary>
        /// <param name="manager"></param>
        public static implicit operator Lifestyle(SiExplicitScopeLifestyleManager manager)
        {
            return manager.Lifestyle;
        }

        /// <summary>
        /// The SimpleInjector Lifestyle (used for registrations).
        /// </summary>
        public virtual Lifestyle Lifestyle { get; }

        protected virtual Container Container { get; }
    }
}
