using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Logging
{
    public interface ILoggerFactory // : IFactory<IGeneralLogger>
    {
        IGeneralLogger GetLogger(Type type, string name = null);
    }

    public static class LoggerFactoryExtension
    {
        public static IGeneralLogger GetLogger(this ILoggerFactory factory, string name)
            => factory.GetLogger(null, name);
    }
}
