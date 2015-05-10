using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSCV_IDE
{
    public partial class BlockDefault : Block
    {
        public BlockDefault()
        {
            InitializeComponent();

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;

            this.pictureBoxThumb = this.pictureBox1;

            foreach (Control c in this.getStartControls())
            {
                this.Controls.Add(c);
                c.Dock = DockStyle.Top;
                c.BringToFront();
            }
        }

        public override void UpdateAddedToWorkspace()
        {
            base.UpdateAddedToWorkspace();
    
            if (this.usesStandardNodeHandelIn())
            {
                this.StandardNodeHandelIn = new NodeHandleIn(this);
                this._Inputs.Add(this.StandardNodeHandelIn);
                this.Parent.Controls.Add(this.StandardNodeHandelIn);
                const int h_gap = 4;
                this.StandardNodeHandelIn.Top = this.Top + (this.Height - this.StandardNodeHandelIn.Height) / 2;
                this.StandardNodeHandelIn.Left = this.Left - h_gap - this.StandardNodeHandelIn.Width;
            }
        }

        public NodeHandleIn StandardNodeHandelIn = null;
        public virtual bool usesStandardNodeHandelIn()
        {
            return true;
        }

        public virtual List<Control> getStartControls()
        {
            return new List<Control>();
        }

        private void checkBoxName_CheckedChanged(object sender, EventArgs e)
        {
            this.ClearFutures();
            this.UpdateNewData();
        }

    }
}
