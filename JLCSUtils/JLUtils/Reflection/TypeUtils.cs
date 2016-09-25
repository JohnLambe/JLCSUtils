using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class TypeUtils
    {
        public static bool IsNumeric(Object value)
        {
            return value is SByte || value is Byte
                || value is Int16 || value is UInt16
                || value is Int32 || value is UInt32
                || value is Int64 || value is UInt64
                || value is Double
                || value is Single;
        }
    }
}
