using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JohnLambe.Util.Logging
{
    public abstract class LoggerInjector
    {
        /// <summary>
        /// Inject static loggers for all relevant classes in the given assemblies.
        /// </summary>
        /// <param name="assemblies"></param>
        public void InjectStaticLoggers(params Assembly[] assemblies)
        {
            foreach(var assembly in assemblies)
            {
                foreach(var cls in assembly.GetTypes().Where(t => t.IsClass))   // for each class in the assembly
                {
                    InjectStaticLogger(cls);
                }
            }
        }


        /// <summary>
        /// Inject static logger(s) in the given type, based on attributes.
        /// </summary>
        /// <param name="type">The type to have loggers injected. Must be a class or struct.</param>
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

        /// <summary>
        /// Inject a logger into the given instance, using the interface, and the <see cref="InjectLoggerAttribute"/> if present.
        /// </summary>
        /// <param name="receiver"></param>
        public void InjectInstanceLogger(IHasLogger receiver)
        {
            string name = null;
            var property = receiver.GetType().GetProperty(nameof(IHasLogger.Logger));
            if (property != null)
            {
                var attribute = property.GetCustomAttribute<InjectLoggerAttribute>();
                name = attribute?.Name;
            }

            receiver.Logger = GetLogger(receiver.GetType(), name);

//            System.Reflection.MethodBase.GetCurrentMethod().Name

  //log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Get a logger for a given type and name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IGeneralLogger GetLogger(Type type, string name = null);
    }


    /// <summary>
    /// Indicates that logger should be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InjectLoggerAttribute : Attribute
    {
        public virtual string Name { get; set; }
    }
}
