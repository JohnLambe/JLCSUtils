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
        public static Exception ExtractException([Nullable] Exception ex, [Nullable("none")] Type[] wrappingExceptionsAssignable, [Nullable("none")] Type[] wrappingExceptions = null)
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
        /// Returns an inner exception (possibly multiple levels down) of the given exception
        /// of one of the given types.
        /// <para>
        /// If the given exception is of assignable to of the required types (in <paramref name="requiredException"/>), it is returned,
        /// otherwise, if its <see cref="Exception.InnerException"/> is assignable to one of those types, it is returned,
        /// otherwise, this is repeated for each level of inner exception until one is found of one of the required types, or the InnerException is null.
        /// </para>
        /// </summary>
        /// <param name="ex">Original exception.</param>
        /// <param name="requiredException">Types of exception that can be returned.</param>
        /// <returns>The extracted exception. null if there is no inner exception of one of the required types, or <paramref name="ex"/> is null.</returns>
        [return: Nullable("If required exception type is not found")]
        public static Exception ExtractExceptionOfType([Nullable] Exception ex, params Type[] requiredException)
        {
            if (ex == null)
                return null;
            do
            {
                if(TypeUtil.IsAssignableToAny(ex.GetType(), requiredException))
                {
                    return ex;           // matching exception found
                }
                ex = ex.InnerException;
            } while (ex.InnerException != null);
            return null;   // not found
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
            catch (Exception ex) when (ex is TException || (ex is TargetInvocationException && ex.InnerException is TException))
            {
                if (onError != null)
                    return onError.Invoke(ex.InnerException as TException);
                else
                    return default(T);
            }
        }
        //TODO: params list of delegates.

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
            catch (Exception ex) when(ex is TException || (ex is TargetInvocationException && ex.InnerException is TException))
            {
                return defaultValue;
            }
        }
    }
}
