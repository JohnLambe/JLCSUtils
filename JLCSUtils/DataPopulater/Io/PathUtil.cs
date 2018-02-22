using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;

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
        public const char ExtensionSeparatorChar = '.';  // platform-specific (but same for most)

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

        /// <summary>
        /// </summary>
        /// <param name="path">The pathname to test. If null, false is returned.</param>
        /// <returns>True if the pathname contains any wildcard character(s).</returns>
        public static bool HasWildcard([Nullable] string path)
        {
            return path?.ContainsAnyCharacters(Wildcards) ?? false;
        }

        public static FilePathCompleteness PathCompleteness(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return FilePathCompleteness.FullPath;
            }
            else if (path.Contains(Path.DirectorySeparatorChar) || path.Contains(Path.AltDirectorySeparatorChar)
                || path.Contains(Path.VolumeSeparatorChar))
            {
                return FilePathCompleteness.RelativePath;
            }
            else
            {
                return FilePathCompleteness.LeafName;
            }
        }

        private static readonly ISet<char> _wildcards = new HashSet<char>(new [] { '*', '#' });  //TODO: Windows-specific
        public static ISet<char> Wildcards => _wildcards;
    }
}
