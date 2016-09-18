using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Reflection;

using JohnLambe.Util.PluginFramework.Attributes;

namespace JohnLambe.Util.PluginFramework.Host
{
    public partial class PluginLoader
    {
        public PluginLoader(PluginHost host)
        {
            this.Host = host;
        }

        protected virtual PluginHost Host { get; set; }

        /// <summary>
        /// Directories to scan for Plugin Assemblies.
        /// </summary>
        public virtual IEnumerable<string> PluginDirectories { get; set; }
        public virtual BooleanExpression<string> FilenameFilter { get; set; }
        public virtual string DirectorySearchPattern { get; set; }
        public virtual SearchOption DirectorySearchOption { get; set; }

        /// <summary>
        /// Scans for and loads Plugin Assemblies.
        /// </summary>
        public virtual void LoadPlugins()
        {
            foreach(var directory in PluginDirectories)
            {
                ScanDirectory(directory);
            }
        }

        /// <summary>
        /// Scans a single directory for Plugin Assemblies, and loads them.
        /// </summary>
        /// <param name="directoryName"></param>
        public virtual void ScanDirectory(string directoryName)
        {
            foreach(var filename in Directory.GetFiles(directoryName, DirectorySearchPattern, DirectorySearchOption))
            {
                if(FilenameFilter.TryEvaluate(filename,true))
                {
                    LoadAssembly(filename);
                }
            }

        }

        /// <summary>
        /// Loads a single Plugin Assembly from the filesystem.
        /// </summary>
        /// <param name="assemblyFilename"></param>
        /// <returns></returns>
        public virtual bool LoadAssembly(string assemblyFilename)
        {
            return ScanAssembly(Assembly.LoadFile(assemblyFilename));            
        }

        /// <summary>
        /// Initialises Plugins in a loaded assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public virtual bool ScanAssembly(Assembly assembly)
        {
            foreach(var t in assembly.GetTypes()
                .Where( a => a.IsClass && !a.IsAbstract
                && a.HasCustomAttribute<PluginAttribute>())
                )
            {
                ProcessClass(t);
            }
            return true;  //TODO
        }

        /// <summary>
        /// Initialises an individual Plugin class.
        /// </summary>
        /// <param name="pluginClass"></param>
        /// <returns></returns>
        public virtual bool ProcessClass(Type pluginClass)
        {
            var pluginAttribute = pluginClass.GetCustomAttribute<PluginAttribute>();
            if ( pluginAttribute.InitialiseOnStart )
            {
                var plugin = pluginClass.Create<object>();
                
//                var pluginDetails = new PluginDetails(plugin);

                Host.AddPlugin(plugin);

            }
            return true; //TODO

        }
    }
}
