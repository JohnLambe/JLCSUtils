using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace JohnLambe.Util.GraphicUtil
{
    /// <summary>
    /// Utilities and extension methods related to <see cref="Image"/>.
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// Returns the image data in its raw format.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, image.RawFormat);
            return stream.GetBuffer();
        }

        /// <summary>
        /// Create an <see cref="Image"/> from a byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="useEmbeddedColorManagement"></param>
        /// <param name="validateImageData"></param>
        /// <returns></returns>
        public static Image FromBytes(byte[] data, bool useEmbeddedColorManagement = true, bool validateImageData = true)
        {
            return Image.FromStream(new MemoryStream(data), useEmbeddedColorManagement, validateImageData);
        }

        /// <summary>
        /// Plot an image over part of another one (e.g. for adding a small icon in the corner of a larger one).
        /// </summary>
        /// <param name="sourceImage">The main image. This is modified by this method.</param>
        /// <param name="overlayImage">The image to be overlaid.</param>
        /// <param name="alignment">Where to place the overlay.</param>
        /// <param name="overlaySize">Size of the overaly. Empty (<see cref="Size.IsEmpty"/>) to not scale it.</param>
        /// <returns><paramref name="sourceImage"/>, which is modified (not copied).</returns>
        public static Image AddOverlay(Image sourceImage, Image overlayImage, ContentAlignment alignment = ContentAlignment.BottomRight, Size overlaySize = new Size())
        {
            if (overlaySize.IsEmpty)                      // if not scaling
                overlaySize = overlayImage.Size;          // use existing size
            var overlayRectangle = CoordinateUtil.PlaceRectangle(new Rectangle(0,0,sourceImage.Width,sourceImage.Height), overlayImage.Size, alignment);

            Graphics g = Graphics.FromImage(sourceImage);
            try
            {
                g.DrawImage(overlayImage, overlayRectangle.Left, overlayRectangle.Top, overlayRectangle.Width, overlayRectangle.Height);
                // Bottom right:            //g.DrawImage(overlayImage, sourceImage.Width - overlayImage.Width, sourceImage.Height - overlayImage.Height);
            }
            finally
            {
                g.Dispose();
            }

            return sourceImage;
        }

    }

}
