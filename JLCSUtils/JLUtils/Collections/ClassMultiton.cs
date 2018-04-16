using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Interfaces;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// A collection of instances, each of a different class, with a unique ID.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ClassMultiton<TKey,TValue>
        where TValue : class
    {
        public virtual TValue Get(TKey key)
        {
            return _values[key];
        }

        public abstract TKey GetKey(TValue value);

        public void Scan<TAttribute>(params Assembly[] assemblies)
        {
            foreach(var type in assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(TValue).IsAssignableFrom(t) && t.IsClass && t.IsDefined(typeof(TAttribute),false)))
            {
                var instance = ReflectionUtil.Create<TValue>(type);
                // ensure that keys are unique

                _values.Add(GetKey(instance), instance);
            }
        }

        protected IDictionary<TKey,TValue> _values = new Dictionary<TKey,TValue>();

        /// <summary>
        /// Enumerator over the keys and values.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerator() => _values.AsEnumerable();
    }

    public class IdClassMultiton<TKey, TValue> : ClassMultiton<TKey,TValue>
        where TValue : class, IHasId<TKey>
    {
        public override TKey GetKey(TValue value) => value.Id;
    }


    public abstract class ClassMultitonMember<TKey, TValue> : IHasId<TKey>
        where TValue : class, IHasId<TKey>
    {
        public abstract TKey Id { get; }

        public static implicit operator TKey(ClassMultitonMember<TKey,TValue> value)
            => value.Id;

        public static readonly ClassMultiton<TKey, TValue> Instance = new IdClassMultiton<TKey, TValue>();
    }

}
