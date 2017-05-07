using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace MvpFramework.Services
{
    /// <summary>
    /// A service that provides details of the current system (details provided by the <see cref="System.Environment"/> class.
    /// </summary>
    public interface IEnvironmentBasicService
    {
        /// <summary>
        /// <see cref="Environment.MachineName"/>.
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// <see cref="Environment.HasShutdownStarted"/>.
        /// </summary>
        bool HasShutdownStarted { get; }

        /// <summary>
        /// <see cref="Environment.Is64BitOperatingSystem"/>.
        /// </summary>
        bool Is64BitOperatingSystem { get; }
        
        /// <summary>
        /// <see cref="Environment.Is64BitProcess"/>.
        /// </summary>
        bool Is64BitProcess { get; }
        
        /// <summary>
        /// <see cref="Environment.NewLine"/>.
        /// </summary>
        string NewLine { get; }
        
        /// <summary>
        /// <see cref="Environment.OSVersion"/>.
        /// </summary>
        OperatingSystem OSVersion { get; }
        
        /// <summary>
        /// <see cref="Environment.ProcessorCount"/>.
        /// </summary>
        int ProcessorCount { get; }
        
        /// <summary>
        /// <see cref="Environment.UserDomainName"/>.
        /// </summary>
        string UserDomainName { get; }
        
        /// <summary>
        /// <see cref="Environment.UserInteractive"/>.
        /// </summary>
        bool UserInteractive { get; }
        
        /// <summary>
        /// <see cref="Environment.UserName"/>.
        /// </summary>
        string UserName { get; }
    }

    public interface IEnvironmentService : IEnvironmentBasicService
    {
        /// <summary>
        /// <see cref="Environment.CommandLine"/>.
        /// </summary>
        string CommandLine { get; }

        /// <summary>
        /// <see cref="Environment.CurrentDirectory"/>.
        /// </summary>
        string CurrentDirectory { get; /*set;*/ }

        /// <summary>
        /// <see cref="Environment.CurrentManagedThreadId"/>.
        /// </summary>
        int CurrentManagedThreadId { get; }

        /// <summary>
        /// <see cref="Environment.ExitCode"/>.
        /// </summary>
        int ExitCode { get; set; }

        /// <summary>
        /// <see cref="Environment.StackTrace"/>.
        /// </summary>
        string StackTrace { get; }
        
        /// <summary>
        /// <see cref="Environment.SystemDirectory"/>
        /// </summary>
        string SystemDirectory { get; }

        /// <summary>
        /// <see cref="Environment.SystemPageSize"/>
        /// </summary>
        int SystemPageSize { get; }

        //int TickCount { get; }  // Use ITimeService

        /// <summary>
        /// <see cref="Environment.Version"/>
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// <see cref="Environment.WorkingSet"/>
        /// </summary>
        long WorkingSet { get; }

        //void Exit(int exitCode);

        /// <summary>
        /// <see cref="Environment.ExpandEnvironmentVariables(string)"/>
        /// </summary>
        string ExpandEnvironmentVariables(string name);

        /// <summary>
        /// <see cref="Environment.FailFast(string, Exception)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void FailFast(string message, Exception exception = null);
        
        /// <summary>
        /// <see cref="Environment.GetCommandLineArgs"/>
        /// </summary>
        /// <returns></returns>
        string[] GetCommandLineArgs();
        
        /// <summary>
        /// <see cref="Environment.GetEnvironmentVariable(string, EnvironmentVariableTarget)"/>
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process);
        
        /// <summary>
        /// <see cref="Environment.GetEnvironmentVariable(string, EnvironmentVariableTarget)"/>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process);
        
        //string GetFolderPath(SpecialFolder folder);
        /// <summary>
        /// <see cref="Environment.GetFolderPath(SpecialFolder, SpecialFolderOption)"/>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        string GetFolderPath(SpecialFolder folder, SpecialFolderOption option = SpecialFolderOption.None); //TODO: Check that this is default for the other overload

        /// <summary>
        /// <see cref="Environment.GetLogicalDrives"/>
        /// </summary>
        /// <returns></returns>
        string[] GetLogicalDrives();

        //void SetEnvironmentVariable(string variable, string value);
        /// <summary>
        /// <see cref="Environment.SetEnvironmentVariable(string, string, EnvironmentVariableTarget)"/>
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
        void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process); //TODO: Check that this is default for the other overload
    }


    /// <summary>
    /// Implementation <see cref="IEnvironmentService"/> that returns environment details of the current system.
    /// </summary>
    public class EnvironmentService : IEnvironmentBasicService
    {
        public virtual bool HasShutdownStarted => Environment.HasShutdownStarted;

        public virtual bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;

        public virtual bool Is64BitProcess => Environment.Is64BitProcess;

        public virtual string MachineName => Environment.MachineName;

        public virtual string NewLine => Environment.NewLine;

        public virtual OperatingSystem OSVersion => Environment.OSVersion;

        public virtual int ProcessorCount => Environment.ProcessorCount;

        public virtual string UserDomainName => Environment.UserDomainName;

        public bool UserInteractive => Environment.UserInteractive;

        public virtual string UserName => Environment.UserName;
    }


    /// <summary>
    /// A class that holds environmental parameters (details provided by the <see cref="System.Environment"/> class), initially populated with those of the current system.
    /// This can be used to keep/store a copy of these details.
    /// To get live values, use <see cref="EnvironmentService"/>.
    /// </summary>
    public class EnvironmentDetails : IEnvironmentBasicService
    {
        public virtual bool HasShutdownStarted { get; set; } = Environment.HasShutdownStarted;

        public virtual bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;

        public virtual bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;

        public virtual string MachineName { get; set; } = Environment.MachineName;

        public virtual string NewLine { get; set; } = Environment.NewLine;

        public virtual OperatingSystem OSVersion { get; set; } = Environment.OSVersion;

        public virtual int ProcessorCount { get; set; } = Environment.ProcessorCount;

        public virtual string UserDomainName { get; set; } = Environment.UserDomainName;

        public virtual bool UserInteractive { get; set; } = Environment.UserInteractive;

        public virtual string UserName { get; set; } = Environment.UserName;
    }


    /// <summary>
    /// Mock implementation of <see cref="IEnvironmentBasicService"/>.
    /// Most properties are initially populated with the live values (of the current system), but can be reassigned.
    /// </summary>
    public class MockEnvironmentService : EnvironmentDetails
    {
    }

}
