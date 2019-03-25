using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Bound Property where the object bound to (target) is provided by a delegate.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object on which the property is defined.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public class DelegateBoundProperty<TTarget, TProperty> : BoundProperty<TTarget, TProperty>
    {
        public DelegateBoundProperty([NotNull] Func<TTarget> targetDelegate, PropertyInfo property) : base(property)
        {
            ObjectExtension.CheckArgNotNull(targetDelegate, nameof(targetDelegate));
            property.ArgNotNull(nameof(property));
            //Diagnostics.PreCondition(property.DeclaringType.IsAssignableFrom(target.GetType()));
            TargetDelegate = targetDelegate;
        }

        /// <summary>
        /// Delegate to return <see cref="Target"/>.
        /// </summary>
        protected Func<TTarget> TargetDelegate { get; }

        /// <summary>
        /// The bound instance.
        /// </summary>
        public override TTarget Target
        {
            get
            {
                return TargetDelegate.Invoke();
            }
        }
    }
}
