using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace JohnLambe.Util.Reflection
{
    public static class ReflectionUtils
    {

        public static T Instantiate<T>(Type t) // where T: new()
        {
            return (T) t.GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

        /*
        public static T? StringToEnum<T>(string value, T? defaultValue = null) where T: struct
        {
            MemberInfo memberInfo = typeof(T).GetMembers().FirstOrDefault( m => m.Name.Equals(value) );
            if (memberInfo == null)
            {
                return defaultValue;
            }
            else
            {
                return memberInfo.

            }
        }
        */

    }

}
