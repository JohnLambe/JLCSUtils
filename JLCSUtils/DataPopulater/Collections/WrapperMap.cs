using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace JohnLambe.Util.Collections
{
    public class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public virtual void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public virtual bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public virtual int Count
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// Wraps an object to provide a map interface.
    /// </summary>
    public class WrapperMap : DictionaryBase<string,object>, IDictionary<string, object>, ILookup<string, object>
    {
        public WrapperMap(object data)
        {
            _data = data;
        }

        protected virtual PropertyInfo GetProperty(string key)
        {
            var property = GetType().GetProperty(key);
            if (property == null)
                throw new KeyNotFoundException("Property \"" + key + "\" does not exist");
            return property;
        }

        #region IDictionary methods

        public override void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(string key)
        {
            var property = GetType().GetProperty(key);
            return property != null;
        }

        public override ICollection<string> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public override ICollection<object> Values
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[string key]
        {
            get
            {
                return GetProperty(key).GetValue(_data, null);
            }
            set
            {
                GetProperty(key).SetValue(_data, value, null);
            }
        }

        public override void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public override void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override int Count
        {
            get
            {
                return GetType().GetProperties().Count();
            }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// The object wrapped by this.
        /// </summary>
        protected object _data;

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        bool ILookup<string, object>.Contains(string key)
        {
            throw new NotImplementedException();
        }

        int ILookup<string, object>.Count
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerable<object> ILookup<string, object>.this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator<IGrouping<string, object>> IEnumerable<IGrouping<string, object>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


}
