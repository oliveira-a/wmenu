namespace wmenu
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
            this.inputTxtBox = new System.Windows.Forms.TextBox();
            this.lblPrograms = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // inputTxtBox
            // 
            this.inputTxtBox.Location = new System.Drawing.Point(1, 3);
            this.inputTxtBox.Name = "inputTxtBox";
            this.inputTxtBox.Size = new System.Drawing.Size(179, 20);
            this.inputTxtBox.TabIndex = 0;
            this.inputTxtBox.TextChanged += new System.EventHandler(this.inputTxtBox_TextChanged);
            this.inputTxtBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.inputTxtBox_KeyUp);
            // 
            // lblPrograms
            // 
            this.lblPrograms.AutoSize = true;
            this.lblPrograms.Location = new System.Drawing.Point(186, 6);
            this.lblPrograms.Name = "lblPrograms";
            this.lblPrograms.Size = new System.Drawing.Size(35, 13);
            this.lblPrograms.TabIndex = 1;
            this.lblPrograms.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 26);
            this.ControlBox = false;
            this.Controls.Add(this.lblPrograms);
            this.Controls.Add(this.inputTxtBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTxtBox;
        private System.Windows.Forms.Label lblPrograms;
    }
}

