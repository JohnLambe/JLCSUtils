using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Exceptions
{
    /// <summary>
    /// Utilities related to exceptions.
    /// </summary>
    public static class ExceptionUtil
    {
        /// <summary>
        /// Returns the inner exception (recursively) of the given one if the given one is <see cref="TargetInvocationException"/> (later versions may also do this for other exceptions that just wrap another),
        /// otherwise the given exception.
        /// </summary>
        /// <param name="ex">The initial exception.
        /// There must not be a loop in the InnerException chain (<see cref="Exception.InnerException"/> must not point to an exception in which this is nested) (otherwise, this would loop indefinitely).
        /// </param>
        /// <returns>
        /// The given exception.
        /// Returns null if and only if <paramref name="ex"/> is null.
        /// The given exception is returned if its inner exception is null.
        /// </returns>
        [return: Nullable("Iff passed null")]
        public static Exception ExtractException([Nullable] Exception ex)
        {
            if (ex == null)
                return null;
            while(ex is TargetInvocationException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        /// <summary>
        /// Exctract the inner exception recursively, where the outer one is one of the specified types.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="wrappingExceptionsAssignable">Exceptions of types that can be assigned to these (e.g. of subclasses of these) are removed.</param>
        /// <param name="wrappingExceptions">Exceptions matching exactly these types are removed.</param>
        /// <returns>The extracted exception (may be the <paramref name="ex"/>).</returns>
        [return: Nullable("Iff passed null")]
        public static Exception ExtractException([Nullable] Exception ex, Type[] wrappingExceptionsAssignable, Type[] wrappingExceptions)
        {
            if (ex == null)
                return null;
            while (ex.InnerException != null
                && (TypeUtil.IsAssignableToAny(ex.GetType(), wrappingExceptionsAssignable) || (wrappingExceptions?.Contains(ex.GetType()) ?? false)))
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        /// <summary>
        /// Execute a delegate, and execute a different one if the first throws an exception.
        /// </summary>
        /// <typeparam name="T">The type returned.</typeparam>
        /// <typeparam name="TException">The exception class to catch. Other exceptions are thrown normally.</typeparam>
        /// <param name="func"></param>
        /// <param name="onError"></param>
        /// <returns>The return value of <paramref name="func"/> if it did not throw an exception,
        /// otherwise, the return value of <paramref name="onError"/>.</returns>
        [return: Nullable]
        public static T TryEvaluate<T,TException>(Func<T> func, Func<Exception,T> onError)
            where TException : Exception
        {
            try
            {
                return func();
            }
            catch(TargetInvocationException ex) when (ex.InnerException is TException)
            {
                if (onError != null)
                    return onError.Invoke(ex.InnerException as TException);
                else
                    return default(T);
            }
        }

        /// <summary>
        /// <inheritdoc cref="TryEvaluate{T}(Func{T}, Func{Exception, T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        [return: Nullable]
        public static T TryEvaluate<T>(Func<T> func, Func<Exception, T> onError)
            => TryEvaluate<T, Exception>(func, onError);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type returned.</typeparam>
        /// <typeparam name="TException">The exception class to catch. Other exceptions are thrown normally.</typeparam>
        /// <param name="func"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryEvaluate<T, TException>(Func<T> func, T defaultValue = default(T))
            where TException : Exception
        {
            try
            {
                return func();
            }
            catch (TargetInvocationException ex) when(ex.InnerException is TException)
            {
                return defaultValue;
            }
        }
    }
}
