using System;
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
    public partial class FormGeneratorDemo : Form
    {
        public FormGeneratorDemo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uiGenerator.SetModel(Contact);
            uiGenerator.Generate();
        }

        protected Model.Contact Contact = new Model.Contact()
        {
            Name = "name",
            Address = "address"
        };
    }
}
