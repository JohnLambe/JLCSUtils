namespace MvpFramework.WinForms
{
    partial class MessageDialogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageDialogView));
            this.uiMessageText = new System.Windows.Forms.Label();
            this.buttonContainer1 = new MvpFramework.WinForms.ButtonContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // uiMessageText
            // 
            this.uiMessageText.BackColor = System.Drawing.Color.Transparent;
            this.uiMessageText.Dock = System.Windows.Forms.DockStyle.Right;
            this.uiMessageText.Location = new System.Drawing.Point(76, 0);
            this.uiMessageText.Name = "uiMessageText";
            this.uiMessageText.Padding = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.uiMessageText.Size = new System.Drawing.Size(398, 108);
            this.uiMessageText.TabIndex = 0;
            this.uiMessageText.Tag = "[Dialog.Message]";
            this.uiMessageText.Text = "label1";
            // 
            // buttonContainer1
            // 
            this.buttonContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(240)))));
            this.buttonContainer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonContainer1.Filter = "DialogOptions";
            this.buttonContainer1.Location = new System.Drawing.Point(0, 108);
            this.buttonContainer1.Name = "buttonContainer1";
            this.buttonContainer1.Size = new System.Drawing.Size(474, 48);
            this.buttonContainer1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(10, 10, 3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox1.Size = new System.Drawing.Size(70, 108);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // MessageDialogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.uiMessageText);
            this.Controls.Add(this.buttonContainer1);
            this.Name = "MessageDialogView";
            this.Size = new System.Drawing.Size(474, 156);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label uiMessageText;
        private ButtonContainer buttonContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
