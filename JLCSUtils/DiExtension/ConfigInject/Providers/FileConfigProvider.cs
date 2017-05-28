using JohnLambe.Util.Io;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DiExtension.ConfigInject.Providers
{
    public abstract class FileConfigProviderBase : IConfigProvider
    {
        public abstract bool GetValue<T>(string key, Type requiredType, out T value);
    }

}
