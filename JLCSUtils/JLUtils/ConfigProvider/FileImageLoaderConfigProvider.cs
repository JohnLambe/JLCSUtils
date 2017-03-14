using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JohnLambe.Util.Io;
using System.Drawing;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Loads (graphics) images from the filesystem, where the key is the image leaf name.
    /// </summary>
    public class FileImageLoaderConfigProvider : IConfigProvider
    {
        public FileImageLoaderConfigProvider(string directory, string[] supportedExtensions = null)
        {
            this.ImageDirectory = directory;
            this.SupportedExtensions = supportedExtensions ?? new string[] { "png", "jpg", "bmp" };
        }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            try
            {
                object image = Image.FromFile(DirectoryUtil.GetFullFilename(Path.Combine(ImageDirectory, key + "."), SupportedExtensions));
                    // (Have to cast `Image` to `object` before casting to `T`).
                /*
                if (!typeof(T).IsAssignableFrom(typeof(image)))
                {
                    value = default(T);
                    return false;
                }
                */
                value = (T)image;
                return true;
            }
            catch(IOException e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
            {   // not found
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// The pathname of the directory containing the image files.
        /// </summary>
        protected readonly string ImageDirectory;

        /// <summary>
        /// The prioritised list of extensions of image files supported.
        /// The image is loaded with the first of these that exists.
        /// </summary>
        protected string[] SupportedExtensions { get; private set; }
    }
}
