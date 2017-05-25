using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Binding;
using MvpFramework.Generator;

namespace MvpFramework.WinForms
{
    public partial class AutoView : WindowViewBase, IAutoView
    {
        public AutoView()
        {
            InitializeComponent();
        }

        /*
        public override void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            base.Bind(model, presenter, binderFactory);
        }
        */

    }
}
