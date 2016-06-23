using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using SamSeifert.CSCV.Cameras;

namespace SampleProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Console.WriteLine(Guid.NewGuid().ToString());
            InitializeComponent();
            Application.Idle += new EventHandler(this.Application_Idle);
        }

        private bool notStarted = true;

        private Camera webCamCapture1 = new Camera();

        private int lastTime = System.Environment.TickCount;
        private int count = 0;

        public void Application_Idle(object sender, EventArgs e)
        {
            if (this.notStarted)
            {
                this.webCamCapture1.Start(this.Handle.ToInt32());
                this.notStarted = false;
                this.Enabled = true;
            }
            else
            {
                Bitmap b = this.webCamCapture1.capture();

                if (b != null)
                {
                    if (this.pictureBox1.Image != null) this.pictureBox1.Image.Dispose();
                    this.pictureBox1.Image = b;


                    this.count++;
                    int newTime = System.Environment.TickCount;
                    int elapsedTime = newTime - this.lastTime;
                    if (elapsedTime > 500)
                    {
                        this.labelFPS.Text = "FPS: " + (count * 1000.0 / elapsedTime).ToString("00.0");
                        this.lastTime = newTime;
                        this.count = 0;
                    }
                }
                else this.labelFPS.Text = "NULL";
            }
        }

        private void buttonResolution_Click(object sender, EventArgs e)
        {
            Console.WriteLine("RES");
            this.webCamCapture1.configureResolution();
        }

        private void buttonAdvanced_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ADV");
            this.webCamCapture1.configureSource();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                this.pictureBox1.Image.Save(
                    this.saveFileDialog1.FileName,
                    System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
