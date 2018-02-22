using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util.Collections
{
    public class DictionaryWrapperMap : WrapperMap
    {

        public DictionaryWrapperMap(object data)
            : base(data)
        {
            _dictionary = data as IDictionary<string, object>;
        }

        #region IDictionary methods

        public override void Add(string key, object value)
        {
            if (_dictionary != null)
                _dictionary.Add(key, value);
            else
                throw new NotSupportedException();
        }

        public override bool ContainsKey(string key)
        {
            if (_dictionary != null)
                if (_dictionary.ContainsKey(key))
                    return true;
            return base.ContainsKey(key);
        }

        public override ICollection<string> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Remove(string key)
        {
            if (_dictionary != null)
                _dictionary.Remove(key);
            throw new NotImplementedException();
        }

        /*
         * public virtual bool TryGetValue(string key, out object value)
                {
                    throw new NotImplementedException();
                }
        */

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
            get { throw new NotImplementedException(); }
        }

        public override bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
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


        protected IDictionary<string, object> _dictionary;

    }
}
