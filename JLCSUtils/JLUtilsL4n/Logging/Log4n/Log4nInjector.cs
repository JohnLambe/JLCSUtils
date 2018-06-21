using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Logging.Log4n
{
    public class Log4nInjector : LoggerInjector
    {
        /// <summary>
        /// Get a logger for a given type and name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IGeneralLogger GetLogger(Type type, string name = null)
        {
            return new Log4nAdapter(
                name != null ? log4net.LogManager.GetLogger(name) : log4net.LogManager.GetLogger(type)
                );
        }
    }
}
