namespace GUI
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
            if (disposing && (components != null)) {
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
            this.mainFormOutput = new System.Windows.Forms.TextBox();
            this.urlInputBox = new System.Windows.Forms.TextBox();
            this.getAllocationsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainFormOutput
            // 
            this.mainFormOutput.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mainFormOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mainFormOutput.Location = new System.Drawing.Point(0, 28);
            this.mainFormOutput.Multiline = true;
            this.mainFormOutput.Name = "mainFormOutput";
            this.mainFormOutput.ReadOnly = true;
            this.mainFormOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mainFormOutput.Size = new System.Drawing.Size(770, 283);
            this.mainFormOutput.TabIndex = 1;
            // 
            // urlInputBox
            // 
            this.urlInputBox.Location = new System.Drawing.Point(0, 0);
            this.urlInputBox.Name = "urlInputBox";
            this.urlInputBox.Size = new System.Drawing.Size(670, 22);
            this.urlInputBox.TabIndex = 2;
            // 
            // getAllocationsButton
            // 
            this.getAllocationsButton.Location = new System.Drawing.Point(676, 0);
            this.getAllocationsButton.Name = "getAllocationsButton";
            this.getAllocationsButton.Size = new System.Drawing.Size(69, 27);
            this.getAllocationsButton.TabIndex = 3;
            this.getAllocationsButton.Text = "Go";
            this.getAllocationsButton.UseVisualStyleBackColor = true;
            this.getAllocationsButton.Click += new System.EventHandler(this.getAllocationsButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 311);
            this.Controls.Add(this.getAllocationsButton);
            this.Controls.Add(this.urlInputBox);
            this.Controls.Add(this.mainFormOutput);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox mainFormOutput;
        private System.Windows.Forms.TextBox urlInputBox;
        private System.Windows.Forms.Button getAllocationsButton;
    }
}

