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
    public partial class Convolute : ToolDefault
    {
        private enum ConvoluteType { Sobel_Edge, Sobel_X, Sobel_Y, Low_Pass, Custom };
        private volatile ConvoluteType t = ConvoluteType.Sobel_Edge;
        private NumericUpDown nud;
        private ComboBox cb;
        private static string name = "Convolute";

        private static readonly Decimal[,] Filter_Sobel_Edge = new Decimal[,]
        {            
            { -1, -1, -1 },
            { -1,  8, -1 },
            { -1, -1, -1 }
        };

        private static readonly Decimal[,] Filter_Sobel_Y = new Decimal[,]
        {            
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
        };

        private static readonly Decimal[,] Filter_Sobel_X = new Decimal[,]
        {            
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };


        private Decimal[,] filt = Filter_Sobel_Edge;

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(Convolute.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellConvolute);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Convolute());
        }
















        public Convolute()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
            this.ToolStrip_Edit.Enabled = true;
        }

        private Sect _ImageDataLast = null;
        public override void MenuEdit()
        {
            new ConvoluteFull(this, this.filt).ShowDialog(this._ImageDataLast);
        }

        public void setFilter(Decimal[,] convs)
        {
            this.filt = convs;

            try
            {
                var ct = (ConvoluteType)this.cb.SelectedItem;

                if (Array.Equals(this.filt, Filter_Sobel_Edge))
                {
                    if (ct != ConvoluteType.Sobel_Edge)
                    {
                        this.nud.Enabled = false;
                        this.cb.SelectedItem = ConvoluteType.Sobel_Edge;
                    }
                }
                else if (Array.Equals(this.filt, Filter_Sobel_X))
                {
                    if (ct != ConvoluteType.Sobel_X)
                    {
                        this.nud.Enabled = false;
                        this.cb.SelectedItem = ConvoluteType.Sobel_X;
                    }
                }
                else if (Array.Equals(this.filt, Filter_Sobel_Y))
                {
                    if (ct != ConvoluteType.Sobel_Y)
                    {
                        this.nud.Enabled = false;
                        this.cb.SelectedItem = ConvoluteType.Sobel_Y;
                    }
                }
                else
                {

                    if (this.filt.GetLength(0) != this.filt.GetLength(1))
                    {
                        if (ct != ConvoluteType.Custom)
                        {
                            this.cb.SelectedItem = ConvoluteType.Custom;
                            return;
                        }
                    }

                    decimal iter = Decimal.MaxValue;
                    foreach (var d in this.filt)
                    {
                        if (iter == Decimal.MaxValue) iter = d;
                        else if (iter != d)
                        {
                            if (ct != ConvoluteType.Custom)
                            {
                                this.cb.SelectedItem = ConvoluteType.Custom;
                                return;
                            }
                        }
                    }

                    this.nud.Enabled = true;

                    if (ct != ConvoluteType.Low_Pass)
                        this.cb.SelectedItem = ConvoluteType.Low_Pass;

                    var val = this.nud.Value;
                    var lens = (int)val;
                    val = 1 / (val * val);

                    if (lens == this.filt.GetLength(0))
                    {
                        for (int i = 0; i < lens; i++)
                            for (int j = 0; j < lens; j++)
                                this.filt[i, j] = val;
                    }
                    else
                    {
                        this.nud.Value = this.filt.GetLength(0);
                    }
                }
            }
            finally
            {
                this.StatusChanged();
            }
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.AddRange(new object[] {
                ConvoluteType.Sobel_Edge,
                ConvoluteType.Sobel_X,
                ConvoluteType.Sobel_Y,
                ConvoluteType.Low_Pass,
                ConvoluteType.Custom
            });

            this.cb.SelectedItem = this.t;
            this.cb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            l.Add(this.cb);

            this.nud = ControlDeque.NumericUpDown(0);
            this.nud.Minimum = 1;
            this.nud.Maximum = 50;
            this.nud.Increment = 2;
            this.nud.Value = 1;
            this.nud.Enabled = false;
            this.nud.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            l.Add(this.nud);

            return l;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.nud.Value % 2 == 0) this.nud.Value -= 1;
            else this.updateCustom();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.nud.Enabled = false;

            switch ((ConvoluteType)this.cb.SelectedItem)
            {
                case ConvoluteType.Sobel_Edge: this.setFilter(Filter_Sobel_Edge); break;
                case ConvoluteType.Sobel_X: this.setFilter(Filter_Sobel_X); break;
                case ConvoluteType.Sobel_Y: this.setFilter(Filter_Sobel_Y); break;
                case ConvoluteType.Low_Pass: this.updateCustom(); break;
            }
        }
        private void updateCustom()
        {
            var val = this.nud.Value;
            var lens = (int)val;
            val = 1 / (val * val);

            Decimal[,] f = new Decimal[lens, lens];

            for (int i = 0; i < lens; i++)
                for (int j = 0; j < lens; j++)
                    f[i, j] = val;            

            this.setFilter(f);
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            this._ImageDataLast = d;

            Sect o = null;

            var h = this.filt.GetLength(0);
            var w = this.filt.GetLength(1);

            var vals = new float[h, w];

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    vals[i, j] = (float)(this.filt[i, j]);

            IA_Single.Convolute(d, vals, ref o);

            return o;
        }

    }
}
