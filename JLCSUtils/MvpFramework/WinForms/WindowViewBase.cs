using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    public partial class WindowViewBase : ViewBase, IWindowView
    {
        public WindowViewBase()
        {
            InitializeComponent();
        }

        public virtual void Close()
        {
            //TODO
        }

        public virtual object ShowModal()
        {
            return null;
        }
    }
}
