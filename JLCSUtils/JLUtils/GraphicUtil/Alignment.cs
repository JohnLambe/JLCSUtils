using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.GraphicUtil
{
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
        /// <summary>
        /// Get the horiontal component of the <seealso cref="ContentAlignment"/>.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the vertical component of the <seealso cref="ContentAlignment"/>.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
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
