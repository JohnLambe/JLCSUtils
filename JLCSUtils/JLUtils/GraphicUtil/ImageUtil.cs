using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using JohnLambe.Util.Types;

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
            if (image == null)
                return null;
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
        /// <returns>The binary data converted to an Image. null if <paramref name="data"/> is null.</returns>
        [return: Nullable]
        public static Image FromBytes([Nullable] byte[] data, bool useEmbeddedColorManagement = true, bool validateImageData = true)
        {
            if (data == null)
                return null;
            return Image.FromStream(new MemoryStream(data), useEmbeddedColorManagement, validateImageData);
        }

        /// <summary>
        /// Plot an image over part of another one (e.g. for adding a small icon in the corner of a larger one).
        /// </summary>
        /// <param name="sourceImage">The main image. This is modified by this method.</param>
        /// <param name="overlayImage">The image to be overlaid. If null, no image is overlaid.</param>
        /// <param name="alignment">Where to place the overlay.</param>
        /// <param name="overlaySize">Size of the overaly. Empty (<see cref="Size.IsEmpty"/>) to not scale it.</param>
        /// <returns><paramref name="sourceImage"/>, which is modified (not copied).</returns>
        public static Image AddOverlay([NotNull] Image sourceImage, [Nullable] Image overlayImage, ContentAlignment alignment = ContentAlignment.BottomRight, Size overlaySize = new Size())
        {
            sourceImage.ArgNotNull(nameof(sourceImage));

            if (overlaySize.IsEmpty)                      // if not scaling
                overlaySize = overlayImage.Size;          // use existing size
            var overlayRectangle = CoordinateUtil.PlaceRectangle(new Rectangle(0,0,sourceImage.Width,sourceImage.Height), overlayImage.Size, alignment);

            if (overlayImage != null)
            {
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
            }

            return sourceImage;
        }

        public static Image Transform([NotNull] Image sourceImage, IImageTransform transform)
        {
            sourceImage.ArgNotNull(nameof(sourceImage));
            Bitmap bm = new Bitmap(sourceImage);

            transform.ArgNotNull(nameof(transform)).Image = bm;

            Graphics g = Graphics.FromImage(sourceImage);
            try
            {
                for (int y = 0; y < bm.Width; y++)
                    for (int x = 0; x < bm.Width; x++)
                    {
                        Color? c = transform.CalculatePixel(x, y);
                        if (c != null)
                            bm.SetPixel(x, y, c.Value);
                    }
//                g.DrawImage(overlayImage, overlayRectangle.Left, overlayRectangle.Top, overlayRectangle.Width, overlayRectangle.Height);
            }
            finally
            {
                g.Dispose();
            }

            return sourceImage;
        }

    }

    public interface IImageTransform
    {
        Bitmap Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The color of the speicified pxiel, or null to leave it unchanged.</returns>
        Color? CalculatePixel(int x, int y);
    }

    public abstract class ImageTransformBase : IImageTransform
    {
        public Bitmap Image { get; set; }

        public abstract Color? CalculatePixel(int x, int y);

        /// <summary>
        /// Read a pixel in the source image.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The pixel Color, or Color.Transparent if out of bounds.</returns>
        protected Color GetPixel(int x, int y)
        {
            if ((x < 0) || (y > 0) || (x >= Image.Width) || (y >= Image.Height))
                return Color.Transparent;
            else
                return Image.GetPixel(x, y);
        }
    }

    /// <summary>
    /// Add a shadow to an image.
    /// Currently a basic hard shadow.
    /// </summary>
    public class ShadowTransform : ImageTransformBase
    {
        public ShadowTransform()
        {
        }
        public ShadowTransform(Color shadowColor)
        {
            ShadowColor = shadowColor;
        }

        public override Color? CalculatePixel(int x, int y)
        {
            if(GetPixel(x,y) == Color.Transparent)  // cast shadow on transparent pixels only. //TODO: Merge color on translucent pixels
            {
                Color sourceColor = Image.GetPixel(x - XOffset, y - YOffset);
                return Color.FromArgb(sourceColor.A, ShadowColor.R, ShadowColor.G, ShadowColor.B);  // shadow has same transparency as the pixel casting it
                //TODO: Merge transparency with ShadowColor
            }
            return null;
        }

        public Color ShadowColor { get; } = Color.Black;
        public int XOffset { get; } = 2;
        public int YOffset { get; } = 2;
        //TODO: public float Soften { get; }    // blur shadow
    }

}
