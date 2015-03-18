using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ImageToolbox;
using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class ToolDefault : Tool
    {
        private NodeHandleOut nhoTD = null;
        private NodeHandleIn nhiTD = null;

        public Sect _SpecialBitmap;

        private ToolDefault()
        {
            InitializeComponent();
        }

        protected internal ToolDefault(bool useOutput, bool useInput)
            : base()
        {
            InitializeComponent();

            if (this.DesignMode) return;

            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(base.dragStart);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(base.dragAction);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(base.dragEnd);
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBoxThumb = this.pictureBox1;

            if (useInput)
            {
                this.nhiTD = new NodeHandleIn(this);
                this.nhiTD.Left -= this.nhiTD.Width + ToolDefault.HandleGap / 2;
                this.nhiTD.Top += (this.Height - this.nhiTD.Height) / 2;
                this._Controls.Add(this.nhiTD);
            }

            if (useOutput)
            {
                this.nhoTD = new NodeHandleOut(this);
                this.nhoTD.Left += this.Width + ToolDefault.HandleGap / 2;
                this.nhoTD.Top += (this.Height - this.nhoTD.Height) / 2;
                this._Controls.Add(this.nhoTD);
            }
        }

        protected virtual List<Control> getStartControls()
        {
            return new List<Control>();
        }

























        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.MenuEdit();
        }

        private void ToolDefualt_Load(object sender, EventArgs e)
        {
            this.PerformLayout();

            int h = this.Height;

            foreach (Control c in this.getStartControls())
            {
                c.Width = this.Width;
                c.Location = new Point(0, h);
                h += c.Height;
                this.Controls.Add(c);
            }

            this.PerformLayout();

            if (this.nhiTD != null)
            {
                this.nhiTD.Location = this.Location;
                this.nhiTD.Left -= this.nhiTD.Width + ToolDefault.HandleGap / 2;
                this.nhiTD.Top += (this.Height - this.nhiTD.Height) / 2;
            }

            if (this.nhoTD != null)
            {
                this.nhoTD.Location = this.Location;
                this.nhoTD.Left += this.Width + ToolDefault.HandleGap / 2;
                this.nhoTD.Top += (this.Height - this.nhoTD.Height) / 2;
            }

            this.PerformLayout();
            this.Refresh();
        }

        protected internal void checkBoxName_CheckedChanged(object sender, EventArgs e)
        {
            this.StatusChanged();
        }






















        public virtual Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            return d;
        }

        public override void SpecialBitmapUpdate()
        {
            this._SpecialBitmap = null;

            var im = this.nhiTD.getSpecialBitmap();

            if (im != null)
            {
                if (this.checkBoxName.Checked)
                {
                    this._SpecialBitmap = this.SpecialBitmapUpdateDefault(ref im);
                }
                else this._SpecialBitmap = im;
            }

            this.UpdateThumb();
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

        public override List<NodeHandleIn> getOutputs()
        {
            if (this.nhoTD != null) return this.nhoTD.nhis;
            return new List<NodeHandleIn>();
        }
    }
}
