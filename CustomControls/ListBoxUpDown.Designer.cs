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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bAdd = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.bRemove = new System.Windows.Forms.Button();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 151);
            this.panel1.TabIndex = 35;
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Margin = new System.Windows.Forms.Padding(0);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(266, 149);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.clb_SelectedIndexChanged);
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBoxUpDown_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.bAdd, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bDown, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.bUp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bRemove, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(268, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(32, 151);
            this.tableLayoutPanel1.TabIndex = 34;
            // 
            // bAdd
            // 
            this.bAdd.BackColor = System.Drawing.SystemColors.Control;
            this.bAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bAdd.Enabled = false;
            this.bAdd.Image = global::SamSeifert.Utilities.Properties.Resources.Plus;
            this.bAdd.Location = new System.Drawing.Point(3, 77);
            this.bAdd.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(29, 29);
            this.bAdd.TabIndex = 3;
            this.bAdd.UseVisualStyleBackColor = false;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bDown
            // 
            this.bDown.BackColor = System.Drawing.SystemColors.Control;
            this.bDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bDown.Enabled = false;
            this.bDown.Image = global::SamSeifert.Utilities.Properties.Resources.SpriteDown;
            this.bDown.Location = new System.Drawing.Point(3, 109);
            this.bDown.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(29, 42);
            this.bDown.TabIndex = 1;
            this.bDown.UseVisualStyleBackColor = false;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bUp
            // 
            this.bUp.BackColor = System.Drawing.SystemColors.Control;
            this.bUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bUp.Enabled = false;
            this.bUp.Image = global::SamSeifert.Utilities.Properties.Resources.SpriteUp;
            this.bUp.Location = new System.Drawing.Point(3, 0);
            this.bUp.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(29, 42);
            this.bUp.TabIndex = 0;
            this.bUp.UseVisualStyleBackColor = false;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bRemove
            // 
            this.bRemove.BackColor = System.Drawing.SystemColors.Control;
            this.bRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bRemove.Enabled = false;
            this.bRemove.Image = global::SamSeifert.Utilities.Properties.Resources.Error;
            this.bRemove.Location = new System.Drawing.Point(3, 45);
            this.bRemove.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bRemove.Name = "bRemove";
            this.bRemove.Size = new System.Drawing.Size(29, 29);
            this.bRemove.TabIndex = 2;
            this.bRemove.UseVisualStyleBackColor = false;
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateToolStripMenuItem,
            this.deleteAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // ListBoxUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ListBoxUpDown";
            this.Size = new System.Drawing.Size(300, 151);
            this.Load += new System.EventHandler(this.ListBoxUpDown_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBoxUpDown_KeyDown);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bRemove;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
    }
}
