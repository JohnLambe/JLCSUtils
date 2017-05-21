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
using MvpFramework.Gemerator;

namespace MvpFramework.WinForms.Controls
{
    /// <summary>
    /// Control that is automatically populated from a model.
    /// </summary>
    [MvpBindChildren]
    public partial class GeneratorControl : UserControl, IControlBinder
    {
        public GeneratorControl()
        {
            InitializeComponent();
            Generate();
        }

        /// <summary>
        /// Generate controls from the model.
        /// </summary>
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

        /// <summary>
        /// Binder for the model of this control (from which controls are generated).
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("The object for which controls are generated.")]
        public virtual ModelBinderWrapper ModelBinder { get; set; }

        /// <summary>
        /// Set the model for this control (for use when the caller does not already have a <see cref="ModelBinderWrapper"/>).
        /// </summary>
        /// <seealso cref="ModelBinder"/>
        /// <param name="model"></param>
        public virtual void SetModel(object model)
        {
            ModelBinder = new ModelBinderWrapper(model);
        }

        /// <summary>
        /// Implements <see cref="IControlBinder.BindModel(ModelBinderWrapper, IPresenter)"/>.
        /// </summary>
        /// <param name="modelBinder"></param>
        /// <param name="presenter"></param>
        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            this.ModelBinder = modelBinder;
            Generate();
        }

        /// <summary>
        /// Implements <see cref="IControlBinder.MvpRefresh"/>.
        /// </summary>
        public virtual void MvpRefresh()
        {
        }

        /// <summary>
        /// Parent property of the properties that generated controls are bound to.
        /// (null or "" for the modl itself (root)).
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual string ModelProperty { get; set; }

    }
}
