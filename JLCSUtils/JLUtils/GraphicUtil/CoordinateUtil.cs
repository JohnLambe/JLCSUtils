using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.GraphicUtil
{
    public static class CoordinateUtil
    {
        /// <summary>
        /// Returns a rectangle of size <paramref name="inset"/> positioned within <paramref name="main"/>,
        /// aligned according to <paramref name="alignment"/>.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inset"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static Rectangle PlaceRectangle(Rectangle main, Size inset, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            int x = AlignInset(main.Width, inset.Width, alignment.HorizontalAlignment());
            int y = AlignInset(main.Height, inset.Height, alignment.VerticalAlignment());

            return new Rectangle(x + main.Left, y + main.Top, inset.Width, inset.Height);
        }

        /// <summary>
        /// Position an item of size <paramref name="innerSize"/> within an item of size <paramref name="outerSize"/> (in one dimension),
        /// aligned according to <paramref name="alignment"/>.
        /// </summary>
        /// <param name="outerSize"></param>
        /// <param name="innerSize"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        /// <seealso cref="PlaceRectangle(Rectangle, Size, ContentAlignment)"/>
        public static int AlignInset(int outerSize, int innerSize, Alignment alignment = Alignment.Middle)
        {
            switch (alignment)
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
}
