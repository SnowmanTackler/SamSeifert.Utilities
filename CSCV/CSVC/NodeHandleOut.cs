using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.CSCV;
using CSCV_IDE.Tools;

namespace CSCV_IDE
{
    public partial class NodeHandleOut : UserControl, NodeHandle
    {
        private static NodeHandleOut _SelectedNodeHandleOut;

        private NodeData __NodeData = null;
        public NodeData _NodeData
        {
            get
            {
                return this.__NodeData;
            }
            set
            {
                if (value == null)
                {
                    this.__NodeData = null;
                }
                else switch (value._Type)
                {
                    case NodeData.DataType.Sect:
                        this.checkBox1.Text = EnumMethods.GetDescription((value as NodeDataSect)._Sect._Type);
                        if (this.checkBox1.Checked) FormMain.get().updateWithData((value as NodeDataSect)._Sect);
                        break;
                    default:
                        this.checkBox1.Text = value._Type.ToString();
                        break;
                }
                this.__NodeData = value;
            }
        }

        public readonly List<NodeHandleIn> _NextLevels = new List<NodeHandleIn>();

        public NodeHandleOut()
        {
            InitializeComponent();
        }

        public NodeHandleOut(Tool t)
        {
            InitializeComponent();
        }

        public void Connect(NodeHandleIn nhi)
        {
            nhi.Connect(this);
        }

        internal void Disconnect()
        {
            var hsb = new HashSet<Block>();
            foreach (var nho in this._NextLevels)
            {
                nho._PreviousLevels.Remove(this);
                hsb.Add(nho._Block);
            }

            this._NextLevels.Clear();
            foreach (Block b in hsb) b.UpdateNewData();
        }

        public Point LocationCustom()
        {
            return new Point(
                this.Location.X + this.Width - (this.Height / 2),
                this.Location.Y + this.Height / 2);
        }

        public Boolean Contains(Point p)
        {
            int r = this.Left + this.Width;
            int l = r - this.Height;
            return ((l < p.X) &&
                    (p.X < r) &&
                    (this.Top < p.Y) &&
                    (p.Y < this.Top + this.Height));
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (!this.checkBox1.Checked)
            {
                if (NodeHandleOut._SelectedNodeHandleOut != null)
                    NodeHandleOut._SelectedNodeHandleOut.checkBox1.Checked = false;

                NodeHandleOut._SelectedNodeHandleOut = this;
                this.checkBox1.Checked = true;

                if (this.__NodeData == null)
                {
                    FormMain.get().updateWithData(null);
                }
                else if (this.__NodeData._Type == NodeData.DataType.Sect)
                {
                    FormMain.get().updateWithData((this.__NodeData as NodeDataSect)._Sect);
                }
                else FormMain.get().updateWithData(null);
            }
        }
    }
}
