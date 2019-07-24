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

using SamSeifert.Utilities;
using System.IO.Pipes;
using System.Threading;
using SamSeifert.Utilities.Misc;

namespace SamSeifert.Utilities.Forms
{
    public partial class ScreenShotSaverForm : Form
    {
        public static event EventHandler ProgramaticTrigger;

        public static void Fire()
        {
            ScreenShotSaverForm.ProgramaticTrigger?.Invoke(null, EventArgs.Empty);
        }

        ScreenCapture sc = new ScreenCapture();

        int index
        {
            get { return (int)this.numericUpDown1.Value; }
            set { this.numericUpDown1.Value = value; }
        }

        public ScreenShotSaverForm()
        {
            InitializeComponent();
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();

            var s = this.folderBrowserDialog1.SelectedPath;

            if (s == null) return;

            this.labelDirectory.Text = s;

            this.index = 0;

            this.setNextLabel();
        }

        private void setNextLabel()
        {
            var s = this.folderBrowserDialog1.SelectedPath;

            if (s == null) return;

            if (!Directory.Exists(s)) return;

            var fs = Directory.GetFiles(s);

            Boolean willBreak = true;

            this.forcing = true;

            while (true)
            {
                s = index.ToString("000") + "0.png";

                willBreak = true;

                foreach (String j in fs)
                {
                    if (j.Contains(s))
                    {
                        willBreak = false;
                        break;
                    }
                }

                if (willBreak) break;

                index++;
            }

            this.forcing = false;

            this.labelFileName.Text = s;
        }

        private void bSave_Click(object sender, EventArgs ev)
        {
            var s = this.labelDirectory.Text;
            var e = this.labelFileName.Text;

            if (s.Length < 2) return;
            if (e.Length < 2) return;

            // PrtSc = Print Screen Key

            Image i = null;
            if (rbOwn.Checked) i = sc.CaptureScreen();
            // string keys = "{PrtSc}";
            // SendKeys.SendWait(keys);
            else i = Clipboard.GetImage();

            if (i == null) return;

            var ar = s.ToCharArray();

            if (!ar[ar.Length - 1].Equals(Path.DirectorySeparatorChar))
                s += Path.DirectorySeparatorChar;

            var p = s + e;

            Console.WriteLine(p);
            i.Save(p, ImageFormat.Png);
            i.Dispose();

            this.setNextLabel();
        }

        private Boolean forcing = false;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.forcing) return;

            this.setNextLabel();
        }

        private void rbCaptureMode_CheckedChanged(object sender, EventArgs e)
        {
            this.bSave.Enabled = this.rbCaptureClick.Checked;
            this.timer1.Enabled = this.rbCaptureAuto.Checked;

        }

        private void ScreenShotSaverForm_Load(object sender, EventArgs e)
        {
            ProgramaticTrigger += bSave_Click;
        }

        private void ScreenShotSaverForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProgramaticTrigger -= bSave_Click;
        }

    }
}
