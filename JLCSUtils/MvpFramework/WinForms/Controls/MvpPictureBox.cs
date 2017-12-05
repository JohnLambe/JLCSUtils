using JohnLambe.Util.Misc;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// A <see cref="PictureBox"/> that can populate icons from a repository, getting an IconId from the model, or using one provided on the control,
    /// and can get an Image from the model.
    /// </summary>
    public class MvpPictureBox : PictureBox
    {
#pragma warning disable CS0067   // Suppress 'Event never used'.  This is fired by reflection.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("Returns the name of the property on the model that contains the IconID of the image to be displayed in this control.\n"
            + "Don't use both this and " + nameof(OnGetImageProperty) + ".")]
        [MvpModelProperty(nameof(IconId))]
        public event GetNameDelegate OnGetIconIdProperty;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("Returns the name of the property on the model that contains the Image to be displayed in this control.")]
        [MvpModelProperty(nameof(Image))]
        public event GetNameDelegate OnGetImageProperty;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [MvpDefineValidatingEventAttribute]
        public event ValueChangedEventHandler ValidatingExt;  //| Or OnValidatingExt 

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [MvpDefineValidatedEventAttribute]
        public event ValueChangedEventHandler ValidatedExt;

        //TODO: Define what happens when both events are set.

#pragma warning restore CS0067

        #region Icon

        [Description("The IconId of the image to be displayed in this control. null of \"\" to make the icon blank.")]
        [IconId]
        public virtual string IconId
        {
            get { return _iconId; }
            set
            {
                _iconId = value;
                Image = IconUtil.GetImageByIconId(Image, IconRepository, IconId);
            }
        }
        private string _iconId;

        [Description("The repository to resolve IconId values.")]
        public virtual IIconRepository<string,Image> IconRepository
        {
            get { return _iconRepository; }
            set
            {
                _iconRepository = value;
                Image = IconUtil.GetImageByIconId(Image, IconRepository, IconId);
            }
        }
        private IIconRepository<string, Image> _iconRepository;

        #endregion

    }
}
