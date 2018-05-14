using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;

namespace JohnLambe.Util.Logging.Log4n
{
    public class LoggerInjector
    {
        public void InjectStaticLogger(Type type)
        {
            foreach(var property in type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                var attribute = property.GetCustomAttribute<InjectLoggerAttribute>();
                if (attribute != null)
                {
                    property.SetValue(null, GetLogger(type, attribute.Name));
                }
            }
        }

        public void InjectInstanceLogger(IHasLogger receiver)
        {
            var property = receiver.GetType().GetProperty(nameof(IHasLogger.Logger));

            receiver.Logger = GetLogger(receiver.GetType());

//            System.Reflection.MethodBase.GetCurrentMethod().Name

  //log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public virtual IGeneralLogger GetLogger(Type type, string name = null)
        {
            return new Log4nAdaptor(
                name != null ? log4net.LogManager.GetLogger(name) : log4net.LogManager.GetLogger(type)
                );
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InjectLoggerAttribute : Attribute
    {
        public virtual string Name { get; set; }
    }
}
