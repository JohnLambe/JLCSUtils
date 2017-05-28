using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
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
    /// <seealso cref="PathUtil"/>
    public static class DirectoryUtil
    {
        /// <summary>
        /// Pass this as an extension, to <see cref="GetFullFilename(string, string[])"/> to return the filename 
        /// of the first file with the given filename changed to any extension.
        /// </summary>
        public const string Extension_Any = "\x01";

        /// <summary>
        /// Returns the filename with the extension changed to the frist one in <paramref name="extensions"/> that matches an existing file.
        /// </summary>
        /// <param name="filename">Original filename (absolute or relative to the current directory).
        /// If this has an extension, it is changed.
        /// If the leaf name includes a '.' and you want to add an extension after it, append '.' to the filename supplied.
        /// </param>
        /// <param name="allowedExtensions">The extensions to try. A null element means no extension (the array must not be null).</param>
        /// <returns>The file found (relative to the same directory as <paramref name="filename"/>), or null if none was matched.</returns>
        [return: Nullable]
        public static string GetFullFilename([NotNull][FilenameValidation] string filename, params string[] allowedExtensions)
        {
            foreach (var extension in allowedExtensions)
            {
                string tryFilename;
                if (extension == Extension_Any)
                {
                    tryFilename = GetMatchingFilename(Path.ChangeExtension(filename, "*"));
                    if (tryFilename != null)
                        return tryFilename;
                }
                else
                {
                    tryFilename = Path.ChangeExtension(filename, extension);
                    if (File.Exists(tryFilename))
                        return tryFilename;
                }
            }
            return null;   // none found
        }

        /// <summary>
        /// Like <see cref="GetFullFilename(string, string[])"/>, except that it throws an exception if no matching file is found.
        /// </summary>
        /// <param name="filename">Original filename (absolute or relative to the current directory).
        /// If this has an extension, it is changed.
        /// If the leaf name includes a '.' and you want to add an extension after it, append '.' to the filename supplied.
        /// </param>
        /// <param name="allowedExtensions">The extensions to try. A null element means no extension (the array must not be null).</param>
        /// <returns>The file found (relative to the same directory as <paramref name="filename"/>).</returns>
        /// <exception cref="FileNotFoundException">if no matching file is found.</exception>
        [return: NotNull]
        public static string GetFullFilenameException([NotNull][FilenameValidation] string filename, params string[] allowedExtensions)
        {
            var found = GetFullFilename(filename, allowedExtensions);
            if (found == null)
                throw new FileNotFoundException("File not found: '" + filename + "' with any of these extensions: " + StrUtil.Concat(allowedExtensions,", "));
            return found;
        }

        /*
        /// <summary>
        /// Returns the filename with the extension changed to the frist one in <paramref name="extensions"/> that matches an existing file.
        /// </summary>
        /// <param name="filename">Original filename (absolute or relative to the current directory).</param>
        /// <param name="extensions">The extensions to try. A null element means no extension (the array must not be null).</param>
        /// <returns>The matched filename (relative to the same directory as <paramref name="filename"/>).</returns>
        public static string TryExtensions([NotNull][FilenameValidation] string filename, params string[] extensions)
        {
            foreach (string extension in extensions)
            {
                string tryFilename = Path.ChangeExtension(filename, extension);
                if (File.Exists(tryFilename))
                    return tryFilename;

                Directory.GetFiles(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ExtensionSeparatorChar);
            }
        }
        */

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


        /// <summary>
        /// Finds a file in a prioritised list of directories.
        /// <para>
        /// This looks for a file with the given relative filename (<paramref name="relativeFilename"/>) within each of the given directories
        /// (<paramref name="paths"/>), and returns the first one found.
        /// </para>
        /// </summary>
        /// <param name="relativeFilename">The filename to find, relative to each of of <paramref name="paths"/>.</param>
        /// <param name="paths">The list of paths/directories to search in. The search is done in the order of this array.</param>
        /// <returns>The pathname of the first matching file found in <see cref="paths"/>. null if not found.</returns>
        public static string FindFileOnPath(string relativeFilename, string[] paths)
        {
            foreach(var path in paths)
            {
                string filename = Path.Combine(path, relativeFilename);
                if (File.Exists(filename))
                    return filename;
            }
            return null;
        }
    }
}
