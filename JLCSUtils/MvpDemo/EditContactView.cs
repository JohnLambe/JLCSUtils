using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JohnLambe.Util;
using MvpFramework;

namespace MvpDemo
{
    public partial class EditContactView : ViewBase, IEditContactView
    {
        public EditContactView()
        {
            InitializeComponent();
        }

//        public event VoidDelegate OnOkClick;

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
