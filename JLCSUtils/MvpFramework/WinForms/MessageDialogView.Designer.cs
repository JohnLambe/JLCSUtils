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
            this.uiMessageText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uiMessageText
            // 
            this.uiMessageText.AutoSize = true;
            this.uiMessageText.Location = new System.Drawing.Point(91, 50);
            this.uiMessageText.Name = "uiMessageText";
            this.uiMessageText.Size = new System.Drawing.Size(35, 13);
            this.uiMessageText.TabIndex = 0;
            this.uiMessageText.Tag = "[Dialog.Message]";
            this.uiMessageText.Text = "label1";
            // 
            // MessageDialogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uiMessageText);
            this.Name = "MessageDialogView";
            this.Size = new System.Drawing.Size(690, 278);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label uiMessageText;
    }
}
