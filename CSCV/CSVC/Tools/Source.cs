using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Source : ToolDefault
    {
        private OpenFileDialog _OpenFileDialog = new OpenFileDialog();
        private ComboBox cb;
        private Sect _SpecialBitmapSource;
        private Image OriginalImage;

        private static readonly String[] AcceptableFileExtensions = new String[]
            {
                ".JPG",
                ".JPEG",
                ".TIFF",
                ".BMP",
                ".PNG",
            };


        public Source()
            : base(true, false)
        {
            this.checkBoxName.Text = "Source";
            this.ToolStrip_Edit.Enabled = true;


            String _Filter = "Image Files|";
            bool first = true;
            foreach (String s in AcceptableFileExtensions)
            {
                _Filter += (first ? "" : ";") + "*" + s;
                first = false;
            }

            this._OpenFileDialog.Filter = _Filter;
            this._OpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            FormMain.OutputWindowResized += new EventHandler(this.FormMain_OutputWindowResized);
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.Add("Lena");
            this.cb.Items.Add("Gear");
            this.cb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.cb.SelectedIndex = 0;
            l.Add(this.cb);

            return l;
        }

        public override void MenuEdit()
        { 
            this._OpenFileDialog.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.loadImages(this._OpenFileDialog.FileName);
        }


        private void loadImages(String name)
        {
            if (name == null) return;
            if (!File.Exists(name)) return;

            List<String> strList = new List<String>();
            ListBoxObject indexF = null;

            String dirName = name.Substring(0, name.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (Directory.Exists(dirName))
            {
                this.cb.Items.Clear();

                foreach (String s in AcceptableFileExtensions)
                {
                    String[] files = Directory.GetFiles(dirName, "*" + s.ToLower(), SearchOption.AllDirectories);

                    foreach (String s2 in files)
                    {
                        ListBoxObject o = new ListBoxObject(s2);
                        this.cb.Items.Add(o);
                        if (name.Equals(s2)) indexF = o;
                    }
                }
            }

            this.cb.SelectedItem = indexF;
        }

        private class ListBoxObject : Object
        {
            public String name, path;

            public ListBoxObject(String p)
            {
                this.path = p;
                int dex = p.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                this.name = p.Substring(dex, p.Length - dex);
            }

            public override String ToString() { return this.name; }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.OriginalImage != null)
            {
                this.OriginalImage.Dispose();
                this.OriginalImage = null;
            }
            try
            {
                ListBoxObject lbo = (this.cb.SelectedItem as ListBoxObject);
                this.OriginalImage = Image.FromFile(lbo.path);
            }
            catch
            {
                this.OriginalImage = null;
                var st = this.cb.SelectedItem as String;
                if (st != null)
                {
                    if (st.Equals("Gear")) this.OriginalImage = Properties.Resources.Gear;
                }
                
                if (this.OriginalImage == null) this.OriginalImage = Properties.Resources.Lena;
            }

            this.FormMain_OutputWindowResized(sender, e);
        }

        private void FormMain_OutputWindowResized(object sender, EventArgs e)
        {
            Size s = FormMain.getWindowSize();
            s.Width -= 10;
            s.Height -= 10;

            Rectangle r = Sizing.fitAinB(this.OriginalImage.Size, s);

            if (FormMain._BoolAutoSizeSourceImages &&
                r.Width < this.OriginalImage.Width &&
                r.Height < this.OriginalImage.Height)
            {               
                this._SpecialBitmapSource =  SectHolder.SectHoldeFromImage(this.OriginalImage, r.Width, r.Height);
            }
            else
            {
                this._SpecialBitmapSource =  SectHolder.SectHoldeFromImage(this.OriginalImage);
            }

            this.StatusChanged();
        }

        public override void SpecialBitmapUpdate()
        {
            if (this.checkBoxName.Checked)
            {
                this._SpecialBitmap = this._SpecialBitmapSource;
                this.UpdateThumb();
            }
            else
            {
                this._SpecialBitmap = null;
            }
        }
    }
}
