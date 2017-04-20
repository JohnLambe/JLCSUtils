namespace MvpDemo
{
    partial class EditContactView
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
            this.button1 = new System.Windows.Forms.Button();
            this.uiName = new System.Windows.Forms.TextBox();
            this.uiAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonContainer1 = new MvpFramework.WinForms.ButtonContainer();
            this.buttonContainer2 = new MvpFramework.WinForms.ButtonContainer();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(110, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Tag = "[Ok]";
            this.button1.Text = "&Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // uiName
            // 
            this.uiName.Location = new System.Drawing.Point(110, 23);
            this.uiName.Name = "uiName";
            this.uiName.Size = new System.Drawing.Size(156, 20);
            this.uiName.TabIndex = 1;
            this.uiName.Tag = "[Name]";
            this.uiName.Validating += new System.ComponentModel.CancelEventHandler(this.textBox2_Validating);
            this.uiName.Validated += new System.EventHandler(this.textBox2_Validated);
            // 
            // uiAddress
            // 
            this.uiAddress.Location = new System.Drawing.Point(110, 49);
            this.uiAddress.Name = "uiAddress";
            this.uiAddress.Size = new System.Drawing.Size(156, 20);
            this.uiAddress.TabIndex = 2;
            this.uiAddress.Tag = "[Address]";
            this.uiAddress.ModifiedChanged += new System.EventHandler(this.textBox2_ModifiedChanged);
            this.uiAddress.Validating += new System.ComponentModel.CancelEventHandler(this.textBox2_Validating);
            this.uiAddress.Validated += new System.EventHandler(this.textBox2_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Address:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(191, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Tag = "[Update]";
            this.button2.Text = "&Update";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // buttonContainer1
            // 
            this.buttonContainer1.BackColor = System.Drawing.Color.Silver;
            this.buttonContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.buttonContainer1.ButtonAlignment = System.Windows.Forms.HorizontalAlignment.Right;
            this.buttonContainer1.Buttons = null;
            this.buttonContainer1.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonContainer1.Filter = "RightPanel";
            this.buttonContainer1.Location = new System.Drawing.Point(302, 0);
            this.buttonContainer1.Name = "buttonContainer1";
            this.buttonContainer1.Size = new System.Drawing.Size(121, 174);
            this.buttonContainer1.TabIndex = 6;
            // 
            // buttonContainer2
            // 
            this.buttonContainer2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.buttonContainer2.Buttons = null;
            this.buttonContainer2.ButtonsLayout = System.Windows.Forms.TabAlignment.Left;
            this.buttonContainer2.ButtonSpacing = 4;
            this.buttonContainer2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonContainer2.Filter = null;
            this.buttonContainer2.Location = new System.Drawing.Point(0, 132);
            this.buttonContainer2.Margin = new System.Windows.Forms.Padding(10, 20, 3, 3);
            this.buttonContainer2.Name = "buttonContainer2";
            this.buttonContainer2.Size = new System.Drawing.Size(302, 42);
            this.buttonContainer2.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.CausesValidation = false;
            this.button3.Location = new System.Drawing.Point(15, 96);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // EditContactView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.buttonContainer2);
            this.Controls.Add(this.buttonContainer1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uiAddress);
            this.Controls.Add(this.uiName);
            this.Controls.Add(this.button1);
            this.Name = "EditContactView";
            this.Size = new System.Drawing.Size(423, 174);
            this.Title = "Edit Contact";
            this.ViewVisibilityChanged += new MvpFramework.ViewVisibilityChangedDelegate(this.EditContactView_ViewVisibilityChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox uiName;
        private System.Windows.Forms.TextBox uiAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private MvpFramework.WinForms.ButtonContainer buttonContainer1;
        private MvpFramework.WinForms.ButtonContainer buttonContainer2;
        private System.Windows.Forms.Button button3;
    }
}

