using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.WinForms;
using MvpFramework;

namespace MvpDemo.Heirarchical
{
    public interface ITestHView : IView
    {
    }

    [View]
    public partial class TestHView : WindowViewBase, ITestHView
    {
        public TestHView()
        {
            InitializeComponent();
        }
    }
}
