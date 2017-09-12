using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Exceptions
{
    public static class ExceptionUtil
    {
        /// <summary>
        /// Returns the inner exception (recursively) of the given one if the out is <see cref="TargetInvocationException"/> (later versions may also do this for other exceptions that just wrap another),
        /// otherwise the given exception.
        /// </summary>
        /// <param name="ex">The initial exception.</param>
        /// <returns>
        /// The given exception or inner exception.
        /// Returns null if and only if <paramref name="ex"/> is null.
        /// The given exception is returned if its inner exception is null.
        /// </returns>
        [return: Nullable]
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

    }
}
