using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JohnLambe.Util.Io;
using System.Drawing;
using JohnLambe.Util.ConfigProvider;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Loads (graphics) images from the filesystem, where the key is the image leaf name.
    /// </summary>
    public class FileImageLoaderConfigProvider : FileLoaderConfigProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="directory"><see cref="FileLoaderConfigProvider.Directory"/></param>
        /// <param name="supportedExtensions"><see cref="FileLoaderConfigProvider.Extensions"/></param>
        public FileImageLoaderConfigProvider(string directory, string[] supportedExtensions = null)
        {
            this.Directory = directory;
            this.Extensions = supportedExtensions ?? new string[] { "png", "jpg", "bmp", "ico", "gif" };
        }

        protected override T LoadFile<T>(string filename, Type requiredType)
        {
            return (T)((object)Image.FromFile(filename));
                // (Have to cast `Image` to `object` before casting to `T` because `T` may not be a reference type).
            /*
            if (!typeof(T).IsAssignableFrom(typeof(image)))
            {
                value = default(T);
                return false;
            }
            */
        }

        /*
        /// <summary>
        /// The pathname of the directory containing the image files.
        /// </summary>
        protected readonly string Directory;

        /// <summary>
        /// The prioritised list of extensions of image files supported.
        /// The image is loaded with the first of these that exists.
        /// <para>Supports <see cref="DirectoryUtil.Extension_Any"/>.</para>
        /// </summary>
        protected string[] Extensions { get; private set; }
        */
    }
}
