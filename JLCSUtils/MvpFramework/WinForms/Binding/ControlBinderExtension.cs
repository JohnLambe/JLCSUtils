using MvpFramework.Binding;
using MvpFramework.WinForms.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms.Binding
{
    public static class ControlBinderExtension
    {
        /// <summary>
        /// Get the bound control.
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        public static Control GetControl(this IControlBinderExt binder)
        {
            if (binder is IControlBinderExt)
                return ((IControlBinderExt)binder).BoundControl as Control;
            if (binder is Control)
                return binder as Control;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="control"></param>
        /// <returns>true if the control bound by this binder is <paramref name="control"/> or child (direct or indirect) of it.</returns>
        public static bool IsInControl(this IControlBinder binder, Control control)
        {
            if (binder is IControlBinderExt)
                return WinFormsUtil.IsInControl(GetControl(binder as IControlBinderExt), control);
            else
                return false;
        }
    }

}
