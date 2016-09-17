using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SamSeifert.Utilities.FileParsing;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class FilePicker : UserControl
    {
        private static JsonDict _Values = null;

        static FilePicker()
        {
            FilePicker._Values = JsonParser.FromString.Dictionary(Properties.Settings.Default.FilePickers);
        }

        public event EventHandler _ValidFile;
        public event EventHandler _ValidFolder;
        public String _SaveIdentifier { get; set; } = "Default";
        public String _Text
        {
            get
            {
                return this.textBox1.Text;
            }
            set
            {
                this.textBox1.Text = value;
            }
        }
        
        public FilePicker()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String fn = this.textBox1.Text;

            if (File.Exists(fn))
            {
                this.textBox1.ForeColor = Color.Green;
                if (this._ValidFile != null)
                    this._ValidFile(this, EventArgs.Empty);
            }
            else if (Directory.Exists(fn))
            {
                this.textBox1.ForeColor = Color.Blue;
                if (this._ValidFolder != null)
                    this._ValidFolder(this, EventArgs.Empty);
            }
            else
            {
                this.textBox1.ForeColor = Color.Red;
                return;
            }

            FilePicker._Values[this._SaveIdentifier] = this.textBox1.Text;
            Properties.Settings.Default.FilePickers = FilePicker._Values.ToString();
            Properties.Settings.Default.Save();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void FilePicker_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            object outo;
            if (FilePicker._Values.TryGetValue(this._SaveIdentifier, out outo))
                if (outo is String)
                    this.textBox1.Text = outo as String;
        }
    }
}
