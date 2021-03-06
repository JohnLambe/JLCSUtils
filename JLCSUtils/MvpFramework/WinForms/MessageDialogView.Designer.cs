﻿namespace MvpFramework.WinForms
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
            this.uiDetails = new System.Windows.Forms.Button();
            this.uiMainPanel = new System.Windows.Forms.Panel();
            this.uiMessageText = new System.Windows.Forms.Label();
            this.uiIcon2 = new System.Windows.Forms.PictureBox();
            this.uiMainIcon = new System.Windows.Forms.PictureBox();
            this.uiButtons = new MvpFramework.WinForms.ButtonContainer();
            this.uiFullDetails = new System.Windows.Forms.TextBox();
            this.uiMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiIcon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiMainIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // uiDetails
            // 
            this.uiDetails.Location = new System.Drawing.Point(3, 131);
            this.uiDetails.Name = "uiDetails";
            this.uiDetails.Size = new System.Drawing.Size(50, 23);
            this.uiDetails.TabIndex = 5;
            this.uiDetails.Text = "Details";
            this.uiDetails.UseVisualStyleBackColor = true;
            this.uiDetails.Click += new System.EventHandler(this.uiDetails_Click);
            // 
            // uiMainPanel
            // 
            this.uiMainPanel.Controls.Add(this.uiMessageText);
            this.uiMainPanel.Controls.Add(this.uiIcon2);
            this.uiMainPanel.Controls.Add(this.uiMainIcon);
            this.uiMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiMainPanel.Location = new System.Drawing.Point(0, 0);
            this.uiMainPanel.Name = "uiMainPanel";
            this.uiMainPanel.Size = new System.Drawing.Size(474, 112);
            this.uiMainPanel.TabIndex = 4;
            // 
            // uiMessageText
            // 
            this.uiMessageText.BackColor = System.Drawing.Color.Transparent;
            this.uiMessageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiMessageText.Location = new System.Drawing.Point(70, 0);
            this.uiMessageText.Name = "uiMessageText";
            this.uiMessageText.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.uiMessageText.Size = new System.Drawing.Size(334, 112);
            this.uiMessageText.TabIndex = 4;
            this.uiMessageText.Tag = "[DetailMessage]";
            this.uiMessageText.Text = "Message text";
            // 
            // uiIcon2
            // 
            this.uiIcon2.BackColor = System.Drawing.Color.Transparent;
            this.uiIcon2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("uiIcon2.BackgroundImage")));
            this.uiIcon2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.uiIcon2.Dock = System.Windows.Forms.DockStyle.Right;
            this.uiIcon2.Location = new System.Drawing.Point(404, 0);
            this.uiIcon2.Margin = new System.Windows.Forms.Padding(10, 10, 3, 3);
            this.uiIcon2.Name = "uiIcon2";
            this.uiIcon2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.uiIcon2.Size = new System.Drawing.Size(70, 112);
            this.uiIcon2.TabIndex = 5;
            this.uiIcon2.TabStop = false;
            this.uiIcon2.Visible = false;
            // 
            // uiMainIcon
            // 
            this.uiMainIcon.BackColor = System.Drawing.Color.Transparent;
            this.uiMainIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("uiMainIcon.BackgroundImage")));
            this.uiMainIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.uiMainIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.uiMainIcon.Location = new System.Drawing.Point(0, 0);
            this.uiMainIcon.Margin = new System.Windows.Forms.Padding(10, 10, 3, 3);
            this.uiMainIcon.Name = "uiMainIcon";
            this.uiMainIcon.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.uiMainIcon.Size = new System.Drawing.Size(70, 112);
            this.uiMainIcon.TabIndex = 6;
            this.uiMainIcon.TabStop = false;
            // 
            // uiButtons
            // 
            this.uiButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.uiButtons.Buttons = null;
            this.uiButtons.ButtonsLayout = System.Windows.Forms.TabAlignment.Right;
            this.uiButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiButtons.Location = new System.Drawing.Point(0, 112);
            this.uiButtons.Name = "uiButtons";
            this.uiButtons.Size = new System.Drawing.Size(474, 48);
            this.uiButtons.TabIndex = 1;
            this.uiButtons.ButtonClicked += new MvpFramework.WinForms.ButtonContainer.ButtonClickedDelegate(this.uiButtons_ButtonClicked);
            // 
            // uiFullDetails
            // 
            this.uiFullDetails.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uiFullDetails.Location = new System.Drawing.Point(0, 160);
            this.uiFullDetails.Multiline = true;
            this.uiFullDetails.Name = "uiFullDetails";
            this.uiFullDetails.ReadOnly = true;
            this.uiFullDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uiFullDetails.Size = new System.Drawing.Size(474, 200);
            this.uiFullDetails.TabIndex = 9;
            this.uiFullDetails.Visible = false;
            // 
            // MessageDialogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ControlBox = false;
            this.Controls.Add(this.uiDetails);
            this.Controls.Add(this.uiMainPanel);
            this.Controls.Add(this.uiButtons);
            this.Controls.Add(this.uiFullDetails);
            this.Name = "MessageDialogView";
            this.Size = new System.Drawing.Size(474, 360);
            this.Resize += new System.EventHandler(this.MessageDialogView_Resize);
            this.uiMainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiIcon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiMainIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ButtonContainer uiButtons;
        private System.Windows.Forms.Panel uiMainPanel;
        private System.Windows.Forms.Label uiMessageText;
        private System.Windows.Forms.PictureBox uiIcon2;
        private System.Windows.Forms.PictureBox uiMainIcon;
        private System.Windows.Forms.Button uiDetails;
        private System.Windows.Forms.TextBox uiFullDetails;
    }
}
