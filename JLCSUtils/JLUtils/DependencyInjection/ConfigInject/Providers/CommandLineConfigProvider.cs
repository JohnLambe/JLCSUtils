using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;

namespace JohnLambe.Util.DependencyInjection.ConfigInject.Providers
{
    /// <summary>
    /// Provider that reads command-line arguments.
    /// </summary>
    public class CommandLineConfigProvider : DictionaryConfigProviderBase<string>
    {
        public CommandLineConfigProvider()
        {
            PopulateArgs();
        }

        protected void PopulateArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach(var arg in args)
            {
                if(arg.StartsWith(ParameterPrefix))
                {
                    string name, value;
                    arg.RemovePrefix(ParameterPrefix).SplitToVars(ParameterValueSeparator, out name, out value);
                    if(name.Length > 0 && value != null && value.Length > 0)  // non-blank name and value
                    {
                        // un-escaping of quotes is already done (by Windows or .Net)
                        _values[name] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Prefix of a of switch of named parameter.
        /// </summary>
        protected const string ParameterPrefix = "/";
        /// <summary>
        /// Separator between the name and value of a named parameter.
        /// </summary>
        protected const char ParameterValueSeparator = '=';
//        protected const char Quote = '"';
    }
}


// Other providers:
//   EnvironmentVariable
//   ConfigFile
//     XML
//     INI
//     Flat (Java Properties)
