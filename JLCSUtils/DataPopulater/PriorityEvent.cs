using JohnLambe.Util.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util
{
    /// <summary>
    /// A list of handlers that can be fired in order of a priority.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public abstract class PriorityEventBase<THandler,TReturn>
    {
//        protected delegate int Del();
//        protected virtual event Del OnX;
        public const int DefaultPriority = 500;

        /*
        public virtual TReturn Invoke()
        {
            foreach(var handler in _handlers)
            {
                InvokeHandler(handler.Value);
            }

            return default(TReturn);
        }

        protected abstract TReturn InvokeHandler(THandler handler);
*/

        public virtual void Add(THandler handler, int priority = DefaultPriority)
        {
            long baseKey = priority + 1 << 32;
            long key = baseKey;
            int index = _handlers.NearestIndexToKey(key);// _handlers.IndexOfKey(key);
            if(index > -1)     // if there is an existing entry with this priortiy
            {                  // find the highest key value with this entry
                do
                {
                    index++;
                    key = _handlers.Keys[index];
                } while(index < _handlers.Count && (key >> 32) == priority);
            }
            _handlers.Add(key + 1, handler);
        }

        public virtual bool Remove(THandler handler)
        {
            int index = _handlers.IndexOfValue(handler);
            if (index > -1)
            {
                _handlers.Values.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected SortedList<long,THandler> _handlers = new SortedList<long,THandler>();

        /// <summary>
        /// Combined with the 'priority' value to form a unique key for the list.
        /// </summary>
        protected uint _uniqueId = 0;
    }

    public class PriorityEvent<THandler, TReturn> : PriorityEventBase<THandler,TReturn>
    {
        public virtual TReturn Invoke()
        {
            foreach (var handler in _handlers)
            {
//                InvokeHandler(handler.Value);
            }

            return default(TReturn);
        }
    }

}
