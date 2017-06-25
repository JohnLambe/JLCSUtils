using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// General utility methods.
    /// </summary>
    public static class GeneralUtil
    {
        /// <summary>
        /// If an exception of the given type, or assignable to the given type, is raised, it is suppressed and <paramref name="defaultValue"/>
        /// is returned.
        /// Otherwise, the result of <paramref name="operation" /> is returned. 
        /// If a different exception is thrown by <paramref name="operation" />, it is thrown by this.
        /// <para>This is for use when a certain type of exception is expected in certain circumstances.</para>
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="exceptionType">The type of exception to suppress. Anything exception class assignable to this is suppressed
        /// (e.g. subclasses; this can be an interface and types that implement it will be suppressed).
        /// <para>Passing <code>typeof(Exception)</code> suppresses all exceptions. This is generally not recommended.</para>
        /// </param>
        /// <param name="defaultValue">The value to be returned if an exception is suppressed.</param>
        /// <returns>The return value of the operation, or the default value.</returns>
        public static T IgnoreException<T>(Func<T> operation, Type exceptionType, T defaultValue = default(T))
        {
            try
            {
                return operation.Invoke();
            }
            catch(Exception ex)
            {
                if (exceptionType.IsAssignableFrom(ex.GetType()))
                    return defaultValue;
                else
                    throw;
            }
        }

        /// <summary>
        /// Same as <see cref="IgnoreException{T}(Func{T}, Type, T)"/> except that multiple exception types can be given.
        /// If an exception type assignable to any of the given types is given, it is suppressed.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="exceptionTypes">The types of exception to suppress. Anything exception class assignable to any of these is suppressed
        /// (e.g. subclasses; this can be an interface and types that implement it will be suppressed).
        /// <para>Passing <code>typeof(Exception)</code> suppresses all exceptions. This is generally not recommended.</para>
        /// </param>
        /// <param name="defaultValue">The value to be returned if an exception is suppressed.</param>
        /// <returns>The return value of the operation, or the default value.</returns>
        public static T IgnoreException<T>(Func<T> operation, Type[] exceptionTypes, T defaultValue = default(T))
        {
            try
            {
                return operation.Invoke();
            }
            catch (Exception ex)
            {
                foreach (var exceptionType in exceptionTypes)
                {
                    if (exceptionType != null && exceptionType.IsAssignableFrom(ex.GetType()))
                        return defaultValue;
                }
                throw;
            }
        }
    }
}
