namespace ImageToolbox.Tools
{
    partial class ToolDefault
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
            this.checkBoxName = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxName
            // 
            this.checkBoxName.BackColor = System.Drawing.Color.Black;
            this.checkBoxName.Checked = true;
            this.checkBoxName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxName.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxName.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.checkBoxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.checkBoxName.ForeColor = System.Drawing.Color.White;
            this.checkBoxName.Location = new System.Drawing.Point(0, 0);
            this.checkBoxName.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxName.Name = "checkBoxName";
            this.checkBoxName.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.checkBoxName.Size = new System.Drawing.Size(128, 25);
            this.checkBoxName.TabIndex = 13;
            this.checkBoxName.Text = "Name";
            this.checkBoxName.UseVisualStyleBackColor = false;
            this.checkBoxName.CheckedChanged += new System.EventHandler(this.checkBoxName_CheckedChanged);
            // 
            // pictureBoxThumb
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(128, 128);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(128, 128);
            this.pictureBox1.Name = "pictureBoxThumb";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // ToolDefualt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.checkBoxName);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ToolDefualt";
            this.Size = new System.Drawing.Size(128, 153);
            this.Load += new System.EventHandler(this.ToolDefualt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.CheckBox checkBoxName;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
