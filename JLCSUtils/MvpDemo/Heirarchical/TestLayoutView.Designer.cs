namespace MvpDemo.Heirarchical
{
    partial class TestLayoutView
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
            this.mvpNestedViewPlaceholder2 = new MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mvpNestedViewPlaceholder1
            // 
            this.mvpNestedViewPlaceholder1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mvpNestedViewPlaceholder1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.mvpNestedViewPlaceholder1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mvpNestedViewPlaceholder1.Location = new System.Drawing.Point(14, 87);
            this.mvpNestedViewPlaceholder1.Name = "mvpNestedViewPlaceholder1";
            this.mvpNestedViewPlaceholder1.Size = new System.Drawing.Size(504, 171);
            this.mvpNestedViewPlaceholder1.TabIndex = 0;
            this.mvpNestedViewPlaceholder1.ViewId = "Contact";
            // 
            // mvpNestedViewPlaceholder2
            // 
            this.mvpNestedViewPlaceholder2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mvpNestedViewPlaceholder2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.mvpNestedViewPlaceholder2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mvpNestedViewPlaceholder2.Location = new System.Drawing.Point(399, 3);
            this.mvpNestedViewPlaceholder2.Name = "mvpNestedViewPlaceholder2";
            this.mvpNestedViewPlaceholder2.Size = new System.Drawing.Size(119, 78);
            this.mvpNestedViewPlaceholder2.TabIndex = 1;
            this.mvpNestedViewPlaceholder2.ViewId = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(97, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Tag = "[Property1]";
            // 
            // TestLayoutView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.mvpNestedViewPlaceholder2);
            this.Controls.Add(this.mvpNestedViewPlaceholder1);
            this.Name = "TestLayoutView";
            this.Size = new System.Drawing.Size(536, 271);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder mvpNestedViewPlaceholder1;
        private MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder mvpNestedViewPlaceholder2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}
