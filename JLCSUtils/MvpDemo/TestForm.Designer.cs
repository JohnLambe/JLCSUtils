namespace MvpDemo
{
    partial class TestForm
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
            this.uiPanel1 = new System.Windows.Forms.Panel();
            this.uiTextBox1_1 = new System.Windows.Forms.TextBox();
            this.uiButton1_2 = new System.Windows.Forms.Button();
            this.uiPanel2 = new System.Windows.Forms.Panel();
            this.uiTextBox2_25 = new System.Windows.Forms.TextBox();
            this.uiTextBox2_15 = new System.Windows.Forms.TextBox();
            this.uiTextBox_5 = new System.Windows.Forms.Button();
            this.uiTextBox0 = new System.Windows.Forms.TextBox();
            this.uiPanel1.SuspendLayout();
            this.uiPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiPanel1
            // 
            this.uiPanel1.Controls.Add(this.uiTextBox1_1);
            this.uiPanel1.Controls.Add(this.uiButton1_2);
            this.uiPanel1.Location = new System.Drawing.Point(12, 35);
            this.uiPanel1.Name = "uiPanel1";
            this.uiPanel1.Size = new System.Drawing.Size(260, 100);
            this.uiPanel1.TabIndex = 1;
            // 
            // uiTextBox1_1
            // 
            this.uiTextBox1_1.Location = new System.Drawing.Point(47, 15);
            this.uiTextBox1_1.Name = "uiTextBox1_1";
            this.uiTextBox1_1.Size = new System.Drawing.Size(100, 20);
            this.uiTextBox1_1.TabIndex = 10;
            // 
            // uiButton1_2
            // 
            this.uiButton1_2.Location = new System.Drawing.Point(47, 66);
            this.uiButton1_2.Name = "uiButton1_2";
            this.uiButton1_2.Size = new System.Drawing.Size(75, 23);
            this.uiButton1_2.TabIndex = 20;
            this.uiButton1_2.Text = "button1";
            this.uiButton1_2.UseVisualStyleBackColor = true;
            // 
            // uiPanel2
            // 
            this.uiPanel2.Controls.Add(this.uiTextBox2_25);
            this.uiPanel2.Controls.Add(this.uiTextBox2_15);
            this.uiPanel2.Location = new System.Drawing.Point(13, 142);
            this.uiPanel2.Name = "uiPanel2";
            this.uiPanel2.Size = new System.Drawing.Size(259, 74);
            this.uiPanel2.TabIndex = 2000;
            // 
            // uiTextBox2_25
            // 
            this.uiTextBox2_25.Location = new System.Drawing.Point(46, 40);
            this.uiTextBox2_25.Name = "uiTextBox2_25";
            this.uiTextBox2_25.Size = new System.Drawing.Size(100, 20);
            this.uiTextBox2_25.TabIndex = 25;
            // 
            // uiTextBox2_15
            // 
            this.uiTextBox2_15.Location = new System.Drawing.Point(46, 14);
            this.uiTextBox2_15.Name = "uiTextBox2_15";
            this.uiTextBox2_15.Size = new System.Drawing.Size(100, 20);
            this.uiTextBox2_15.TabIndex = 5;
            // 
            // uiTextBox_5
            // 
            this.uiTextBox_5.Location = new System.Drawing.Point(13, 227);
            this.uiTextBox_5.Name = "uiTextBox_5";
            this.uiTextBox_5.Size = new System.Drawing.Size(75, 23);
            this.uiTextBox_5.TabIndex = 1;
            this.uiTextBox_5.Text = "button2";
            this.uiTextBox_5.UseVisualStyleBackColor = true;
            this.uiTextBox_5.Click += new System.EventHandler(this.button2_Click);
            // 
            // uiTextBox0
            // 
            this.uiTextBox0.Location = new System.Drawing.Point(13, 9);
            this.uiTextBox0.Name = "uiTextBox0";
            this.uiTextBox0.Size = new System.Drawing.Size(100, 20);
            this.uiTextBox0.TabIndex = 2;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.uiTextBox0);
            this.Controls.Add(this.uiTextBox_5);
            this.Controls.Add(this.uiPanel2);
            this.Controls.Add(this.uiPanel1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TestForm_KeyDown);
            this.uiPanel1.ResumeLayout(false);
            this.uiPanel1.PerformLayout();
            this.uiPanel2.ResumeLayout(false);
            this.uiPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel uiPanel1;
        private System.Windows.Forms.Button uiButton1_2;
        private System.Windows.Forms.Panel uiPanel2;
        private System.Windows.Forms.Button uiTextBox_5;
        private System.Windows.Forms.TextBox uiTextBox1_1;
        private System.Windows.Forms.TextBox uiTextBox2_25;
        private System.Windows.Forms.TextBox uiTextBox2_15;
        private System.Windows.Forms.TextBox uiTextBox0;
    }
}