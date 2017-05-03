﻿namespace MvpDemo
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.btnEmbedded = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.contactBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.mvpTextBox1 = new MvpFramework.WinForms.Controls.MvpTextBox();
            this.mvpComboBox1 = new MvpFramework.WinForms.Controls.MvpComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Launch";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnEmbedded
            // 
            this.btnEmbedded.Location = new System.Drawing.Point(26, 64);
            this.btnEmbedded.Name = "btnEmbedded";
            this.btnEmbedded.Size = new System.Drawing.Size(114, 23);
            this.btnEmbedded.TabIndex = 2;
            this.btnEmbedded.Text = "Launch Embedded";
            this.btnEmbedded.UseVisualStyleBackColor = true;
            this.btnEmbedded.Click += new System.EventHandler(this.btnEmbedded_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(149, 181);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.ThreeState = true;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // contactBindingSource
            // 
            this.contactBindingSource.DataSource = typeof(MvpDemo.Model.Contact);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 240);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = this.contactBindingSource;
            this.bindingSource1.CurrentChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(156, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Exception";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // mvpTextBox1
            // 
            this.mvpTextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Lines", this.contactBindingSource, "Name", true));
            this.mvpTextBox1.Items = new string[] {
        "[Text<->Name|TextChanged=>Updated]"};
            this.mvpTextBox1.Location = new System.Drawing.Point(26, 154);
            this.mvpTextBox1.ModelProperty = "Name;=>Updated";
            this.mvpTextBox1.Name = "mvpTextBox1";
            this.mvpTextBox1.Size = new System.Drawing.Size(114, 20);
            this.mvpTextBox1.TabIndex = 5;
            this.mvpTextBox1.TypeProperty = null;
            this.mvpTextBox1.TextChanged += new System.EventHandler(this.mvpTextBox1_TextChanged);
            // 
            // mvpComboBox1
            // 
            this.mvpComboBox1.FormattingEnabled = true;
            this.mvpComboBox1.Location = new System.Drawing.Point(149, 154);
            this.mvpComboBox1.ModelProperty = null;
            this.mvpComboBox1.Name = "mvpComboBox1";
            this.mvpComboBox1.Size = new System.Drawing.Size(121, 21);
            this.mvpComboBox1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mvpTextBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.mvpComboBox1);
            this.Controls.Add(this.btnEmbedded);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnEmbedded;
        private MvpFramework.WinForms.Controls.MvpComboBox mvpComboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private MvpFramework.WinForms.Controls.MvpTextBox mvpTextBox1;
        private System.Windows.Forms.BindingSource contactBindingSource;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Button button2;
    }
}