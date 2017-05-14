namespace MvpDemo
{
    partial class FormGeneratorDemo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiGenerator = new MvpFramework.WinForms.Controls.GeneratorControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uiGenerator
            // 
            this.uiGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiGenerator.AutoSize = true;
            this.uiGenerator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.uiGenerator.Location = new System.Drawing.Point(12, 12);
            this.uiGenerator.ModelBinder = null;
            this.uiGenerator.ModelProperty = null;
            this.uiGenerator.Name = "uiGenerator";
            this.uiGenerator.Size = new System.Drawing.Size(453, 56);
            this.uiGenerator.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 225);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormGeneratorDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 262);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.uiGenerator);
            this.Name = "FormGeneratorDemo";
            this.Text = "FormGeneratorDemo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MvpFramework.WinForms.Controls.GeneratorControl uiGenerator;
        private System.Windows.Forms.Button button1;
    }
}