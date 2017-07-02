using JohnLambe.Util.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.WinForms
{
    public static class IconUtil
    {
        /// <summary>
        /// Assign the image of a control to the icon with the given ID from the provided repository (<paramref name="iconRepository"/>).
        /// </summary>
        /// <param name="image">The existing value of the image.</param>
        /// <param name="iconRepository">The icon repository for resolving values of <paramref name="iconId"/>.</param>
        /// <param name="iconId">The IconID (<see cref="IconIdAttribute"/>) to get the image by.</param>
        /// <returns>The new value of the image.</returns>
        public static Image GetImageByIconId(Image image, IIconRepository<string, Image> iconRepository, string iconId)
        {
            if (string.IsNullOrEmpty(iconId))
            {
                return null;
            }
            else
            {
                if (iconRepository != null)
                    return iconRepository.GetIcon(iconId);
            }
            return image;    // unmodified

            //TODO: Keep original image (probably set in the WinForms designer) until either the image or both the repository and Icon ID are set ?
        }
    }
}
