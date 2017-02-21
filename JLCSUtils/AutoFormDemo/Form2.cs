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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new Form1();
            generator.Target = form;
            generator.Generate();
            form.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
//            generator = new AutoFormGenerator();
            generator.Coords = new Point(0, 4);
            generator.Model = new TestModel()
            {
                Name = "Joe Bloggs",
                Address = "12 Main Street"
            };

            generator.GenerateMappings();

        }

        protected AutoFormGenerator generator = new AutoFormGenerator();

        private void button3_Click(object sender, EventArgs e)
        {
            var generator = new AutoFormGenerator();
            generator.Coords = new Point(0, 4);
            generator.Model = new TestModel2()
            {
                Name = "Joe Bloggs",
                Address = "12 Main Street"
            };
            var form = new Form();
            generator.Target = form;

            generator.GenerateMappings();
            generator.Generate();
            form.Show();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var c = new Button() { Text = "button", MaximumSize = new System.Drawing.Size(300, 400), MinimumSize = new System.Drawing.Size(100, 100) };
            var f = new WrapperForm(c);
            //f.ShowDialog();
            //c.Visible = false;
            c.Visible = true;

        }
    }
}
