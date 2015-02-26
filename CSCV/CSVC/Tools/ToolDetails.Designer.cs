namespace ImageToolbox.Tools
{
    partial class ToolDetails
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
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.labelInput = new System.Windows.Forms.Label();
            this.labelOutput = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.pictureBoxOut = new System.Windows.Forms.PictureBox();
            this.pictureBoxIn = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).BeginInit();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(200, 100);
            // 
            // labelInput
            // 
            this.labelInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInput.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInput.ForeColor = System.Drawing.Color.White;
            this.labelInput.Location = new System.Drawing.Point(10, 264);
            this.labelInput.Margin = new System.Windows.Forms.Padding(0);
            this.labelInput.Name = "labelInput";
            this.labelInput.Size = new System.Drawing.Size(250, 23);
            this.labelInput.TabIndex = 9;
            this.labelInput.Text = "Input";
            this.labelInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOutput
            // 
            this.labelOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOutput.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOutput.ForeColor = System.Drawing.Color.White;
            this.labelOutput.Location = new System.Drawing.Point(270, 264);
            this.labelOutput.Margin = new System.Windows.Forms.Padding(0);
            this.labelOutput.Name = "labelOutput";
            this.labelOutput.Size = new System.Drawing.Size(250, 23);
            this.labelOutput.TabIndex = 10;
            this.labelOutput.Text = "Output";
            this.labelOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.labelStatus);
            this.panel1.Controls.Add(this.pictureBoxOut);
            this.panel1.Controls.Add(this.pictureBoxIn);
            this.panel1.Controls.Add(this.labelOutput);
            this.panel1.Controls.Add(this.labelInput);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(530, 293);
            this.panel1.TabIndex = 11;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.labelStatus.ForeColor = System.Drawing.Color.Red;
            this.labelStatus.Location = new System.Drawing.Point(207, 264);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(111, 25);
            this.labelStatus.TabIndex = 48;
            this.labelStatus.Text = "Ghosting...";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxOut
            // 
            this.pictureBoxOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxOut.BackgroundImage = global::ImageToolbox.Properties.Resources.backImg;
            this.pictureBoxOut.Location = new System.Drawing.Point(270, 10);
            this.pictureBoxOut.Margin = new System.Windows.Forms.Padding(10, 10, 0, 10);
            this.pictureBoxOut.Name = "pictureBoxOut";
            this.pictureBoxOut.Size = new System.Drawing.Size(250, 250);
            this.pictureBoxOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxOut.TabIndex = 12;
            this.pictureBoxOut.TabStop = false;
            // 
            // pictureBoxIn
            // 
            this.pictureBoxIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxIn.BackgroundImage = global::ImageToolbox.Properties.Resources.backImg;
            this.pictureBoxIn.Location = new System.Drawing.Point(10, 10);
            this.pictureBoxIn.Margin = new System.Windows.Forms.Padding(10, 10, 0, 10);
            this.pictureBoxIn.Name = "pictureBoxIn";
            this.pictureBoxIn.Size = new System.Drawing.Size(250, 250);
            this.pictureBoxIn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxIn.TabIndex = 11;
            this.pictureBoxIn.TabStop = false;
            // 
            // ToolFullDefault
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel1);
            this.Name = "ToolFullDefault";
            this.Size = new System.Drawing.Size(530, 500);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.Label labelInput;
        private System.Windows.Forms.Label labelOutput;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.PictureBox pictureBoxOut;
        public System.Windows.Forms.PictureBox pictureBoxIn;
        private System.Windows.Forms.Label labelStatus;

    }
}
