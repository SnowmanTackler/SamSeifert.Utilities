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
    public partial class NodeHandleIn : UserControl, NodeHandle
    {
        public readonly List<NodeHandleOut> _PreviousLevels = new List<NodeHandleOut>();

        public Block _Block;

        internal NodeHandleIn()
        {
            InitializeComponent();
        }

        public Dictionary<NodeData.DataType, NodeData> getData()
        {
            if (this._PreviousLevels.Count == 0) return null;
            var ls = new List<NodeData>();

            Boolean has_sect_holder = false;

            var ret = new Dictionary<NodeData.DataType, NodeData>();

            foreach (var nho in this._PreviousLevels)
            {
                if (nho._NodeData == null)
                {
                    return null;
                }
                else
                {
                    switch (nho._NodeData._Type)
                    {
                        case NodeData.DataType.Sect:
                            {
                                ls.Add(nho._NodeData);
                                has_sect_holder |= ((nho._NodeData as NodeDataSect)._Sect._Type == SectType.Holder);
                                break;
                            }
                        default:
                            {
                                if (ret.ContainsKey(nho._NodeData._Type)) return null;
                                else ret[nho._NodeData._Type] = nho._NodeData;
                                break;
                            }
                    }
                }
            }

            if (has_sect_holder)
            {
                if (ls.Count == 1)
                {
                    ret[NodeData.DataType.Sect] = ls.First();
                }
                else return null;
            }
            else
            {
                var sects = new Dictionary<SectType, Sect>();
                foreach (var nd in ls)
                {
                    var sect = (nd as NodeDataSect)._Sect;
                    if (sects.ContainsKey(sect._Type)) return null;
                    else sects[sect._Type] = sect;
                }
                try
                {
                    ret[NodeData.DataType.Sect] = new NodeDataSect(new SectHolder(sects.Values.ToArray()));
                }
                catch
                {
                    return null;
                }
            }

            return ret;
        }

        public NodeHandleIn(Tool t)
        {
            InitializeComponent();
        }

        internal NodeHandleIn(Block b)
        {
            InitializeComponent();
            this._Block = b;
        }

        internal void Connect(NodeHandleOut nho)
        {
            this._PreviousLevels.Add(nho);
            nho._NextLevels.Add(this);
            this._Block.UpdateNewData();
        }

        internal void Disconnect()
        {
            foreach (var nho in this._PreviousLevels) nho._NextLevels.Remove(this);
            this._PreviousLevels.Clear();
            this._Block.UpdateNewData();
        }

        public Point LocationCustom()
        {
            return new Point(
                this.Location.X + this.Width / 2,
                this.Location.Y + this.Height / 2);
        }

        public Boolean Contains(Point p)
        {
            return ((this.Left < p.X) &&
                    (p.X < this.Left + this.Width) &&
                    (this.Top < p.Y) &&
                    (p.Y < this.Top + this.Height));
        }

    }
}
