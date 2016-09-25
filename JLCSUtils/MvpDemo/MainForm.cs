using System;
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

namespace MvpDemo
{
    public partial class MainForm : Form
    {
        public MainForm(IPresenterFactory<IEditContactPresenter,Contact> editContactFactory)
        {
            this.EditContactFactory = editContactFactory;

            InitializeComponent();
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
    }
}
