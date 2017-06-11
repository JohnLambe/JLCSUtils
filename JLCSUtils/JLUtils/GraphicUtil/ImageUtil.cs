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
            var overlayRectangle = PlaceRectangle(new Rectangle(0,0,sourceImage.Width,sourceImage.Height), overlayImage.Size, alignment);

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

        public static Rectangle PlaceRectangle(Rectangle main, Size inset, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            int x = AlignInset(main.Width, inset.Width, alignment.HorizontalAlignment());
            int y = AlignInset(main.Height, inset.Height, alignment.VerticalAlignment());

            return new Rectangle(x + main.Left, y + main.Top, inset.Width, inset.Height);
        }

        public static int AlignInset(int outerSize, int innerSize, Alignment alignment = Alignment.Middle)
        {
            switch(alignment)
            {
                case Alignment.Start:
//                case ContentAlignment.BottomLeft:
//                case ContentAlignment.MiddleLeft:
//                case ContentAlignment.TopLeft:
                    return 0;    // left

                //                case ContentAlignment.BottomCenter:
                //                case ContentAlignment.MiddleCenter:
                //                case ContentAlignment.TopCenter:
                case Alignment.Middle:
                    return outerSize - innerSize / 2;

                //                case ContentAlignment.BottomRight:
                //                case ContentAlignment.MiddleRight:
                //                case ContentAlignment.TopRight:
                case Alignment.End:
                    return outerSize - innerSize;

                default:
                    throw new ArgumentException("Unrecognised Alignment");
            }
        }

    }

    /// <summary>
    /// Alignment of an item in one dimension.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Similar to System.Windows.Forms.HotizontalAlignment, except that this can be horizontal or vertical,
    /// and this avoids a deendency on WinForms.
    /// </para>
    /// <para>
    /// Similar to <see cref="System.Drawing.StringAlignment"/> except that that interpretation of this is generally
    /// not affected by the locale, and this may be vertical.
    /// </para>
    /// <para>
    /// <see cref="System.Drawing.ContentAlignment"/> has these options extended to two dimensions.
    /// </para>
    /// </remarks>
    public enum Alignment
    {
        /// <summary>
        /// Left or top.
        /// </summary>
        Start = 1,
        Middle,
        End
    }

    public static class AlignmentExtension
    {
        public static Alignment HorizontalAlignment(this ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    return Alignment.Start;    // left

                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    return Alignment.Middle;

                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    return Alignment.End;  // right

                default:
                    throw new ArgumentException("Unrecognised ContentAlignment");
            }
        }

        public static Alignment VerticalAlignment(this ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    return Alignment.Start;  // top

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    return Alignment.Middle;

                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    return Alignment.End;    // bottom

                default:
                    throw new ArgumentException("Unrecognised ContentAlignment");
            }
        }
    }
}
