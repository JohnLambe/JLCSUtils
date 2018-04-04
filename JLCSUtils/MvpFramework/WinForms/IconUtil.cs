using JohnLambe.Util.Misc;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Icon-related utilities.
    /// </summary>
    public static class IconUtil
    {
        /// <summary>
        /// Assign the image of a control to the icon with the given ID from the provided repository (<paramref name="iconRepository"/>).
        /// </summary>
        /// <param name="image">The existing value of the image.</param>
        /// <param name="iconRepository">The icon repository for resolving values of <paramref name="iconId"/>. (If null, <paramref name="image"/> is returned.)</param>
        /// <param name="iconId">The IconID (<see cref="IconIdAttribute"/>) to get the image by.</param>
        /// <returns>The new value of the image.</returns>
        [return: Nullable]
        public static Image GetImageByIconId([Nullable] Image image, [Nullable] IIconRepository<string, Image> iconRepository, [Nullable] string iconId)
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

        /// <summary>
        /// Set the icon of a control (for certain types of control).
        /// <para>
        /// This supports:<br/>
        /// - Controls attributed with <see cref="MvpBoundControlAttribute"/> (using its properties to specify how to assign the icon).<br/>
        /// - <see cref="ButtonBase"/>.
        /// </para>
        /// </summary>
        /// <param name="target">The control on which to set the icon.</param>
        /// <param name="iconId">The ID of the icon in <paramref name="repository"/>.</param>
        /// <param name="repository">The repository to get the icon from.</param>
        public static void SetIcon([NotNull] Control target, [Nullable] string iconId, IIconRepository<string,Image> repository = null)
        {
            if (repository != null)
            {
                var attribute = target.GetType().GetCustomAttribute<MvpBoundControlAttribute>();
                if (attribute != null)
                {
                    if (attribute?.IconIdProperty != null)                              // try to assign Icon ID
                        ReflectionUtil.TrySetPropertyValue(target, attribute.IconIdProperty, iconId);
                    if (attribute?.IconProperty != null && repository != null)          // if the Icon property is known, get the icon and assign the property
                    {
                        var icon = repository.GetIcon(iconId);
                        if (icon != null)                                                // if we have an icon
                            ReflectionUtil.TrySetPropertyValue(target, attribute.IconProperty, icon);
                    }
                }
                else
                {
                    if (target is ButtonBase)
                    {
                        var icon = repository.GetIcon(iconId);
                        if (icon != null)
                            ((ButtonBase)target).Image = icon;
                    }
                    /*
                    else if(target is Form)
                    {
                        convert repository.GetIcon(iconId) to Icon.
                        IconConverter
                        ((Form)target).Icon = ...
                    }
                    */
                }
            }
        }

    }
}
