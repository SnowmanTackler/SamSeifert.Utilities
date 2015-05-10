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
using SamSeifert.CSCV;
using CSCV_IDE.Tools;

namespace CSCV_IDE
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
                ,Tools.MeanShift.getAddCellButton()
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











        private Sect _SpecialBitmap;
        private Bitmap _ImageHist = null;
        public Image Histogram()
        {
            HistogramViewer.histogram(
                this._SpecialBitmap,
                this.pictureBoxHist.Height,
                this.checkBoxAutoscaleHistogramBool, ref this._ImageHist);

            return this._ImageHist;
        }




        internal void updateWithData(Sect indata)
        {
            if (indata != null)
            {
                this._SpecialBitmap = indata;

                Image h = this.Histogram();
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

            Sect sectR = null;
            Sect sectG = null;
            Sect sectB = null;

            switch (this._SpecialBitmap._Type)
            {
                case SectType.RGB_R:
                    sectR = this._SpecialBitmap;
                    break;
                case SectType.RGB_G:
                    sectG = this._SpecialBitmap;
                    break;
                case SectType.RGB_B:
                    sectB = this._SpecialBitmap;
                    break;
                case SectType.Holder:
                    {
                        var sh = this._SpecialBitmap as SectHolder;
                        sh.Sects.TryGetValue(SectType.RGB_R, out sectR);
                        sh.Sects.TryGetValue(SectType.RGB_G, out sectG);
                        sh.Sects.TryGetValue(SectType.RGB_B, out sectB);
                    }
                    break;
            }

            if (sectR != null)
            {
                var s = sectR;
                this.labelMinR.Text = (255 * s.min).ToString(format);
                this.labelMaxR.Text = (255 * s.max).ToString(format);
                this.labelAvgR.Text = (255 * s.avg).ToString(format);
                if (s is SectArray) this.labelSDR.Text = (255 * (s as SectArray).std).ToString(format);
                else this.labelSDR.Text = nan;
            }
            else
            {
                this.labelMinR.Text = nan;
                this.labelMaxR.Text = nan;
                this.labelAvgR.Text = nan;
                this.labelSDR.Text = nan;
            }

            if (sectG != null)
            {
                var s = sectG;
                this.labelMinG.Text = (255 * s.min).ToString(format);
                this.labelMaxG.Text = (255 * s.max).ToString(format);
                this.labelAvgG.Text = (255 * s.avg).ToString(format);
                if (s is SectArray) this.labelSDG.Text = (255 * (s as SectArray).std).ToString(format);
                else this.labelSDG.Text = nan;
            }
            else
            {
                this.labelMinG.Text = nan;
                this.labelMaxG.Text = nan;
                this.labelAvgG.Text = nan;
                this.labelSDG.Text = nan;
            }

            if (sectB != null)
            {
                var s = sectB;
                this.labelMinB.Text = (255 * s.min).ToString(format);
                this.labelMaxB.Text = (255 * s.max).ToString(format);
                this.labelAvgB.Text = (255 * s.avg).ToString(format);
                if (s is SectArray) this.labelSDB.Text = (255 * (s as SectArray).std).ToString(format);
                else this.labelSDB.Text = nan;
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




































        private Tool DragToolView = null;
        private Block DraggingBlock = null;

        public static void addCell(Control _ToolView)
        {
            FormMain.get()._addCell(_ToolView);
        }

        private void _addCell(Control c)
        {
            if (c is Tool)
            {
                this.DragToolView = c as Tool;
                this.DragToolView.addTo(this.Controls);
                this.DragToolView.BringToFront();
                this.DragToolView.moveTo(FormMain.get().PointToClient(MousePosition));
            }
            else if (c is Block)
            {
                this.DraggingBlock = c as Block;
                this.Controls.Add(this.DraggingBlock);
                this.DraggingBlock.BringToFront();
                this.DraggingBlock.moveTo(FormMain.get().PointToClient(MousePosition));
            }
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

            if (this.DraggingBlock != null)
            {
                this.Controls.Remove(this.DraggingBlock);

                if (p.X > 0 && p.X < this.panelWorkspace.Width && p.Y > 0 && p.Y < this.panelWorkspace.Height)
                {
                    this.panelWorkspace.Controls.Add(this.DraggingBlock);
                    this.DraggingBlock.moveTo(p.X, p.Y);
                    this.DraggingBlock.BringToFront();
                    this.DraggingBlock.UpdateAddedToWorkspace();
                }

                this.DraggingBlock = null;
            }

            if (this.NodeHandleClicked != null)
            {
                try
                {
                    foreach (Control c in this.panelWorkspace.Controls)
                    {
                        if (c is NodeHandle)
                        {
                            var n = c as NodeHandle;
                            if (n.Contains(p))
                            {
                                if (this.NodeHandleClicked is NodeHandleIn)
                                {
                                    if (n is NodeHandleOut)
                                    {
                                        (this.NodeHandleClicked as NodeHandleIn).Connect(n as NodeHandleOut);
                                        return;
                                    }
                                }
                                if (n is NodeHandleIn)
                                {
                                    if (this.NodeHandleClicked is NodeHandleOut)
                                    {
                                        (n as NodeHandleIn).Connect(this.NodeHandleClicked as NodeHandleOut);
                                        return;
                                    }
                                }
                            }
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
                foreach (Control c in this.panelWorkspace.Controls)
                {
                    if (c is NodeHandle)
                    {
                        var n = c as NodeHandle;
                        if (n.Contains(p))
                        {
                            this.NodeHandleClicked = n;
                            this.panelWorkspace.Invalidate();
                            return;
                        }
                    }
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
            if (this.DraggingBlock != null)
            {
                this.DraggingBlock.move(e.X, e.Y);
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
                    this.paintLine(e.Graphics, p, this.NodeHandleClicked.LocationCustom());
                else
                    this.paintLine(e.Graphics, this.NodeHandleClicked.LocationCustom(), p);

            }

            foreach (Control c in this.panelWorkspace.Controls)
            {

                if (c is NodeHandleIn)
                {
                    NodeHandleIn nh = c as NodeHandleIn;
                    foreach (var nho in nh._PreviousLevels)
                    {
                        this.paintLine(e.Graphics, nh.LocationCustom(), nho.LocationCustom());
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
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
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
        }

        private void pictureBox3_MouseDown_1(object sender, MouseEventArgs e)
        {
        }

        private void pictureBoxOuput_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }




    }
}
