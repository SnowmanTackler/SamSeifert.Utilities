namespace SamSeifert.Utilities.Forms
{
    partial class ScreenShotSaverForm
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
            this.components = new System.ComponentModel.Container();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.labelDirectory = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbCaptureAuto = new System.Windows.Forms.RadioButton();
            this.rbCaptureClick = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.labelFileName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbOwn = new System.Windows.Forms.RadioButton();
            this.rbClipboard = new System.Windows.Forms.RadioButton();
            this.bSave = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDirectory
            // 
            this.labelDirectory.BackColor = System.Drawing.Color.White;
            this.labelDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDirectory.Location = new System.Drawing.Point(0, 0);
            this.labelDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.labelDirectory.Name = "labelDirectory";
            this.labelDirectory.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelDirectory.Size = new System.Drawing.Size(541, 23);
            this.labelDirectory.TabIndex = 0;
            this.labelDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDirectory.DoubleClick += new System.EventHandler(this.bBrowse_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00063F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00063F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelFileName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.bSave, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(541, 92);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbCaptureAuto);
            this.panel2.Controls.Add(this.rbCaptureClick);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(273, 36);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(129, 53);
            this.panel2.TabIndex = 11;
            // 
            // rbCaptureAuto
            // 
            this.rbCaptureAuto.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbCaptureAuto.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbCaptureAuto.Location = new System.Drawing.Point(0, 25);
            this.rbCaptureAuto.Name = "rbCaptureAuto";
            this.rbCaptureAuto.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.rbCaptureAuto.Size = new System.Drawing.Size(129, 25);
            this.rbCaptureAuto.TabIndex = 9;
            this.rbCaptureAuto.Text = "Auto (1 Hz)";
            this.rbCaptureAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbCaptureAuto.UseVisualStyleBackColor = true;
            this.rbCaptureAuto.CheckedChanged += new System.EventHandler(this.rbCaptureMode_CheckedChanged);
            // 
            // rbCaptureClick
            // 
            this.rbCaptureClick.Checked = true;
            this.rbCaptureClick.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbCaptureClick.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbCaptureClick.Location = new System.Drawing.Point(0, 0);
            this.rbCaptureClick.Name = "rbCaptureClick";
            this.rbCaptureClick.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.rbCaptureClick.Size = new System.Drawing.Size(129, 25);
            this.rbCaptureClick.TabIndex = 8;
            this.rbCaptureClick.TabStop = true;
            this.rbCaptureClick.Text = "Click Button";
            this.rbCaptureClick.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbCaptureClick.UseVisualStyleBackColor = true;
            this.rbCaptureClick.CheckedChanged += new System.EventHandler(this.rbCaptureMode_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(273, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 33);
            this.label3.TabIndex = 7;
            this.label3.Text = "Capture Mode:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(138, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "Image Source:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 33);
            this.label2.TabIndex = 2;
            this.label2.Text = "Next File:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown1.Location = new System.Drawing.Point(3, 69);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(129, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // labelFileName
            // 
            this.labelFileName.BackColor = System.Drawing.Color.White;
            this.labelFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFileName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFileName.Location = new System.Drawing.Point(3, 33);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(129, 33);
            this.labelFileName.TabIndex = 3;
            this.labelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbOwn);
            this.panel1.Controls.Add(this.rbClipboard);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(138, 36);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(129, 53);
            this.panel1.TabIndex = 10;
            // 
            // rbOwn
            // 
            this.rbOwn.Checked = true;
            this.rbOwn.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbOwn.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOwn.Location = new System.Drawing.Point(0, 25);
            this.rbOwn.Name = "rbOwn";
            this.rbOwn.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.rbOwn.Size = new System.Drawing.Size(129, 25);
            this.rbOwn.TabIndex = 9;
            this.rbOwn.TabStop = true;
            this.rbOwn.Text = "Capture Own";
            this.rbOwn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbOwn.UseVisualStyleBackColor = true;
            // 
            // rbClipboard
            // 
            this.rbClipboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbClipboard.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbClipboard.Location = new System.Drawing.Point(0, 0);
            this.rbClipboard.Name = "rbClipboard";
            this.rbClipboard.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.rbClipboard.Size = new System.Drawing.Size(129, 25);
            this.rbClipboard.TabIndex = 8;
            this.rbClipboard.Text = "Use Clipboard";
            this.rbClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbClipboard.UseVisualStyleBackColor = true;
            // 
            // bSave
            // 
            this.bSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bSave.Location = new System.Drawing.Point(408, 3);
            this.bSave.Name = "bSave";
            this.tableLayoutPanel1.SetRowSpan(this.bSave, 3);
            this.bSave.Size = new System.Drawing.Size(130, 86);
            this.bSave.TabIndex = 12;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.bSave_Click);
            // 
            // ScreenShotSaverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 115);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.labelDirectory);
            this.Name = "ScreenShotSaverForm";
            this.ShowIcon = false;
            this.Text = "Screen Capture";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScreenShotSaverForm_FormClosed);
            this.Load += new System.EventHandler(this.ScreenShotSaverForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label labelDirectory;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.RadioButton rbOwn;
        private System.Windows.Forms.RadioButton rbClipboard;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbCaptureAuto;
        private System.Windows.Forms.RadioButton rbCaptureClick;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Timer timer1;
    }
}

