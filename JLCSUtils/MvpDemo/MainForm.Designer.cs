namespace MvpDemo
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.uiAutoView = new System.Windows.Forms.Button();
            this.uiNested = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.mvpTextBox1 = new MvpFramework.WinForms.Controls.MvpTextBox();
            this.contactBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mvpComboBox1 = new MvpFramework.WinForms.Controls.MvpComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).BeginInit();
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
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 249);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(601, 22);
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
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(156, 89);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(114, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Generator";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // uiAutoView
            // 
            this.uiAutoView.Location = new System.Drawing.Point(156, 118);
            this.uiAutoView.Name = "uiAutoView";
            this.uiAutoView.Size = new System.Drawing.Size(114, 23);
            this.uiAutoView.TabIndex = 9;
            this.uiAutoView.Text = "AutoView";
            this.uiAutoView.UseVisualStyleBackColor = true;
            this.uiAutoView.Click += new System.EventHandler(this.uiAutoView_Click);
            // 
            // uiNested
            // 
            this.uiNested.Location = new System.Drawing.Point(26, 89);
            this.uiNested.Name = "uiNested";
            this.uiNested.Size = new System.Drawing.Size(114, 23);
            this.uiNested.TabIndex = 10;
            this.uiNested.Text = "Launch Nested";
            this.uiNested.UseVisualStyleBackColor = true;
            this.uiNested.Click += new System.EventHandler(this.uiNested_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(26, 192);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(114, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "form";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(307, 64);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(292, 185);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(284, 159);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(284, 159);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(311, 35);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // mvpTextBox1
            // 
            this.mvpTextBox1.DataBindings.Add(new System.Windows.Forms.Binding("Lines", this.contactBindingSource, "Name", true));
            this.mvpTextBox1.Items = new string[] {
        "[Text<->Name|TextChanged=>Updated]"};
            this.mvpTextBox1.Location = new System.Drawing.Point(26, 154);
            this.mvpTextBox1.Name = "mvpTextBox1";
            this.mvpTextBox1.Size = new System.Drawing.Size(114, 20);
            this.mvpTextBox1.TabIndex = 5;
            this.mvpTextBox1.TextChanged += new System.EventHandler(this.mvpTextBox1_TextChanged);
            // 
            // contactBindingSource
            // 
            this.contactBindingSource.DataSource = typeof(MvpDemo.Model.Contact);
            // 
            // mvpComboBox1
            // 
            this.mvpComboBox1.FormattingEnabled = true;
            this.mvpComboBox1.Location = new System.Drawing.Point(149, 154);
            this.mvpComboBox1.Name = "mvpComboBox1";
            this.mvpComboBox1.Size = new System.Drawing.Size(121, 21);
            this.mvpComboBox1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(618, 269);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.uiNested);
            this.Controls.Add(this.uiAutoView);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mvpTextBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.mvpComboBox1);
            this.Controls.Add(this.btnEmbedded);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).EndInit();
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button uiAutoView;
        private System.Windows.Forms.Button uiNested;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox1;
    }
}