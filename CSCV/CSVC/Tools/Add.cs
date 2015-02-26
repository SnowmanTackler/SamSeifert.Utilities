using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;
using ImageToolbox;

namespace ImageToolbox.Tools
{
    public partial class Add : Tool
    {
        private Boolean[] signs = new Boolean[0];
        private List<NodeHandleIn> nhis = new List<NodeHandleIn>();
        private NodeHandleOut nho = null;
        private ImageData _SpecialBitmap;


        public Add()
        {
            InitializeComponent();

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBoxThumb = this.pictureBox1;

            this.nho = new NodeHandleOut(this);
            this._Controls.Add(this.nho);
            this.nho.Location = this.Location;
            this.nho.Left += this.Width + ToolDefault.HandleGap;
            this.nho.Top += (this.Height - this.nho.Height) / 2;

            this.textBox1_TextChanged(this, EventArgs.Empty);
        }

        private void allignNodes()
        {
            int top = 0;

            foreach (NodeHandleIn n in this.nhis)
            {
                n.Location = this.Location;
                n.Left -= n.Width + ToolDefault.HandleGap / 2;
                n.Top += top;
                top += n.Height + ToolDefault.HandleGap;
            }

            top -= ToolDefault.HandleGap;
            top = (this.Height - top) / 2;

            for (int i = 0; i < this.nhis.Count; i++)
            {
                this.nhis[i].Top += top;
                this.nhis[i].BackgroundImage.Dispose();
                this.nhis[i].BackgroundImage =
                    this.signs[i] ? Properties.Resources.ArrowR : Properties.Resources.ArrowRB;
            }

            FormMain.InvalidateWorkspace();
        }

        private bool textBox1_CustomChange = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1_CustomChange) return;
            this.textBox1_CustomChange = true;
            var l = this.textBox1.Text.ToCharArray();
            String s = "";
            var b = new List<Boolean>();
            foreach (char c in l)
            {
                if (c.Equals('+'))
                {
                    s += c;
                    b.Add(true);
                }
                if (c.Equals('-'))
                {
                    s += c;
                    b.Add(false);
                }
            }
            this.textBox1.Text = s;

            this.signs = b.ToArray();

            int count = this.signs.Length;

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

            this.textBox1_CustomChange = false;
        }















        public override Boolean ClearData(NodeHandleIn sender)
        {
            this._SpecialBitmap = null;
            this.UpdateThumb();
            return true;
        }

        public override ImageData SpecialBitmapGet(NodeHandleOut sender)
        {
            return this._SpecialBitmap;
        }

        public override void SpecialBitmapUpdate()
        {
            var indata = new List<ImageData>();
            var indataB = new List<Boolean>();

            for (int i = 0; i < this.nhis.Count; i++)
            {
                NodeHandleIn nhi = this.nhis[i];
                if (nhi.nho != null)
                {
                    ImageData t = nhi.nho.getSpecialBitmap();
                    if (t != null)
                    {
                        indata.Add(t);
                        indataB.Add(this.signs[i]);
                    }
                }
            }

            ImageAlgorithms.Add_O(
                indata.ToArray(),
                indataB.ToArray(),
                out this._SpecialBitmap);

            this.UpdateThumb();
        }

        public override List<NodeHandleIn> getOutputs()
        {
            return this.nho.nhis;
        }
    }
}
