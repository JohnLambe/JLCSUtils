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
        /// Version number string with no trailing zero parts.
        /// The first part is always included (even if 0).
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string ToCompactString(this Version version)
        {
            int parts = NonZeroParts(version);
            return version.Major +
                (parts < 2 ? "" :
                    "." + version.Minor +
                    (parts < 3 ? "" :
                        "." + version.Build +
                        (parts < 4 ? "" :
                            "." + version.Revision
                        )
                    )
                );
        }

        /// <summary>
        /// Returns the minimum number of parts that can be used to display the version number -
        /// the number of parts in the version number, excluding trailing zero parts.
        /// <para>
        /// In the range 1 to 4 inclusive.
        /// </para>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int NonZeroParts(this Version version)
        {
            if (version.Revision > 0)
                return 4;
            else if (version.Build > 0)
                return 3;
            else if (version.Minor > 0)
                return 2;
            else
                return 1;
        }

        public static int Parts(this Version version)
        {
            if (version.Revision > -1)
                return 4;
            else if (version.Build > -1)
                return 3;
            else if (version.Minor > -1)
                return 2;
            else
                return 1;
        }

        /// <summary>
        /// Same as <see cref="Version(string)"/> except<br/>
        /// - "" or null returns null<br/>
        /// - one part version numbers are supported (e.g. "1" is equivalent to "1.0").
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static Version FromString(string version)
        {
            if (string.IsNullOrEmpty(version))
                return null;
            if (!version.Contains("."))    // only one part (not supported by Version(string))
                return new Version(int.Parse(version), 0);
            else
                return new Version(version);
        }

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
