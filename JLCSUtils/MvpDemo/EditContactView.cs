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
using MvpFramework.WinForms;

namespace MvpDemo
{
    [View]
    public partial class EditContactView : WindowViewBase, IEditContactView
    {
        public EditContactView()
        {
            InitializeComponent();
            //Text = "Edit Contact Window";
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
//            MessageBox.Show("Validating " + (sender as Control).Name);
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
//            MessageBox.Show("Validated " + e.ToString() + " " + (sender as Control).Name);

        }

        private void EditContactView_ViewVisibilityChanged(object sender, ViewVisibilityChangedEventArgs args)
        {
//            MessageBox.Show("ViewVisibilityChanged");
        }

        private void textBox2_ModifiedChanged(object sender, EventArgs e)
        {
//            MessageBox.Show("ModifiedChanged " + (sender as Control).Name + " " + (sender as TextBoxBase).Modified);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Ok clicked");  // fires before Presenter
        }

        private void button3_Click(object sender, EventArgs e)
        {
            uiAddress.Modified = false;
        }
    }
}
