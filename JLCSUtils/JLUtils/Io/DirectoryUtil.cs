using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Io
{
    /// <summary>
    /// File directory -related utilities.
    /// </summary>
    public static class DirectoryUtil
    {
        /// <summary>
        /// Find the first 
        /// </summary>
        /// <param name="filename">File pathname. If this has an extension, it is changed.
        /// If the leaf name includes a '.' and you want to add an extension after it, append '.' to the filename supplied.
        /// </param>
        /// <returns></returns>
        public static string GetFullFilename(string filename, params string[] allowedExtensions)
        {
            foreach (var extension in allowedExtensions)
            {
                string tryFilename = Path.ChangeExtension(filename, extension);
                if (File.Exists(tryFilename))
                    return tryFilename;
            }
            return null;   // none found
        }

        /// <summary>
        /// Find the first file matching a given wildcard.
        /// </summary>
        /// <param name="filename">Possibly-wildcarded pathname.</param>
        /// <returns>Pathname of the file found, or null if there is no matching file.</returns>
        public static string GetMatchingFilename(string filename)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(filename), Path.GetFileName(filename));
            if (files.Length == 0)
                return null;   // none found
            else
                return files[0];   // first file found
        }
    }
}
