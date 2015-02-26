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
    public partial class SplitRGB : Tool
    {
        internal NodeHandleIn nhi;
        internal NodeHandleOut[] nhos = new NodeHandleOut[3];
        private const int count = 3;
        private readonly ImageData[] _SpecialBitmap = new ImageData[count];

        public SplitRGB()
        {
            InitializeComponent();

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.ContextMenuStrip = this.contextMenuStrip1;

            int top = 0;

            for (int i = 0; i < 3; i++)
            {
                var nho = new NodeHandleOut(this);
                this.nhos[i] = nho;
                this._Controls.Add(nho);

                if (i == 0) nho.BackgroundImage = Properties.Resources.ArrowR;
                else if (i == 1) nho.BackgroundImage = Properties.Resources.ArrowG;
                else if (i == 2) nho.BackgroundImage = Properties.Resources.ArrowB;

                nho.Location = this.Location;
                nho.Left += this.Width + ToolDefault.HandleGap / 2;
                nho.Top += top;
                top += nho.Height + ToolDefault.HandleGap;
            }

            top -= ToolDefault.HandleGap;
            top = (this.Height - top) / 2;
            foreach (NodeHandleOut n in this.nhos) n.Top += top;

            top = 0;
            var nhi = new NodeHandleIn(this);
            this.nhi = nhi;
            this._Controls.Add(nhi);
            nhi.Location = this.Location;
            nhi.Left -= nhi.Width + ToolDefault.HandleGap / 2;
            top += nhi.Height;
            top = (this.Height - top) / 2;
            nhi.Top += top;
        }















        public override Boolean ClearData(NodeHandleIn sender)
        {
            for (int i = 0; i < count; i++)
                this._SpecialBitmap[i] = null;

            return true;
        }

        public override void SpecialBitmapUpdate()
        {
            for (int i = 0; i < count; i++)
                this._SpecialBitmap[i] = null;
        }

        public override ImageData SpecialBitmapGet(NodeHandleOut sender)
        {
            ImageData t = null;

            if (this.nhi.nho != null)
                t = this.nhi.nho.getSpecialBitmap();

            if (t == null) return null;

            var rgb = ImageData.rgb;

            for (int cnt = 0; cnt < count; cnt++)
            {
                if (this.nhos[cnt] == sender)
                {
                    var st = rgb[cnt];
                    if (this._SpecialBitmap[cnt] == null && t.checkSect(st))
                    {
                        this._SpecialBitmap[cnt] = new ImageData(t);
                        this._SpecialBitmap[cnt].addSect(t.getSect(st, DataType.Read));
                    }
                    return this._SpecialBitmap[cnt];
                }
            }

            return null;
        }

        public override List<NodeHandleIn> getOutputs()
        {
            var list = new List<NodeHandleIn>();
            foreach (NodeHandleOut nho in this.nhos)
                list.AddRange(nho.nhis);
            return list;
        }
   }
}
