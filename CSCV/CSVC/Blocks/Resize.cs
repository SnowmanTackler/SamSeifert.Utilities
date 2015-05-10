using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.CSCV;

namespace CSCV_IDE.Tools
{
    public partial class Resize : BlockDefault
    {
        private volatile float inud = 0.5f;
        private NumericUpDown nud;
        private volatile ResizeType t = ResizeType.NearestNeighbor;
        private ComboBox cb;

        public const string name = "Resize";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellResize);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Resize());
        }
        
        
        
        
        
      

        public Resize()
        {
            this.checkBoxName.Text = name;
        }

        public override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.AddRange(new object[] {
                ResizeType.NearestNeighbor,
                ResizeType.Bilinear
            });
            this.cb.SelectedItem = this.t;
            this.cb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            l.Add(this.cb);

            this.nud = ControlDeque.NumericUpDown(2);
            this.nud.Minimum = 0.01m;
            this.nud.Maximum = 10.0m;
            this.nud.Value = new decimal(this.inud);
            this.nud.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            l.Add(this.nud);

            return l;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.inud = (float)this.nud.Value;
            this.ClearFutures();
            this.UpdateNewData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.t = (ResizeType)this.cb.SelectedItem;
            this.ClearFutures();
            this.UpdateNewData();
        }


        public override void UpdateNewData()
        {
            base.UpdateNewData();

            if (this.checkBoxName.Checked)
            {
                var inp = this.StandardNodeHandelIn.getData();

                Size sz;
                Sect s;
                NodeData ask;
                if (inp == null)
                {
                    this.UpdateCompleteWithOutputs(null);
                }
                else if (inp.TryGetValue(NodeData.DataType.Sect, out ask))
                {
                    s = (ask as NodeDataSect)._Sect;

                    if (inp.TryGetValue(NodeData.DataType.Size, out ask)) sz = (ask as NodeDataSize)._Size;
                    else sz = s.getPrefferedSize();

                    sz.Width = (int)(sz.Width * this.inud);
                    sz.Height = (int)(sz.Height * this.inud);

                    Sect o = null;
                    IA_Single.Resize(s, sz, t, ref o);

                    var dc = new Dictionary<NodeData.DataType, NodeData>();
                    dc[NodeData.DataType.Sect] = new NodeDataSect(o);
                    dc[NodeData.DataType.Size] = new NodeDataSize(sz);
                    this.UpdateCompleteWithOutputs(dc);
                }
                else this.UpdateCompleteWithOutputs(null);
            }
            else this.UpdateCompleteWithOutputs(this.StandardNodeHandelIn.getData());
        }
    }
}
