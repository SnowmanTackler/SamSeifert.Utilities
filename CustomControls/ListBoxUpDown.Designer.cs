namespace SamSeifert.Utilities.CustomControls
{
    partial class ListBoxUpDown
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bDown = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.bRemove = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.listBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(368, 200);
            this.panel2.TabIndex = 35;
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(366, 198);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.clb_SelectedValueChanged);
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBoxUpDown_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.bDown, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bUp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bRemove, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(368, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(32, 200);
            this.tableLayoutPanel1.TabIndex = 34;
            // 
            // bDown
            // 
            this.bDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bDown.Enabled = false;
            this.bDown.Image = global::SamSeifert.Utilities.Properties.Resources.SpriteDown;
            this.bDown.Location = new System.Drawing.Point(3, 117);
            this.bDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(29, 83);
            this.bDown.TabIndex = 1;
            this.bDown.UseVisualStyleBackColor = false;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bUp
            // 
            this.bUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bUp.Enabled = false;
            this.bUp.Image = global::SamSeifert.Utilities.Properties.Resources.SpriteUp;
            this.bUp.Location = new System.Drawing.Point(3, 0);
            this.bUp.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(29, 82);
            this.bUp.TabIndex = 0;
            this.bUp.UseVisualStyleBackColor = false;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bRemove
            // 
            this.bRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bRemove.Enabled = false;
            this.bRemove.Image = global::SamSeifert.Utilities.Properties.Resources.Error;
            this.bRemove.Location = new System.Drawing.Point(3, 85);
            this.bRemove.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(29, 29);
            this.bRemove.TabIndex = 2;
            this.bRemove.UseVisualStyleBackColor = false;
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // ListBoxUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.Name = "ListBoxUpDown";
            this.Size = new System.Drawing.Size(400, 200);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bRemove;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBox1;
    }
}
