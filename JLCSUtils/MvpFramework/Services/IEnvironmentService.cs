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
        string MachineName { get; }
        bool HasShutdownStarted { get; }
        bool Is64BitOperatingSystem { get; }
        bool Is64BitProcess { get; }
        string NewLine { get; }
        OperatingSystem OSVersion { get; }
        int ProcessorCount { get; }
        string UserDomainName { get; }
        bool UserInteractive { get; }
        string UserName { get; }
    }

    public interface IEnvironmentService : IEnvironmentBasicService
    {
        string CommandLine { get; }
        string CurrentDirectory { get; /*set;*/ }
        //int CurrentManagedThreadId { get; }
        //int ExitCode { get; set; }
        //string StackTrace { get; }
        string SystemDirectory { get; }
        int SystemPageSize { get; }
        //int TickCount { get; }  // Use ITimeService
        Version Version { get; }
        long WorkingSet { get; }

        //void Exit(int exitCode);
        string ExpandEnvironmentVariables(string name);
        void FailFast(string message);
        void FailFast(string message, Exception exception);
        string[] GetCommandLineArgs();
        string GetEnvironmentVariable(string variable);
        string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
        IDictionary GetEnvironmentVariables();
        IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target);
        string GetFolderPath(SpecialFolder folder);
        string GetFolderPath(SpecialFolder folder, SpecialFolderOption option);
        string[] GetLogicalDrives();
        void SetEnvironmentVariable(string variable, string value);
        void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target);
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
}
