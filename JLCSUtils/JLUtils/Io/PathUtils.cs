using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace JohnLambe.Util.Io
{
    /// <summary>
    /// Utilities for working with file pathnames.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// Appends a string before the extension of a pathname.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="append"></param>
        /// <returns>The modified pathname.</returns>
        public static string AppendBeforeExtension(string path, string append)
        {
            return Path.ChangeExtension(path, null) + append + Path.GetExtension(path);
        }

        /// <summary>
        /// Change the filename part of a pathname without changing the extension.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newFilename">The new filename part without the extension.
        /// If this has an extension, it is inserted before the existing extension.</param>
        /// <returns>The modified pathname.</returns>
        public static string ChangeFilenameWithoutExtension(string path, string newFilename)
        {
            return Path.Combine(Path.GetDirectoryName(path), newFilename + Path.GetExtension(path));
        }

        /// <summary>
        /// Change the filename part (including the extension) of a given pathname.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newFilename">The new filename part, including the extension.</param>
        /// <returns>The modified pathname.</returns>
        public static string ChangeFilename(string path, string newFilename)
        {
            return Path.Combine(Path.GetDirectoryName(path), newFilename);
        }

        /// <summary>
        /// Change the directory (including the root) part of a pathname.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newDirectory"></param>
        /// <returns></returns>
        public static string ChangeDirectory(string path, string newDirectory)
        {
            return Path.Combine(newDirectory, Path.GetFileName(path));
        }

    }
}
