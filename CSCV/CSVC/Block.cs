using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SamSeifert.CSCV;

namespace CSCV_IDE
{
    public class Block : UserControl
    {
        public Boolean _AddedToWorkspace = false;
        private System.ComponentModel.IContainer components = null;
        public ToolStripMenuItem ToolStrip_Edit;
        private ToolStripMenuItem ToolStrip_Delete;
        public ContextMenuStrip contextMenuStrip1;

        public Block()
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
            this.Name = "Block";
            this.Size = new System.Drawing.Size(5, 24);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void ToolStrip_Edit_Click(object sender, EventArgs e)
        {
            this.MenuEdit();
        }

        public virtual void MenuEdit() { }
    
        private void ToolStrip_Delete_Click(object sender, EventArgs e)
        {
            this.MenuDelete();
        }

        public virtual void MenuDelete()
        {
            var hsb = new HashSet<Block>();

            foreach (var c in this._Inputs)
            {
                foreach (var nho in c._PreviousLevels)
                {
                    nho._NextLevels.Remove(c);
                }
                c._PreviousLevels.Clear();
                c._Block = null;
                c.Parent.Controls.Remove(c);
            }

            this._Inputs.Clear();

            foreach (var c in this._Outputs.Values)
            {
                foreach (var nho in c._NextLevels)
                {
                    nho._PreviousLevels.Remove(c);
                    hsb.Add(nho._Block);
                }
                c._NextLevels.Clear();
                c._NodeData = null;
                c.Parent.Controls.Remove(c);
            }

            this._Outputs.Clear();

            this.Parent.Controls.Remove(this);

            foreach (var b in hsb) b.UpdateNewData();
        }

        private Boolean dragging = false;
        private Point DragStartPoint = new Point();

        protected internal void dragStart(object sender, MouseEventArgs e)
        {
            this.DragStartPoint = new Point(e.X, e.Y);

            foreach (var c in this._Inputs) c.BringToFront();
            foreach (var c in this._Outputs.Values) c.BringToFront();
            this.BringToFront();

            this.dragging = true;
        }

        protected internal void dragAction(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.move(e.X - this.DragStartPoint.X, e.Y - this.DragStartPoint.Y);
                FormMain.InvalidateWorkspace();
            }
        }

        protected internal void dragEnd(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;
            }
        }

        internal void move(int x, int y)
        {
            foreach (var c in this._Inputs)
            {
                c.Left += x;
                c.Top += y;
            }
            foreach (var c in this._Outputs.Values)
            {
                c.Left += x;
                c.Top += y;
            }
            this.Left += x;
            this.Top += y;
        }

        internal void moveTo(Point p)
        {
            this.moveTo(p.X, p.Y);
        }

        internal void moveTo(int x, int y)
        {
            var lc = this.LocationCustom();
            this.move(x - lc.X, y - lc.Y);
        }

        private Point LocationCustom()
        {
            return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
        }












        public readonly List<NodeHandleIn> _Inputs = new List<NodeHandleIn>();
        private readonly Dictionary<NodeHandleKey, NodeHandleOut> _Outputs = new Dictionary<NodeHandleKey, NodeHandleOut>();

        public virtual void UpdateAddedToWorkspace()
        {
            this._AddedToWorkspace = true;
        }

        public virtual void ClearFutures()
        {

        }

        public virtual void UpdateNewData()
        {
        }

        public void UpdateCompleteWithOutputs(Dictionary<NodeData.DataType, NodeData> dat)
        {
            if ((dat == null) || (dat.Count == 0))
            {
                this.UpdateThumb(null);

                var hsb = new HashSet<Block>();

                foreach (var outp in this._Outputs.Values)
                    foreach (var nhi in outp._NextLevels) 
                        hsb.Add(nhi._Block);

                foreach (var b in hsb) b.UpdateNewData();
            }
            else
            {
                var unused_outputs_keys = this._Outputs.Keys.ToList();
                var used_outputs = new List<NodeHandleOut>();

                foreach (var pair in dat)
                {
                    switch (pair.Key)
                    {
                        case NodeData.DataType.Sect:
                            {
                                NodeHandleOut nho;
                                var sect = (pair.Value as NodeDataSect)._Sect;
                                var key_base = new NodeHandleKey(sect._Type);

                                if ((unused_outputs_keys.Remove(key_base)))
                                {
                                    nho = this._Outputs[key_base];
                                }
                                else
                                {
                                    nho = new NodeHandleOut();
                                    this.Parent.Controls.Add(nho);
                                    this._Outputs[key_base] = nho;
                                }

                                nho._NodeData = pair.Value;
                                used_outputs.Add(nho);

                                if (sect._Type == SectType.Holder)
                                {
                                    foreach (var s2 in (sect as SectHolder).Sects.Values)
                                    {
                                        key_base = new NodeHandleKey(s2._Type);
                                        if ((unused_outputs_keys.Remove(key_base)))
                                        {
                                            nho = this._Outputs[key_base];
                                        }
                                        else
                                        {
                                            nho = new NodeHandleOut();
                                            this.Parent.Controls.Add(nho);
                                            this._Outputs[key_base] = nho;
                                        }
                                        nho._NodeData = new NodeDataSect(s2);
                                        used_outputs.Add(nho);
                                    }
                                }

                                if (this.pictureBoxThumb != null) this.UpdateThumb(sect);
                            }
                            break;
                        default:
                            {
                                NodeHandleOut nho;
                                var key_base = new NodeHandleKey(pair.Value._Type);

                                if ((unused_outputs_keys.Remove(key_base)))
                                {
                                    nho = this._Outputs[key_base];
                                }
                                else
                                {
                                    nho = new NodeHandleOut();
                                    this.Parent.Controls.Add(nho);
                                    this._Outputs[key_base] = nho;
                                }

                                nho._NodeData = pair.Value;
                                used_outputs.Add(nho);
                            }
                            break;
                    }
                }

                foreach (var k in unused_outputs_keys)
                {
                    var to_remove = this._Outputs[k];
                    to_remove.Disconnect();
                    to_remove.Parent.Controls.Remove(to_remove);
                    this._Outputs.Remove(k);
                }

                const int v_gap = 2;
                const int h_gap = 4;
                int th = -v_gap;

                foreach (var outp in used_outputs) th += outp.Size.Height + v_gap;

                int topper = this.Top + (this.Height - th) / 2;
                int lefter = this.Left + this.Width + h_gap;

                var hsb = new HashSet<Block>();

                foreach (var outp in used_outputs)
                {
                    outp.Top = topper;
                    outp.Left = lefter;
                    topper += v_gap + outp.Size.Height;
                    foreach (var nhi in outp._NextLevels) hsb.Add(nhi._Block);
                }

                foreach (var b in hsb) b.UpdateNewData();
            }
        }

        public PictureBox pictureBoxThumb = null;
        protected internal void UpdateThumb(Sect s)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.UpdateThumb(s);
                });
            }
            else
            {
                if (this.pictureBoxThumb.Image != null)
                {
                    this.pictureBoxThumb.Image.Dispose();
                    this.pictureBoxThumb.Image = null;
                }

                if (s != null)
                {
                    var sz = Sizing.fitAinB(s.getPrefferedSize(), this.pictureBoxThumb.Size).Size;
                    this.pictureBoxThumb.Image = s.getImage(sz.Width, sz.Height, !FormMain._BoolAutoZeroColor);
                }
            }
        }















        private struct NodeHandleKey : IEquatable<NodeHandleKey>
        {
            private readonly SectType _st;
            private readonly NodeData.DataType _dt;

            public NodeHandleKey(SectType s)
            {
                this._st = s;
                this._dt = NodeData.DataType.Sect;
            }

            public NodeHandleKey(NodeData.DataType d)
            {
                this._st = SectType.NaN;
                this._dt = d;
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 31 + (int)(this._st);
                hash = hash * 31 + (int)(this._dt);
                return hash;
            }

            public override bool Equals(object other)
            {
                return other is NodeHandleKey ? Equals((NodeHandleKey)other) : false;
            }

            public bool Equals(NodeHandleKey other)
            {
                switch (this._dt)
                {
                    case NodeData.DataType.Sect:
                        return (other._dt == NodeData.DataType.Sect) && (this._st == other._st);
                    default:
                        return other._dt == this._dt;
                }
            }
        }
    }
}
