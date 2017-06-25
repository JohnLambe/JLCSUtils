// by John Lambe - Public Domain.

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
    /// Utilities for working with files or filing systems.
    /// </summary>
    /// <seealso cref="PathUtil"/>
    public static class FilingUtil
    {
        /// <summary>
        /// Open the file to append if it exists, or create it if it doesn't exist.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="createDirectory">Iff true, and the directory of the file doesn't exist, it is created.</param>
        /// <returns>Stream for the opened file.</returns>
        /// <exception>Can throw the same exceptions as File.OpenWrite or File.Create.</exception>
        /// <seealso cref="DirectoryUtil"/> - for anything which involves scanning directories.
        public static FileStream OpenAppendOrCreate([FilenameValidation] string filename, bool createDirectory = false)
        {
            try
            {
                var stream = File.OpenWrite(filename);  // open for writing
                stream.Seek(0, SeekOrigin.End);         // seek to end
                return stream;
            }
            catch(IOException ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    if (createDirectory)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    }
                    return File.Create(filename);
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
