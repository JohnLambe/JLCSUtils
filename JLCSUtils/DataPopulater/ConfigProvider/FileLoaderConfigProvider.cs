using DiExtension.ConfigInject;
using JohnLambe.Util.Io;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JohnLambe.Util.ConfigProvider
{
    /// <summary>
    /// Provider that resolves a key to the contents of a file.
    /// </summary>
    public abstract class FileLoaderConfigProvider : IConfigProvider
    {
        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            try
            {
                string filename = GetFilenameFromKey(key);
                var content = LoadFile<object>(filename, requiredType);
                value = GeneralTypeConverter.Convert<T>(content);
                return true;
            }
            catch (FileNotFoundException ex)
            {
                if (ExceptionWhenNotFound)
                {
                    throw new KeyNotFoundException("Key not found: " + key, ex);
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
        }

        /// <summary>
        /// Return the filename (full pathname) corresponding to a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string GetFilenameFromKey(string key)
        {
            string filename = Path.Combine(Directory, key);
            if (Extensions.Length > 0)
                filename = DirectoryUtil.GetFullFilenameException(filename, Extensions);
            return filename;
        }

        /// <summary>
        /// Load the whole contents of a file into an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="requiredType"></param>
        /// <returns></returns>
        protected virtual T LoadFile<T>(string filename, Type requiredType)
        {
            object content = null;
            if (requiredType.Equals(typeof(byte[])))
            {
                content = File.ReadAllBytes(filename);
            }
            else
            {
                content = File.ReadAllText(filename);
            }
            return (T)content;
        }

        /// <summary>
        /// The pathname of the directory from which the files are loaded.
        /// </summary>
        [FilenameValidation(IsDirectory = NullableBool.True)]
        public virtual string Directory { get; set; }

        /// <summary>
        /// The prioritised list of extensions of files supported.
        /// If not empty, the extension is changed to the first of these that matches a file.
        /// <para>Supports <see cref="DirectoryUtil.Extension_Any"/>.</para>
        /// </summary>
        public virtual string[] Extensions { get; set; }

        /// <summary>
        /// Iff true, exceptions are thrown if the file is not found.
        /// If false, default(T) is returned.
        /// </summary>
        public virtual bool ExceptionWhenNotFound { get; set; } = true;

        // For later version:
        // Option to convert '.' in key to directory path.
    }

}
