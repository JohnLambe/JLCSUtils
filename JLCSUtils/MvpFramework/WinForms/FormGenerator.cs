using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Binding;
using DiExtension;
using JohnLambe.Util;
using MvpFramework.Generator;
using System.Reflection;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Subclass of <see cref="FormGeneratorBase{T}"/> for WinForms.
    /// </summary>
    public class FormGenerator : FormGeneratorBase<Control>
    {
        public FormGenerator(ControlMappings<Control> mappings = null, IDiContext diContext = null)
            : base(mappings,diContext)
        {
        }

        public override void StartGenerate()
        {
            AddMapping(typeof(object), typeof(TextBox));  //XXX temporary

            base.StartGenerate();
            Target.SuspendLayout();
        }

        public override void EndGenerate()
        {
            Target.ResumeLayout();
            base.EndGenerate();
        }

        public override Control CreateGroup(IUiGroupModel parent, IUiGroupModel group)
        {
            var uiGroup = base.CreateGroup(parent, group);
            if (group.AutoGenerate)
            {
                var groupBox = new GroupBox();
                groupBox.Dock = DockStyle.Top;
                groupBox.Text = group.DisplayName;
                groupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                groupBox.AutoSize = true;

                /*
                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.Dock = DockStyle.Fill;
                groupBox.Controls.Add(panel);
                */

                Target.Controls.Add(groupBox);
                Target.Controls.SetChildIndex(groupBox, 0);

                return groupBox;
            }

            return uiGroup;
        }

        public override Control CreateControl(ControlGeneratorContext<Control> context)
        {
            base.CreateControl(context);

            return null;
        }

        protected override void AfterCreateControl(ControlGeneratorContext<Control> context)
        {
            var panel = new Panel();
            panel.Dock = DockStyle.Top;

            //TODO: Refactor this with a control (could be a subclass of Panel etc.) that combines the Label and Field (data) control,
            // and also handles validation (populates validation properties on Field Control,
            // and attaches an event handler for other validation).

            var captionControl = new Label();
            captionControl.Text = Accelerators.SetAccelerator(context.PropertyBinder.DisplayName,AcceleratorCaptionUtil.Auto);
            captionControl.Left = HSpacing;
            captionControl.Top = VSpacing / 2;
            panel.Controls.Add(captionControl);
//            panel.Controls.SetChildIndex(captionControl,0);

            //TODO: delegate to interface; implementation per control type (DataType mapped to control type; could be overridden by attribute).
            // e.g. bool mapped to CheckBox.
            var dataControl = context.NewControl; //new TextBox();
            dataControl.Left = FieldControlLeft;
            dataControl.Top = captionControl.Top;
            dataControl.Width = panel.Width - HSpacing - dataControl.Left;   // fill width of panel
            dataControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            dataControl.Tag = "" + GeneralControlBinder.TagPrefix + StrUtil.BlankPropagate(ModelProperty, ".") + context.PropertyBinder.Name + GeneralControlBinder.TagSuffix;

            panel.Height = dataControl.Bottom + VSpacing / 2;

            panel.TabIndex = context.Index;

            panel.Controls.Add(dataControl);

            context.ParentControl.Controls.Add(panel);
            context.ParentControl.Controls.SetChildIndex(panel, 0);
        }

        /// <summary>
        /// X-coordinate of field controls.
        /// </summary>
        protected virtual int FieldControlLeft { get; set; } = 200;

        /// <summary>
        /// Vertical spacing between controls, in pixels.
        /// </summary>
        protected const int VSpacing = 4;
        /// <summary>
        /// Horizontal padding.
        /// </summary>
        protected const int HSpacing = 4;
    }


}
