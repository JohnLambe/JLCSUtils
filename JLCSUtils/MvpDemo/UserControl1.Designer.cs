namespace MvpDemo
{
    partial class UserControl1
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
            this.mvpNestedViewPlaceholder1 = new MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mvpNestedViewPlaceholder1
            // 
            this.mvpNestedViewPlaceholder1.Location = new System.Drawing.Point(88, 33);
            this.mvpNestedViewPlaceholder1.Name = "mvpNestedViewPlaceholder1";
            this.mvpNestedViewPlaceholder1.Size = new System.Drawing.Size(200, 255);
            this.mvpNestedViewPlaceholder1.TabIndex = 0;
            this.mvpNestedViewPlaceholder1.ViewId = null;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(99, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.mvpNestedViewPlaceholder1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(332, 291);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder mvpNestedViewPlaceholder1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
