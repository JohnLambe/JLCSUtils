using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Utilities related to version numbers.
    /// </summary>
    public static class VersionUtil
    {
        /// <summary>
        /// Convert the File Version in the given <see cref="FileVersionInfo"/> to a <see cref="Version"/>.
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <returns></returns>
        public static Version FileVersionToVersion(this FileVersionInfo fileVersion)
        {
            return new Version(fileVersion.FileMajorPart, fileVersion.FileMinorPart, fileVersion.FileBuildPart, fileVersion.FilePrivatePart);
        }

        /// <summary>
        /// Convert the Product Version in the given <see cref="FileVersionInfo"/> to a <see cref="Version"/>.
        /// </summary>
        /// <param name="fileVersion"></param>
        /// <returns></returns>
        public static Version ProductVersionToVersion(this FileVersionInfo fileVersion)
        {
            return new Version(fileVersion.ProductMajorPart, fileVersion.ProductMinorPart, fileVersion.ProductBuildPart, fileVersion.ProductPrivatePart);
        }

        public static IApplicationInfo GetAppInfo(string filename)
        {
            return GetAppInfo(FileVersionInfo.GetVersionInfo(filename));
        }

        public static IApplicationInfo GetAppInfo(FileVersionInfo fileVersion)
        {
            return new ApplicationInfo()
            {
                AppName = fileVersion.ProductName,
                Copyright = fileVersion.LegalCopyright,
                Version = fileVersion.ProductVersionToVersion()
            };
        }
    }


    public interface IApplicationInfo
    {
        string AppName { get; }
        Version Version { get; }
        string Copyright { get; }
        Image Icon { get; }
    }

    public class ApplicationInfo : IApplicationInfo
    {
        public string AppName { get; set; }

        public string Copyright { get; set; }

        public Image Icon { get; set; }

        public Version Version { get; set; }
    }

}
