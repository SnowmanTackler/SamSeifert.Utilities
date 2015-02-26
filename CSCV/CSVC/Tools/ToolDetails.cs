using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class ToolDetails : UserControl
    {
        private ImageData _SpecialBitmapIn;
        private ImageData _SpecialBitmapOut;

        private Form _Form;
        private Boolean startup = true;
        private volatile Boolean _Dead = false;
        private Size OriginalSize;

        private System.Threading.Timer timer1;
        private static volatile Thread currentThread = null;

        public ToolDetails()
        {
            InitializeComponent();

            this.timer1 = new System.Threading.Timer((TimerCallback)delegate
            { (new Thread(this.threadStarter)).Start(); });

        }

        public void ShowDialog(ImageData input)
        {
            this._SpecialBitmapIn = input;
            if (this._SpecialBitmapIn != null) this.pictureBoxIn.Image = this._SpecialBitmapIn.getImage();

            this._Form = new Form();
            this._Form.ShowIcon = false;
            this._Form.ShowInTaskbar = false;
            this._Form.ClientSize = this.Size;

            if (this.startup)
            {
                this.OriginalSize = this._Form.Size;
                this.startup = false;
            }

            this._Form.MinimumSize = this.OriginalSize;
            this._Form.Controls.Add(this);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;

            this._Form.FormClosing += new FormClosingEventHandler(_Form_FormClosing);

            this.updateImage();
            
            this._Form.ShowDialog();

            this.pictureBoxOut.Image = null;
            this.pictureBoxIn.Image = null;
            this._SpecialBitmapIn = null;
            this._SpecialBitmapOut = null;
        }

        public void updateImage()
        {
            this.timer1.Change(500, Timeout.Infinite);
            this.labelStatus.Show();
        }

        private void threadStarter()
        {
            if (this._Dead) return;

            var t = ToolDetails.currentThread;
            ToolDetails.currentThread = Thread.CurrentThread;
            ToolDetails.currentThread.Name = "Tool Edit Update Thread";

            if (t != null) t.Join();

            this._SpecialBitmapOut = this.updateOverride(this._SpecialBitmapIn);
            
            Image i = null;

            if (this._SpecialBitmapOut != null) i = this._SpecialBitmapOut.getImage();

            if (Thread.CurrentThread == ToolDetails.currentThread)
            {
                this.pictureBoxOut.Invoke((MethodInvoker)delegate { this.setImage(i); });
            }
        }

        public void setImage(Image i)
        {
            this.labelStatus.Hide();
            if (this.pictureBoxOut.Image != null) this.pictureBoxOut.Image.Dispose();
            this.pictureBoxOut.Image = i;
        }

        public virtual ImageData updateOverride(ImageData indata) { return null; }

        private void panel1_Resize(object sender, EventArgs e)
        {
            int oW = this.pictureBoxIn.Width;
            int w = (this.panel1.Width - 30)/2;
            
            this.pictureBoxIn.Width = w;
            this.pictureBoxOut.Width = w;

            this.pictureBoxOut.Left += (w - oW);
        }

        protected virtual void _Form_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._Dead = true;
            var t = ToolDetails.currentThread;
            if (t != null) t.Join();

            if (this.pictureBoxIn.Image != null) this.pictureBoxIn.Image.Dispose();
            if (this.pictureBoxOut.Image != null) this.pictureBoxOut.Image.Dispose();

            this.pictureBoxIn.Image = null;
            this.pictureBoxOut.Image = null;
        }
    }
}
