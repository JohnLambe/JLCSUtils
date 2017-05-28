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
    /// <para>
    /// <seealso cref="DirectoryUtil"/> - for anything which involves scanning directories.</para>
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// The character that separates the extension from the rest of the filename.
        /// </summary>
        public const char ExtensionSeparatorChar = '.';

        /// <summary>
        /// Appends a string before the extension of a pathname.
        /// </summary>
        /// <param name="path">The original file pathname.</param>
        /// <param name="append">The part to be appended.</param>
        /// <returns>The modified pathname.</returns>
        public static string AppendBeforeExtension(string path, string append)
        {
            return Path.ChangeExtension(path, null) + append + Path.GetExtension(path);
        }

        /// <summary>
        /// Change the filename part of a pathname without changing the extension.
        /// </summary>
        /// <param name="path">The original file pathname.</param>
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
        /// <param name="path">The original file pathname.</param>
        /// <param name="newFilename">The new filename part, including the extension.</param>
        /// <returns>The modified pathname.</returns>
        public static string ChangeFilename(string path, string newFilename)
        {
            return Path.Combine(Path.GetDirectoryName(path), newFilename);
        }

        /// <summary>
        /// Change the directory (including the root) part of a pathname.
        /// </summary>
        /// <param name="path">The original file pathname.</param>
        /// <param name="newDirectory">The new directory part.</param>
        /// <returns>The modified pathname.</returns>
        public static string ChangeDirectory(string path, string newDirectory)
        {
            return Path.Combine(newDirectory, Path.GetFileName(path));
        }
    }
}
