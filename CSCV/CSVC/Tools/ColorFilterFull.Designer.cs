namespace CSCV_IDE.Tools
{
    partial class ColorFilterFull
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
            this.hueChooser1 = new SamSeifert.HueChooser.HueChooser();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).BeginInit();
            this.SuspendLayout();
            // 
            // hueChooser1
            // 
            this.hueChooser1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hueChooser1.BackColor = System.Drawing.SystemColors.Control;
            this.hueChooser1.Location = new System.Drawing.Point(10, 303);
            this.hueChooser1.Margin = new System.Windows.Forms.Padding(10);
            this.hueChooser1.Name = "hueChooser1";
            this.hueChooser1.Size = new System.Drawing.Size(510, 142);
            this.hueChooser1.TabIndex = 8;
            this.hueChooser1.ValueChanged += new SamSeifert.HueChooser.ValueChangedEventHandler(this.hueChooser1_ValueChanged);
            // 
            // ToolColorFilterFull
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hueChooser1);
            this.Name = "ToolColorFilterFull";
            this.Controls.SetChildIndex(this.hueChooser1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SamSeifert.HueChooser.HueChooser hueChooser1;
    }
}
