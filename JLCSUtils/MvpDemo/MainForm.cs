﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MvpFramework;
using MvpDemo.Model;
using MvpFramework.Generator;
using JohnLambe.Util.Exceptions;

namespace MvpDemo
{
    public partial class MainForm : Form
    {
        public MainForm(IPresenterFactory<IEditContactPresenter,Contact> editContactFactory,
            IPresenterFactory<Heirarchical.ITestHPresenter,TestHViewModel> hpresenterFactory,
            IPresenterFactory<Heirarchical.ITestLayoutPresenter, TestHViewModel> testLayoutPresenterFactory,
            IPresenterFactory<IAutoPresenter,object> autoPresenterFactory
            )
        {
            this.EditContactFactory = editContactFactory;
            this.HPresenterFactory = hpresenterFactory;
            this.AutoPresenterFactory = autoPresenterFactory;
            this.TestLayoutPresenterFactory = testLayoutPresenterFactory;

            InitializeComponent();


            extTabControl1.ChangeTabNumberKeys = MvpFramework.Util.NumberKeysType.MainZeroHigh | MvpFramework.Util.NumberKeysType.NumPadZeroHigh;
            extTabControl1.TabPages[2].Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var contact = new Contact()
            {
                Name = "name"
            };

            EditContactFactory.Create(contact).Show();

        }

        protected readonly IPresenterFactory<IEditContactPresenter,Contact> EditContactFactory;

        protected readonly IPresenterFactory<Heirarchical.ITestHPresenter, TestHViewModel> HPresenterFactory;
        protected readonly IPresenterFactory<Heirarchical.ITestLayoutPresenter, TestHViewModel> TestLayoutPresenterFactory;

        protected readonly IPresenterFactory<IAutoPresenter, object> AutoPresenterFactory;


        private void btnEmbedded_Click(object sender, EventArgs e)
        {
            var model = new TestHViewModel()
            {
                Contact = new Contact()
                {
                    Name = "Name 2",
                    Address = "Address 2"
                },
                Property1 = "Property 1 Value"
            };

            HPresenterFactory.Create(model).Show();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("checkBox1_CheckedChanged");
        }

        private string mvpTextBox1_GetModelProperty()
        {
            return nameof(this.BindingContext);
        }

        private void mvpTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new UserErrorException("Test");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FormGeneratorDemo().Show();
        }

        private void uiAutoView_Click(object sender, EventArgs e)
        {
            var model = new Contact()
            {
                Name = "name",
                PhoneNumber = "+353-1-12345",
                Comment = "This is a comment."
            };
            AutoPresenterFactory.Create(model).Show();

        }

        private void uiNested_Click(object sender, EventArgs e)
        {
            var model = new TestHViewModel()
            {
                Contact = new Contact()
                {
                    Name = "Name 2",
                    Address = "Address 2"
                },
                Property1 = "Property 1 Value"
            };

            //TestLayoutPresenterFactory.Create(model).Show();
            var presenter = TestLayoutPresenterFactory.Create(model);
            presenter.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new TestForm().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MvpFramework.WinForms.Util.WinFormsUtil.FocusControl(textBox1);
//            ScrollControlIntoView(textBox1);
        }
    }
}
