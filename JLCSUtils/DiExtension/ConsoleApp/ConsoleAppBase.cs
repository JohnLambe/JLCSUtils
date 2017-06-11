using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConsoleApp
{
    // From ConsoleAppLib:

    /// <summary>
    /// Base class with utilities for command-line programs.
    /// </summary>
//    [AutoConfigProcessor(ProcessorClass = typeof(CommandLineAutoConfigProcessor))]
    public class ConsoleAppBase //: AutoConfigBase
    {
/*
        public static int Init(Type mainClass, string[] args)
        {
            //            var constructor = mainClass.GetConstructor(new Type[] { args.GetType() });
            var constructor = mainClass.GetConstructor(new Type[] { typeof(string[]) });

            if (constructor == null)
                throw new ArgumentException("ConsoleAppBase.Main: Given type does not have a constructor taking string[]");

            var program = constructor.Invoke(new object[] { args }) as ConsoleAppBase;
            if (program == null)
                throw new ArgumentException("ConsoleAppBase.Main: Given type is not valid");
            program.Configure();
            return program.Execute();
            //            new CommandLineProgram(args).Execute();
        }
*/

        public ConsoleAppBase()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">the command-line arguments.</param>
        public ConsoleAppBase(string[] args)
        {
            Args = args;
        }


        /// <summary>
        /// Default name of the executable, without the extension.
        /// This defaults to the class name.
        /// Override if different.
        /// </summary>
        public virtual string ProgramName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// A line of text with the name of this utility and the version number.
        /// </summary>
        public virtual string VersionStringLine()
        {
            return ProgramName + " " /* + VersionUtils.ApplicationVersion */;
        }

        /// <summary>
        /// Outputs a line of text with the name of this utility and the version number.
        /// </summary>
        public virtual void OutputVersion()
        {
            Console.Out.WriteLine(VersionStringLine());
        }

        /// <summary>
        /// The main method of the command line program.
        /// </summary>
        /// <returns></returns>
        protected virtual int Execute()
        {
            return 0;
        }

        /// <summary>
        /// The command line arguments.
        /// </summary>
        protected virtual string[] Args { get; private set; }
    }
}
