using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;
using SimpleInjector;


namespace DiExtension.SimpleInject
{
    ////**** TO BE REFACTORED: This is copied-and-modified from ExplicitScopeLifestyleManager

    /// <summary>
    /// Lifestyle for SimpleInjector with scopes started and ended by explicit calls.
    /// </summary>
    public class ManagedScopeLifestyleManager
    {
        public ManagedScopeLifestyleManager()
        {
            StartScope();  // create the root scope
        }

        /// <summary>
        /// Start a new scope.
        /// Anything resolved between this call and the next call to <see cref="EndScope"/> is resolved in the this scope.
        /// Each calls to this must be matched by a call to <see cref="EndScope"/>.
        /// </summary>
        /// <param name="scope">null to create a new scope. Otherwise, the scope to be used.</param>
        /// <returns>The scope now being used. If <paramref name="scope"/> is not null, it is returned, otherwise, the new scope is returned.</returns>
        public virtual DiScope StartScope([Nullable] DiScope scope = null)
        {
            scope = scope ?? new DiScope();
            Scopes.Push(scope);
            CurrentScope = scope;
            return scope;
        }

        /// <summary>
        /// End the current scope (returning to the previous one).
        /// </summary>
        public virtual void EndScope()
        {
            if (Scopes.Count <= 1)       // if in the root scope
                throw new InvalidOperationException(nameof(ExplicitScopeLifestyleManager) + ": No open scope");   // throw exception and don't close it
            Scopes.Pop();
            CurrentScope = Scopes.Peek();
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
                        CurrentScope.Resolved.TryGetValue(key, out instance);
                        instanceScope = CurrentScope;
                    }
                    if (instance == null)
                    {
                        instance = instanceCreator.Invoke();
                        instanceScope = CurrentScope;
                        CurrentScope.Resolved.Add(key, instance);
                    }
                    return instance;
                }
            };
        }
        //| We could put the delegate returned above in a separate virtual method, to facilitate overriding.

        /// <summary>
        /// Stack of scopes.
        /// Added and removed by <see cref="StartScope"/> and <see cref="EndScope"/>.
        /// There must always be at least one.
        /// <para>
        /// The dictionary in each scope holds all instances resolved in the scope. The key of the dictionary is the `key` variable in the <see cref="Invoke(Func{object})"/> method.
        /// </para>
        /// </summary>
        protected Stack<DiScope> Scopes { get; } = new Stack<DiScope>();

        public virtual DiScope CurrentScope { get; protected set; }
    }


    /// <summary>
    /// Lifestyle for SimpleInjector with scopes started and ended by explicit calls,
    /// with .
    /// </summary>
    public class SiManagedScopeLifestyleManager : ManagedScopeLifestyleManager
    {
        /// <summary>
        /// Create this and the <see cref="SimepleInjector.Lifestyle"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        public SiManagedScopeLifestyleManager(Container container, string name = null)
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
        public SiManagedScopeLifestyleManager()
        {
        }

        /// <summary>
        /// Casts to the <see cref="SimpleInjector.Lifestyle"/>.
        /// </summary>
        /// <param name="manager"></param>
        public static implicit operator Lifestyle(SiManagedScopeLifestyleManager manager)
        {
            return manager.Lifestyle;
        }

        /// <summary>
        /// The SimpleInjector Lifestyle (used for registrations).
        /// </summary>
        public virtual Lifestyle Lifestyle { get; }

        protected virtual Container Container { get; }
    }


    /// <summary>
    /// A scope in which items can be resolved in a <see cref="ManagedScopeLifestyleManager"/>.
    /// </summary>
    public class DiScope
    {
        public Dictionary<object, object> Resolved { get; } = new Dictionary<object, object>();
    }
}
