using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFormDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            var generator = new AutoFormGenerator();
            generator.Coords = new Point(0, 4);
            generator.Target = this;
            generator.Model = new TestModel()
            {
                Name = "Joe Bloggs",
                Address = "12 Main Street"
            };

            generator.GenerateMappings();
            generator.Generate();
            */
        }
    }
}
