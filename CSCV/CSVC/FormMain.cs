using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;

using SamSeifert.GlobalEvents;
using SamSeifert.ImageProcessing;
using ImageToolbox.Tools;

namespace ImageToolbox
{
    public partial class FormMain : Form
    {
        private static FormMain _FormMain;
        internal static FormMain get()
        {
            if (FormMain._FormMain == null)
            {
                FormMain._FormMain = new FormMain();
            }
            return FormMain._FormMain;
        }

        internal static void InvalidateWorkspace() { FormMain.get().panelWorkspace.Invalidate(); }
        internal static Size getWindowSize() { return FormMain.get().pictureBoxOuput.Size; }
        internal static Boolean _BoolAutoSizeSourceImages { get; private set; }
        internal static Boolean _BoolAutoZeroColor { get; set; }
        internal static event EventHandler OutputWindowResized;

        private void checkBoxAutoSize_CheckedChanged(object sender, EventArgs e)
        {
            FormMain._BoolAutoSizeSourceImages = this.checkBoxAutoSize.Checked;
            if (FormMain.OutputWindowResized != null) FormMain.OutputWindowResized(this, e);
        }

        private void checkBoxGrayBack_CheckedChanged(object sender, EventArgs e)
        {
            FormMain._BoolAutoZeroColor = this.checkBoxGrayBack.Checked;
            if (FormMain.OutputWindowResized != null) FormMain.OutputWindowResized(this, e);
        }

        private void panelOutput_Resize(object sender, EventArgs e)
        {
            this.checkBoxAutoSize_CheckedChanged(sender, e);
        }











        PictureBox _PictureBox = new PictureBox();
        void PictureBoxRemove(object sender, MouseEventArgs e)
        {
            if (this._PictureBox == null) return;

            this._PictureBox.MouseDown -= new MouseEventHandler(this.PictureBoxRemove);
            this.Controls.Remove(this._PictureBox);
            this._PictureBox.Image.Dispose();
            this._PictureBox.Image = null;
            this._PictureBox = null;
        }

        private FormMain()
        {
            InitializeComponent();

            FormMain._BoolAutoSizeSourceImages = true;
            FormMain._BoolAutoZeroColor = true;

            this.panelToolBar1.Height = 168;
            this.panelToolBar1.Height += this.labelToolBar.Height;
            this.panelToolBar1.Height += SystemInformation.HorizontalScrollBarHeight;
            this.panelToolBar1.Height += 10;
            this.populateToolbar();

            this.Controls.Add(this._PictureBox);
            this._PictureBox.Location = new Point(0, 0);
            this._PictureBox.Size = this.ClientSize;
            this._PictureBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this._PictureBox.BackColor = Color.White;
            this._PictureBox.Image = Properties.Resources.Startup;
            this._PictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            this._PictureBox.BringToFront();
            this._PictureBox.MouseDown += new MouseEventHandler(this.PictureBoxRemove);
            this._PictureBox.Show();

        }

        private void populateToolbar()
        {
            ToolButtonDefault[] buttons = new ToolButtonDefault[]
            {
                Tools.BitsPerPixel.getAddCellButton()
                ,Tools.Resize.getAddCellButton()
                ,Tools.Grayscale.getAddCellButton()
                ,Tools.ColorFilter.getAddCellButton()
                ,Tools.HistogramEqualizer.getAddCellButton()
                ,Tools.Colormap.getAddCellButton()
                ,Tools.Noise.getAddCellButton()
                ,Tools.BlackWhite.getAddCellButton()
                ,Tools.ErodeDilate.getAddCellButton()
                ,Tools.Convolute.getAddCellButton()
                ,Tools.Gradient.getAddCellButton()
                ,Tools.HoughFootOfNormal.getAddCellButton()
                ,Tools.HoughRhoTheta.getAddCellButton()
            };

            int gap = 10;
            int left = gap;
            int top = gap;


            foreach (ToolButtonDefault button in buttons)
            {
                this.panelToolBar3.Controls.Add(button);
                button.BackColor = this.panelToolBar3.BackColor;
                button.Top = top;
                button.Left = left;
                left += button.Width + gap;
            }
        }











        private ImageData _SpecialBitmap;

        internal void updateWithData(ImageData indata)
        {
            if (indata != null)
            {
                this._SpecialBitmap = indata;

                Image h = this._SpecialBitmap.Histogram(this.pictureBoxHist.Height, this.checkBoxAutoscaleHistogramBool);

                Image i = this._SpecialBitmap.getImage(!FormMain._BoolAutoZeroColor);

                this.pictureBoxOuput.Image = i;
                this.pictureBoxHist.Image = h;

                if (this.InvokeRequired) this.Invoke((MethodInvoker)delegate { this.setLabels(); });
                else this.setLabels();
            }
            else this.setError("Ghosting..."); ;
        }

        internal void setError(string p)
        {
            if (this.InvokeRequired) this.Invoke((MethodInvoker)delegate { this.labelStatus.Text = p; });
            else this.labelStatus.Text = p;
        }

        private void setLabels()
        {
            this.setError("");

            const string format = "0.#";
            const string nan = "-";

            var ts = this._SpecialBitmap.getImageSectTypes();

            if (ts.r != SectType.NaN)
            {
                var s = this._SpecialBitmap.getSect(ts.r, DataType.Read);
                this.labelMinR.Text = (255 * s.min).ToString(format);
                this.labelMaxR.Text = (255 * s.max).ToString(format);
                this.labelAvgR.Text = (255 * s.avg).ToString(format);
                this.labelSDR.Text = (255 * s.std).ToString(format);
            }
            else
            {
                this.labelMinR.Text = nan;
                this.labelMaxR.Text = nan;
                this.labelAvgR.Text = nan;
                this.labelSDR.Text = nan;
            }

            if (ts.g != SectType.NaN)
            {
                var s = this._SpecialBitmap.getSect(ts.g, DataType.Read);
                this.labelMinG.Text = (255 * s.min).ToString(format);
                this.labelMaxG.Text = (255 * s.max).ToString(format);
                this.labelAvgG.Text = (255 * s.avg).ToString(format);
                this.labelSDG.Text = (255 * s.std).ToString(format);
            }
            else
            {
                this.labelMinG.Text = nan;
                this.labelMaxG.Text = nan;
                this.labelAvgG.Text = nan;
                this.labelSDG.Text = nan;
            }

            if (ts.b != SectType.NaN)
            {
                var s = this._SpecialBitmap.getSect(ts.b, DataType.Read);
                this.labelMinB.Text = (255 * s.min).ToString(format);
                this.labelMaxB.Text = (255 * s.max).ToString(format);
                this.labelAvgB.Text = (255 * s.avg).ToString(format);
                this.labelSDB.Text = (255 * s.std).ToString(format);
            }
            else
            {
                this.labelMinB.Text = nan;
                this.labelMaxB.Text = nan;
                this.labelAvgB.Text = nan;
                this.labelSDB.Text = nan;
            }
        }

        private volatile Boolean checkBoxAutoscaleHistogramBool;

        private void checkBoxAutoscaleHistogram_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxAutoscaleHistogramBool = this.checkBoxAutoscaleHistogram.Checked;
            this.updateWithData(this._SpecialBitmap);
        }




































        private ImageToolbox.Tools.Tool DragToolView = null;

        public static void addCell(ImageToolbox.Tools.Tool _ToolView)
        {
            FormMain.get()._addCell(_ToolView);
        }

        private void _addCell(ImageToolbox.Tools.Tool _ToolView)
        {
            this.DragToolView = _ToolView;
            this.DragToolView.addTo(this.Controls);
            this.DragToolView.BringToFront();
            this.DragToolView.moveTo(FormMain.get().PointToClient(MousePosition));
        }

        private Point getWorkspaceCoordOfMouse()
        {
            Point p = this.PointToClient(MousePosition);
            p.X -= this.panelWorkspace.Location.X;
            p.Y -= this.panelWorkspace.Location.Y;
            return p;
        }






        private void FormMain_Load(object sender, EventArgs e)
        {
            GlobalEventHandler.LMouseDown += this.LMouseDown;
            GlobalEventHandler.LMouseUp += this.LMouseUp;
            GlobalEventHandler.LMouseDrag += this.LMouseDrag;
        }

        private void LMouseUp(object sender, EventArgs e)
        {
            Point p = this.getWorkspaceCoordOfMouse();

            if (this.DragToolView != null)
            {
                this.DragToolView.removeFrom(this.Controls);

                if (p.X > 0 && p.X < this.panelWorkspace.Width && p.Y > 0 && p.Y < this.panelWorkspace.Height)
                {
                    this.DragToolView.addTo(this.panelWorkspace.Controls);
                    this.DragToolView.moveTo(p.X, p.Y);
                    this.DragToolView.BringToFront();
                }

                this.DragToolView = null;
            }

            if (this.NodeHandleClicked != null)
            {
                try
                {
                    if (this.NodeHandleClicked.IsInput)
                    {
                        foreach (NodeHandle n in NodeHandleOut.allOuts) if (n.Contains(p))
                            {
                                this.NodeHandleClicked.setInputNode(n);
                                return;
                            }
                    }
                    else
                    {
                        foreach (NodeHandle n in NodeHandleIn.allIns) if (n.Contains(p))
                            {
                                n.setInputNode(this.NodeHandleClicked);
                                return;
                            }
                    }
                }
                finally
                {
                    this.NodeHandleClicked = null;
                    this.panelWorkspace.Invalidate();
                }
            }
        }

        private void LMouseDown(object sender, EventArgs e)
        {
            Point p = this.getWorkspaceCoordOfMouse();

            this.NodeHandleClicked = null;

            if (p.X < this.panelWorkspace.Width && p.Y < this.panelWorkspace.Height)
            {
                foreach (NodeHandle n in NodeHandleOut.allOuts) if (n.Contains(p))
                    {
                        this.NodeHandleClicked = n;
                        this.panelWorkspace.Invalidate();
                        return;
                    }

                foreach (NodeHandle n in NodeHandleIn.allIns) if (n.Contains(p))
                    {
                        this.NodeHandleClicked = n;
                        this.panelWorkspace.Invalidate();
                        return;
                    }
            }
        }

        private void LMouseDrag(object sender, MouseEventArgs e)
        {
            if (this.DragToolView != null)
            {
                DragToolView.move(e.X, e.Y);
            }
            if (NodeHandleClicked != null)
            {
                this.panelWorkspace.Invalidate();
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.DragToolView != null)
                {
                    this.DragToolView.removeFrom(this.Controls);
                    this.DragToolView = null;
                }
            }
        }












        private NodeHandle NodeHandleClicked = null;

        private void panelWorkspace_Paint(object sender, PaintEventArgs e)
        {
            Point p = this.getWorkspaceCoordOfMouse();

            if (NodeHandleClicked != null)
            {
                if (this.NodeHandleClicked as NodeHandleIn == null)
                    this.paintLine(e.Graphics, p, this.NodeHandleClicked.LocationCustom);
                else
                    this.paintLine(e.Graphics, this.NodeHandleClicked.LocationCustom, p);

            }

            foreach (Control c in this.panelWorkspace.Controls)
            {
                NodeHandleIn nh = c as NodeHandleIn;

                if (nh != null)
                {
                    if (nh.nho != null)
                    {
                        this.paintLine(e.Graphics, nh.LocationCustom, nh.nho.LocationCustom);
                    }
                }
            }
        }

        private void paintLine(Graphics g, Point pp1, Point pp2)
        {
            Color col = Color.White;
            const int width = 3;
            const int offset = 15;

            if (pp2.X + offset * 2 > pp1.X)
            {
                Point p1 = pp1;
                p1.X -= offset;

                Point p2 = pp2;
                p2.X += offset;

                g.DrawLine(new Pen(col, width), pp1, p1);
                g.DrawLine(new Pen(col, width), pp2, p2);

                int mid = (p1.Y + p2.Y) / 2;

                Point pm1 = new Point(p1.X, mid);
                Point pm2 = new Point(p2.X, mid);

                g.DrawLine(new Pen(col, width), pm1, pm2);
                g.DrawLine(new Pen(col, width), pm1, p1);
                g.DrawLine(new Pen(col, width), pm2, p2);
            }
            else
            {
                int mid = (pp1.X + pp2.X) / 2;

                Point pm1 = new Point(mid, pp1.Y);
                Point pm2 = new Point(mid, pp2.Y);

                g.DrawLine(new Pen(col, width), pm1, pm2);
                g.DrawLine(new Pen(col, width), pm1, pp1);
                g.DrawLine(new Pen(col, width), pm2, pp2);
            }
        }


















        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.pictureBoxOuput.Image.Save(this.saveFileDialog1.FileName);
        }










        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Version 1.01\n" +
                "05 20 2013\n" +
                "Developed by Sam Seifert\n" +
                "Advised by Lester Gerhardt"
                );
        }


















        private void addToolSource(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new Source());
        }

        private void addToolSink(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new Sink());
        }

        private void addToolAdder(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new Add());
        }

        private void addToolSwitch(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new ToolSwitch());
        }

        private void addToolGain(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new Gain());
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new SplitRGB());
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new SplitPN());
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new AbsoluteValue());
        }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new Multiply());
        }

        private void pictureBox2_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new SplitHSV());

        }

        private void pictureBox3_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                this._addCell(new SplitHSL());
        }

        private void pictureBoxOuput_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }




    }
}
