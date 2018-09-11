using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;

namespace MvpFramework.WinForms
{
    // Could use the general Handler Resolver. This has better performance.
    // Could support non-standard keys combinations (not representable by Keys), e.g. LEFT CTRL + RIGHT CTRL.

    public class CommandKeyProcessor
    {
        /// <summary>
        /// Fire the handler for the given keystroke.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns>true iff handled. false if there is no handler.</returns>
        public virtual bool ProcessKey(Keys keyData)
        {
            return Handlers.TryGetValue(keyData)?.Invoke(keyData) ?? false;
        }

        /// <summary>
        /// Define a handler for the given keystroke.
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="handler">The handler for this key.</param>
        public virtual void RegisterHandler(Keys keyData, KeyHandlerDelegate handler)
        {
            Handlers[keyData] = handler;
        }

        /// <summary>
        /// Define handlers based on attributes on a given object.
        /// </summary>
        /// <param name="handlerObject">The object which may define handlers.</param>
        public virtual void Scan(object handlerObject)
        {
            foreach(var method in handlerObject.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                var attribute = method.GetCustomAttribute<KeyHandlerAttribute>();
                if(attribute?.Enabled ?? false)
                {
                    KeyHandlerDelegate handler;
                    var parameters = method.GetParameters();
                    if (parameters.Count() == 0)
                        handler = key => (bool)(method.Invoke(handlerObject, EmptyCollection<object>.EmptyArray) ?? false);  // if void return value, treat as false
                    else if (parameters.Count() == 1 && parameters[1].ParameterType == typeof(Keys))
                        handler = key => (bool)(method.Invoke(handlerObject, new object[] { key }) ?? false);
                    else
                        throw new InvalidOperationException("Invalid handler method for " + nameof(KeyHandlerAttribute) + " on " + handlerObject + "; method: " + method.Name
                            + "; The handler method must have no parameters or one parameter of type Keys");
                    //TODO: Error if already registered
                    RegisterHandler(attribute.Key, handler);
                }
            }
        }

        protected IDictionary<Keys, KeyHandlerDelegate> Handlers { get; } = new Dictionary<Keys, KeyHandlerDelegate>();

        public delegate bool KeyHandlerDelegate(Keys keyData);
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class KeyHandlerAttribute : Attribute
    {
        public KeyHandlerAttribute(Keys key)
        {
            Key = key;
        }

        public virtual Keys Key { get; set; }

        public virtual bool Enabled { get; set; } = true;
    }
}
