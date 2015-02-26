namespace ImageToolbox.Tools
{
    partial class Tool
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStrip_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStrip_Edit,
            this.ToolStrip_Delete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
            // 
            // ToolStrip_Edit
            // 
            this.ToolStrip_Edit.Enabled = false;
            this.ToolStrip_Edit.Name = "ToolStrip_Edit";
            this.ToolStrip_Edit.Size = new System.Drawing.Size(152, 22);
            this.ToolStrip_Edit.Text = "Edit";
            this.ToolStrip_Edit.Click += new System.EventHandler(this.ToolStrip_Edit_Click);
            // 
            // ToolStrip_Delete
            // 
            this.ToolStrip_Delete.Name = "ToolStrip_Delete";
            this.ToolStrip_Delete.Size = new System.Drawing.Size(152, 22);
            this.ToolStrip_Delete.Text = "Delete";
            this.ToolStrip_Delete.Click += new System.EventHandler(this.ToolStrip_Delete_Click);
            // 
            // Tool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Name = "Tool";
            this.Size = new System.Drawing.Size(5, 24);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ToolStripMenuItem ToolStrip_Edit;
        private System.Windows.Forms.ToolStripMenuItem ToolStrip_Delete;
        protected internal System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
