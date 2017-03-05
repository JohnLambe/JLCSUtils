using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Util
{
    /// <summary>
    /// Utilities for use with WinForms.
    /// </summary>
    public static class WinFormsUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="parentControl"></param>
        /// <returns>true if this control is <paramref name="parentControl"/> or child (direct or indirect) of it.</returns>
        public static bool IsInControl(this Control control, Control parentControl)
        {
            var currentControl = control;
            do
            {
                if (currentControl == parentControl)
                    return true;                                // found - control is an ancestor of the bound control
                currentControl = currentControl.Parent;         // move up a level
            } while (currentControl != null);

            return false;                   // reached the topmost control without mathcing
        }

        /// <summary>
        /// Returns the rectangle of the area inside the margins of the control, in the coordinate system of the container of this control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns>the area inside the margins of the control.</returns>
        public static Rectangle InsideMarginsRectangle(this Control control)
        {
            return new Rectangle(control.Location.X + control.Margin.Left, control.Location.Y + control.Margin.Top,
                control.Width - control.Margin.Horizontal, control.Height - control.Margin.Vertical);
        }

        /// <summary>
        /// Returns the rectangle of the area inside the margins of the control, in the client coordinate system (relative to this control).
        /// </summary>
        /// <param name="control"></param>
        /// <returns>the area inside the margins of the control.</returns>
        public static Rectangle ClientInsideMarginsRectangle(this Control control)
        {
            return new Rectangle(control.ClientRectangle.Left + control.Margin.Left, control.ClientRectangle.Top + control.Margin.Top,
                control.ClientRectangle.Width - control.Margin.Horizontal, control.ClientRectangle.Height - control.Margin.Vertical);
        }

        /// <summary>
        /// Calculate the X coordinate relative to a container (or other area)
        /// to place an item of a specified width in it, with a given alignment.
        /// </summary>
        /// <param name="alignment">how to align the item within the container.</param>
        /// <param name="containerWidth">the width of the container, in pixels.</param>
        /// <param name="childWidth">the width of the item to be placed, in pixels.</param>
        /// <returns>the calcualted X coordinate (where 0 is the left of the container) of the item in the container so that it is aligned as specified.</returns>
        //| This could be useful outside of WinForms, but it takes a WinForms type as a parameter.
        public static int CalcAlignedPosition(HorizontalAlignment alignment, int containerWidth, int childWidth)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    return 0;
                case HorizontalAlignment.Right:
                    return containerWidth - childWidth;
                default:  // must be HorizontalAlignment.Center unless invalid
                    return (containerWidth - childWidth) / 2;   // half of the space around child is to the left
            }
        }

    }
}
