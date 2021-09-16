using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    public static class VersionCompatibilityChecker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local">Version of the local implementation.</param>
        /// <param name="localLcv">Lowest version that the local implementation is compatible with.</param>
        /// <param name="remote">Version of the remote implementation.</param>
        /// <param name="remoteLcv">Lowest version that the remote implementation is compatible with.</param>
        /// <returns>The latest version that both local and remote support,
        /// or null if there is no commonly supported version.</returns>
        public static Version CheckVersion(Version local, Version localLcv, Version remote, Version remoteLcv)
        {
            if (local >= remote)
            {
                if (remote >= localLcv)
                    return remote;
                else
                    return null;
            }
            else
            {
                if (local >= remoteLcv)
                    return local;
                else
                    return null;
            }
        }

        /// <summary>
        /// Same as <see cref="CheckVersion(Version, Version, Version, Version)"/>
        /// but the version are provided as strings.
        /// </summary>
        /// <param name="local">The local verion and lowest compatible version, separated by a semi-colon.</param>
        /// <param name="remote">The remote verion and lowest compatible version, separated by a semi-colon.</param>
        /// <returns>The latest compatible version, or null if there is no compatible version.</returns>
        public static Version CheckVersion(string local, string remote)
        {
            StrUtil.SplitToVars(local, ';', out var localVersion, out var localLcv);
            StrUtil.SplitToVars(remote, ';', out var remoteVersion, out var remoteLcv);
            return CheckVersion(VersionUtil.FromString(localVersion), VersionUtil.FromString(localLcv), VersionUtil.FromString(remoteVersion), VersionUtil.FromString(remoteLcv));
        }

        /// <summary>
        /// Same as <see cref="CheckVersion(string, string)"/> except that the result is a string.
        /// </summary>
        /// <param name="local"></param>
        /// <param name="remote"></param>
        /// <returns>The latest compatible version, or null if there is no compatible version.</returns>
        public static string CheckVersionString(string local, string remote)
        {
            return CheckVersion(local, remote)?.ToString();
        }
    }
}