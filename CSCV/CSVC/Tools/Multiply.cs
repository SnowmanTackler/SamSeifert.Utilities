using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Multiply : Tool
    {
        private const int count = 2;
        private NodeHandleIn[] nhis = new NodeHandleIn[count];
        private NodeHandleOut nho = null;
        private Sect _SpecialBitmap;

        public Multiply()
        {
            InitializeComponent();

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBoxThumb = this.pictureBox1;

            int top = 0;

            for (int i = 0; i < count; i++)
            {
                var nhi = new NodeHandleIn(this);
                this.nhis[i] = nhi;
                this._Controls.Add(nhi);

                nhi.Location = this.Location;
                nhi.Left -= nhi.Width + ToolDefault.HandleGap / 2;
                nhi.Top += top;
                top += nhi.Height + ToolDefault.HandleGap;
            }

            top -= ToolDefault.HandleGap;
            top = (this.Height - top) / 2;
            foreach (NodeHandleIn n in this.nhis) n.Top += top;

            this.nho = new NodeHandleOut(this);
            this._Controls.Add(this.nho);
            this.nho.Location = this.Location;
            this.nho.Left += this.Width + ToolDefault.HandleGap;
            this.nho.Top += (this.Height - this.nho.Height) / 2;        
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.StatusChanged();
        }
















        public override Boolean ClearData(NodeHandleIn sender)
        {
            this._SpecialBitmap = null;
            this.UpdateThumb();
            return true;
        }

        public override Sect SpecialBitmapGet(NodeHandleOut sender)
        {
            return this._SpecialBitmap;
        }

        public override void SpecialBitmapUpdate()
        {
            try
            {
                var list = new List<Sect>();
                Sect a, b;

                a = this.nhis[0].getSpecialBitmap();
                if (a == null) return;
                list.Add(a);

                b = this.nhis[1].getSpecialBitmap();
                if (b == null) return;
                list.Add(b);

                IA_Multiple.Mult(list.ToArray(), ref this._SpecialBitmap);

            }
            finally
            {
                this.UpdateThumb();
            }
        }




        public override List<NodeHandleIn> getOutputs()
        {
            return this.nho.nhis;
        }
    }
}
