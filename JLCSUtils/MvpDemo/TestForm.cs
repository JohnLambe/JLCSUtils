﻿using MvpFramework.WinForms.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpDemo
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ControlSorter.GetControlFullTabOrder(uiTextBox2_25));

            /*
            IEnumerable l = Controls;
            var q = l.AsQueryable();
            var qc = q.OfType<Control>();
            foreach (var c in ControlSorter.SortControls(qc))
            {
                Console.Out.WriteLine(c.Name + "  " + ControlSorter.GetControlFullTabOrder(c));
            }
            */
        }

        private void TestForm_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("KeyDown: " + e.KeyCode.ToString());
            e.Handled = true;
        }

        private void TestForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void TestForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MessageBox.Show("PreviewKeyDown: " + e.KeyCode.ToString());
        }
    }
}
