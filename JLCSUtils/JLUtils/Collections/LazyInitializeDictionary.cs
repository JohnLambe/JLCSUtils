using System;
using System.Collections.Generic;

namespace JohnLambe.Util.Collections
{
    
    public class LazyInitializeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public delegate TValue LookupDelegate(TKey key);

        public LazyInitializeDictionary(LookupDelegate lookupDelegate)
        {
            _createDelegate = lookupDelegate;
        }

        protected LookupDelegate _createDelegate;

        public override bool TryGetValue(TKey key, out TValue value)
        {
            bool result = base.TryGetValue(key, out value);
            if(result)
            {
                return true;
            }
            else  // not already in Dictionary
            {
                return TryCreate(key, out value);
            }
        }

        public override bool ContainsKey(TKey key)
        {
            if (base.ContainsKey(key))
            {
                return true;
            }
            else
            {
                TValue value;
                return TryCreate(key, out value);
            }
        }

        public override TValue this[TKey key]
        {
            get
            {
                try
                {
                    return base[key];
                }
                catch(Exception ex)
                {   // lookup failed
                    TValue value;
                    if (TryGetValue(key, out value))
                    {
                        return value;
                    }
                    else  // still failed
                    {
                        throw ex;    // rethrow the original exception
                    }
                }
            }
            set { base[key] = value;  } 
        }

        protected virtual bool TryCreate(TKey key, out TValue value)
        {
            value = _createDelegate(key);
            if(value != null)
            {
                Add(key, value);
            }
            return value != null;
        }

    }
}