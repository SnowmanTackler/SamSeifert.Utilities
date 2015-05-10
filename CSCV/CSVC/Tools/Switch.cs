using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.CSCV;
using CSCV_IDE;

namespace CSCV_IDE.Tools
{
    public partial class ToolSwitch : Tool
    {
        private NodeHandleIn nhi = null;
        private NodeHandleOut nho = null;
        private List<NodeHandleIn> nhis = new List<NodeHandleIn>();

        public ToolSwitch()
        {
            InitializeComponent();

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBoxThumb = this.pictureBox1;

            var nho = new NodeHandleOut(this);
            this.nho = nho;
            this._Controls.Add(nho);
            nho.Location = this.Location;
            nho.Left += this.Width + ToolDefault.HandleGap;
            nho.Top += (this.Height - nho.Height) / 2;

            this.numericUpDown1_ValueChanged(this, EventArgs.Empty);
            this.numericUpDown2_ValueChanged(this, EventArgs.Empty);
        }

        private void allignNodes()
        {
            int top = 0;

            foreach (NodeHandleIn n in this.nhis)
            {
                n.Location = this.Location;
                n.Left -= n.Width + ToolDefault.HandleGap / 2;
                n.Top += this.Height - n.Height - top;
                top += n.Height + ToolDefault.HandleGap;
            }

            top -= ToolDefault.HandleGap;
            top = (this.Height - top) / 2;

//            foreach (NodeHandle n in this.nhis) n.Top -= top;

            FormMain.InvalidateWorkspace();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int count = (int)this.numericUpDown1.Value;

            while (this.nhis.Count < count)
            {
                NodeHandleIn nhi = new NodeHandleIn(this);
                this.nhis.Add(nhi);
                this._Controls.Add(nhi);

                Control c = this.Parent;
                if (c != null) c.Controls.Add(nhi);
            }

            while (this.nhis.Count > count)
            {
                NodeHandleIn nhi = this.nhis[this.nhis.Count - 1];
                this._Controls.Remove(nhi);
                this.nhis.Remove(nhi);
                nhi.Dispose();
            }

            this.allignNodes();

            this.numericUpDown2.Maximum = count;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            foreach (NodeHandleIn nhi in this.nhis)
                nhi.BackgroundImage = Properties.Resources.Arrow;

            this.nhi = this.nhis[(int)(this.numericUpDown2.Value) - 1];
            this.nhi.BackgroundImage = Properties.Resources.ArrowRB;

            this.StatusChanged();
        }


        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public override Boolean ClearData(NodeHandleIn sender)
        {
            if (sender != null && sender != this.nhi) return false;
            this.UpdateThumb();
            return true;
        }

        public override void SpecialBitmapUpdate()
        {
            this.UpdateThumb();
        }

        public override Sect SpecialBitmapGet(NodeHandleOut sender)
        {
//            if (this.nhi != null)
  //              if (this.nhi.nho != null)
    //                return this.nhi.nho.getSpecialBitmap();

            return null;
        }

        public override List<NodeHandleIn> getOutputs()
        {
            return null;
//            return this.nho.nhis;
        }
    }
}
