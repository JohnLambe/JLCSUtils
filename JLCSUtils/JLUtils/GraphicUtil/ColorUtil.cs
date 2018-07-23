using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.GraphicUtil
{
    /// <summary>
    /// Color-related utilities.
    /// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// 
        /// IMPLEMENTATION SUBJECT TO CHANGE.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>true iff the color is darker than 50% grey.</returns>
        public static bool IsDark(this Color color)
        {
            var l = color.B + color.G * 1.5 + color.R * 1.2;  //TODO Get correct weightings
            return l < (256 * (1 + 1.5 + 1.2)) / 2;
        }

        /// <summary>
        /// Returns a lightened version of a given color.
        /// The alpha level is unchanged.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="lightenAmount">How much lighter to make the color, in the range 0 (no change) to 1 (make white).</param>
        /// <returns></returns>
        public static Color Lighten(Color color, double lightenAmount)
        {
            return Color.FromArgb(color.A, LightenComponent(color.R, lightenAmount), LightenComponent(color.G, lightenAmount), LightenComponent(color.B, lightenAmount));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Value of an 8-bit color channel (0 - 255).</param>
        /// <param name="lightenAmount">How much lighter to make the color, in the range 0 (no change) to 1 (make white).</param>
        /// <returns></returns>
        public static int LightenComponent(int value, double lightenAmount)
        {
            return (int)((255 - value) * lightenAmount + value);
        }

        /// <summary>
        /// Convert a grey level in the range 0 to 255 to a <see cref="Color"/>.
        /// </summary>
        /// <param name="greyLevel"></param>
        /// <returns></returns>
        public static Color FromGrey(int greyLevel) => Color.FromArgb(greyLevel, greyLevel, greyLevel);

        /// <summary>
        /// Convert a hexadecimal color value to a <see cref="Color"/>.
        /// </summary>
        /// <param name="hexColor">
        /// Hexadecimal RGB or ARGB value, with 1 or 2 digits per channel,
        /// with or without a leading "#".
        ///     CURRENTLY SUPPORTS ONLY THE 2 DIGIT PER CHANNEL FORMAT.
        /// If null or "", <see cref="Color.Empty"/> is returned.
        /// <para>If no alpha value is supplied, the output color is opaque.</para>
        /// <para>HTML format hexadecimal values can be used.</para>
        /// <para>The order of the channels is: alpha, red, green, blue.</para>
        /// </param>
        /// <returns></returns>
        public static Color FromHex(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
            {
                return Color.Empty;
            }
            else
            {
                if (hexColor.StartsWith("#"))
                    hexColor = hexColor.Substring(1);
                if (hexColor.Length == 8)   // ARGB 8 bits per channel
                {
                    return Color.FromArgb(int.Parse(hexColor, System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                else if (hexColor.Length == 6)  // RGB 8 bits per channel
                {
                    return Color.FromArgb(int.Parse("FF" + hexColor, System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                /*
                else if (hexColor.Length == 3)  // RGB 4 bits per channel
                {
                    return Color.FromArgb(hexColor[0].HexValue * 0x10, hexColor[1].HexValue, hexColor[2].HexValue);
                }
                else if (hexColor.Length == 4)   // ARGB 4 bits per channel
                {
                    return Color.FromArgb(hexColor[0].HexValue * 0x10, hexColor[1].HexValue, hexColor[2].HexValue, hexColor[3].HexValue);
                }
                */
                else
                {
                    throw new ArgumentException("Invalid hexadecimal color value: " + hexColor);
                }
            }
        }
    }
}
