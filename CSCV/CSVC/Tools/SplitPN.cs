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
    public partial class SplitPN : Tool
    {
        private const int count = 2;
        internal NodeHandleIn nhi;
        private NodeHandleOut[] nhos = new NodeHandleOut[count];
        private readonly ImageData[] _SpecialBitmap = new ImageData[2];

        public SplitPN()
        {
            InitializeComponent();

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.ContextMenuStrip = this.contextMenuStrip1;

            int top = 0;

            for (int i = 0; i < count; i++)
            {
                var nho = new NodeHandleOut(this);
                this.nhos[i] = nho;
                this._Controls.Add(nho);

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

            var list = t.getSects(DataType.Read);

            int x, y, i;

            if (this.nhos[0] == sender)
            {
                if (this._SpecialBitmap[0] == null  && t.max > 0)
                {
                    this._SpecialBitmap[0] = new ImageData(t);

                    for (i = 0; i < list.Length; i++)
                    {
                        Sect s = list[i];
                        if (s.min >= 0) this._SpecialBitmap[0].addSect(s);
                        else if (s.max < 0) this._SpecialBitmap[0].addSect(new Sect(s._Type, s._Width, s._Height));
                        else
                        {
                            var d = new Single[s._Width, s._Height];

                            for (y = 0; y < s._Height; y++)
                            {
                                for (x = 0; x < s._Width; x++)
                                {
                                    d[y, x] = Math.Max(0, s._Data[y, x]);
                                }
                            }

                            this._SpecialBitmap[0].addSect(new Sect(d, s._Type));
                        }
                    }
                }
                return this._SpecialBitmap[0];
            }
            else if (this.nhos[1] == sender)
            {
                if (this._SpecialBitmap[1] == null && t.min < 0)
                {
                    this._SpecialBitmap[1] = new ImageData(t);

                    for (i = 0; i < list.Length; i++)
                    {
                        Sect s = list[i];
                        if (s.max <= 0) this._SpecialBitmap[1].addSect(s);
                        else if (s.min > 0) this._SpecialBitmap[1].addSect(new Sect(s._Type, s._Width, s._Height));
                        else
                        {
                            var d = new Single[s._Width, s._Height];

                            for (y = 0; y < s._Height; y++)
                            {
                                for (x = 0; x < s._Width; x++)
                                {
                                    d[y, x] = Math.Min(0, s._Data[y, x]);
                                }
                            }

                            this._SpecialBitmap[1].addSect(new Sect(d, s._Type));
                        }
                    }
                }
                return this._SpecialBitmap[1];
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
