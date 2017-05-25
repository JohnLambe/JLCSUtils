namespace MvpFramework.WinForms
{
    partial class AutoView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.generatorControl = new MvpFramework.WinForms.Controls.GeneratorControl();
            this.SuspendLayout();
            // 
            // generatorControl
            // 
            this.generatorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generatorControl.Location = new System.Drawing.Point(0, 0);
            this.generatorControl.ModelBinder = null;
            this.generatorControl.ModelProperty = null;
            this.generatorControl.Name = "generatorControl";
            this.generatorControl.Size = new System.Drawing.Size(495, 251);
            this.generatorControl.TabIndex = 0;
            // 
            // AutoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generatorControl);
            this.Name = "AutoView";
            this.Size = new System.Drawing.Size(495, 251);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GeneratorControl generatorControl;
    }
}
