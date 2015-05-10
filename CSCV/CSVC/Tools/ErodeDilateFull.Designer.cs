namespace CSCV_IDE.Tools
{
    partial class ErodeDilateFull
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
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownCycles = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownC = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownR = new System.Windows.Forms.NumericUpDown();
            this.radioButtonE = new System.Windows.Forms.RadioButton();
            this.radioButtonD = new System.Windows.Forms.RadioButton();
            this.radioButtonO = new System.Windows.Forms.RadioButton();
            this.radioButtonC = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(395, 469);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Cycles:";
            // 
            // numericUpDownCycles
            // 
            this.numericUpDownCycles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCycles.BackColor = System.Drawing.Color.White;
            this.numericUpDownCycles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.numericUpDownCycles.ForeColor = System.Drawing.Color.Black;
            this.numericUpDownCycles.Location = new System.Drawing.Point(454, 468);
            this.numericUpDownCycles.Margin = new System.Windows.Forms.Padding(10);
            this.numericUpDownCycles.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownCycles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCycles.Name = "numericUpDownCycles";
            this.numericUpDownCycles.Size = new System.Drawing.Size(78, 20);
            this.numericUpDownCycles.TabIndex = 34;
            this.numericUpDownCycles.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownCycles.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCycles.ValueChanged += new System.EventHandler(this.numericUpDownCycles_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(127, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Columns:";
            // 
            // numericUpDownC
            // 
            this.numericUpDownC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDownC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.numericUpDownC.Location = new System.Drawing.Point(188, 301);
            this.numericUpDownC.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.numericUpDownC.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownC.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownC.Name = "numericUpDownC";
            this.numericUpDownC.Size = new System.Drawing.Size(62, 20);
            this.numericUpDownC.TabIndex = 30;
            this.numericUpDownC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownC.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownC.ValueChanged += new System.EventHandler(this.numericUpDownC_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(6, 303);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Rows:";
            // 
            // numericUpDownR
            // 
            this.numericUpDownR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDownR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.numericUpDownR.Location = new System.Drawing.Point(54, 301);
            this.numericUpDownR.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.numericUpDownR.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownR.Name = "numericUpDownR";
            this.numericUpDownR.Size = new System.Drawing.Size(62, 20);
            this.numericUpDownR.TabIndex = 28;
            this.numericUpDownR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownR.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownR.ValueChanged += new System.EventHandler(this.numericUpDownR_ValueChanged);
            // 
            // radioButtonE
            // 
            this.radioButtonE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonE.AutoSize = true;
            this.radioButtonE.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioButtonE.Location = new System.Drawing.Point(479, 369);
            this.radioButtonE.Name = "radioButtonE";
            this.radioButtonE.Size = new System.Drawing.Size(53, 17);
            this.radioButtonE.TabIndex = 36;
            this.radioButtonE.Text = "Erode";
            this.radioButtonE.UseVisualStyleBackColor = true;
            this.radioButtonE.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonD
            // 
            this.radioButtonD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonD.AutoSize = true;
            this.radioButtonD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioButtonD.Location = new System.Drawing.Point(474, 392);
            this.radioButtonD.Name = "radioButtonD";
            this.radioButtonD.Size = new System.Drawing.Size(58, 17);
            this.radioButtonD.TabIndex = 37;
            this.radioButtonD.Text = "Dialate";
            this.radioButtonD.UseVisualStyleBackColor = true;
            this.radioButtonD.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonO
            // 
            this.radioButtonO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonO.AutoSize = true;
            this.radioButtonO.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioButtonO.Checked = true;
            this.radioButtonO.Location = new System.Drawing.Point(481, 415);
            this.radioButtonO.Name = "radioButtonO";
            this.radioButtonO.Size = new System.Drawing.Size(51, 17);
            this.radioButtonO.TabIndex = 38;
            this.radioButtonO.TabStop = true;
            this.radioButtonO.Text = "Open";
            this.radioButtonO.UseVisualStyleBackColor = true;
            this.radioButtonO.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonC
            // 
            this.radioButtonC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonC.AutoSize = true;
            this.radioButtonC.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioButtonC.Location = new System.Drawing.Point(481, 438);
            this.radioButtonC.Name = "radioButtonC";
            this.radioButtonC.Size = new System.Drawing.Size(51, 17);
            this.radioButtonC.TabIndex = 39;
            this.radioButtonC.Text = "Close";
            this.radioButtonC.UseVisualStyleBackColor = true;
            this.radioButtonC.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // ToolErodeDilateFull
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioButtonC);
            this.Controls.Add(this.radioButtonO);
            this.Controls.Add(this.radioButtonD);
            this.Controls.Add(this.radioButtonE);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownCycles);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownR);
            this.Name = "ToolErodeDilateFull";
            this.Controls.SetChildIndex(this.numericUpDownR, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.numericUpDownC, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.numericUpDownCycles, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.radioButtonE, 0);
            this.Controls.SetChildIndex(this.radioButtonD, 0);
            this.Controls.SetChildIndex(this.radioButtonO, 0);
            this.Controls.SetChildIndex(this.radioButtonC, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownCycles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownR;
        private System.Windows.Forms.RadioButton radioButtonE;
        private System.Windows.Forms.RadioButton radioButtonD;
        private System.Windows.Forms.RadioButton radioButtonO;
        private System.Windows.Forms.RadioButton radioButtonC;
    }
}
