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
            var generator = CreateGenerator();

            generator.Target = generator.Target ?? this;   // default to this if not set in CreateGenerator()
            generator.Model = ModelBinder;
            generator.ModelProperty = this.ModelProperty;

            Clear();
            generator.Generate();
        }

        /// <summary>
        /// Remove all generated controls.
        /// </summary>
        public virtual void Clear()
        {
            Controls.Clear();
        }

        /// <summary>
        /// Create the generator object.
        /// <para>Override to use a different generator implemenation.</para>
        /// </summary>
        /// <returns>The generator instance to use.</returns>
        protected virtual FormGeneratorBase<Control> CreateGenerator()
        {
            return new FormGenerator();
        }

        /// <summary>
        /// Binder for the model of this control (from which controls are generated).
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("The object for which controls are generated.")]
        public virtual ModelBinderWrapper ModelBinder
        {
            get { return _modelBinder; }
            set
            {
                _modelBinder = value;
                if (AutoGenerate)
                    Generate();
            }
        }
        private ModelBinderWrapper _modelBinder;

        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("True to re-generate the controls on assigning the model, and on initial binding.")]
        [DefaultValue(true)]
        public virtual bool AutoGenerate { get; set; } = true;

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
            if(AutoGenerate)
                Generate();
        }

        /// <summary>
        /// Removes all generated controls within this control.
        /// </summary>
        public virtual void Reset()
        {
            Controls.Clear();
        }

        /// <summary>
        /// Implements <see cref="IControlBinder.MvpRefresh"/>.
        /// </summary>
        public virtual void MvpRefresh()
        {
        }

        [Description("Parent property of the properties that generated controls are bound to. "
            + "(null or \"\" for the model itself (root)).")]
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual string ModelProperty { get; set; }

        /* TODO:
        /// <summary>
        /// The lowest <see cref="MvpUiAttributeBase.Order"/> value for items to be included.
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual int StartOrder { get; set; } = int.MinValue;

        /// <summary>
        /// The highest <see cref="MvpUiAttributeBase.Order"/> value for items to be included.
        /// </summary>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        public virtual int EndOrder { get; set; } = int.MaxValue;
        */

    }
}
