﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Binding;
using JohnLambe.Util;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// Control that is automatically populated from a model.
    /// </summary>
    public partial class GeneratorControl : UserControl
    {
        public GeneratorControl()
        {
            InitializeComponent();
            Generate();
        }

        public virtual void Generate()
        {
            var generator = new FormGenerator()
            {
                Target = this,
                Model = ModelBinder,
                ModelProperty = this.ModelProperty
            };
            generator.Generate();
        }

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("The object for which controls are generated.")]
        public virtual ModelBinderWrapper ModelBinder { get; set; }

        public virtual void SetModel(object model)
        {
            ModelBinder = new ModelBinderWrapper(model);
        }

        /// <summary>
        /// Parent property of the properties that generated controls are bound to.
        /// (null or "" for the modl itself (root)).
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual string ModelProperty { get; set; }

    }
}
