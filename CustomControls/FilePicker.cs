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

using SamSeifert.Utilities.Json;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class FilePicker : UserControl
    {
        private static JsonDict _Values = null;

        static FilePicker()
        {
            FilePicker._Values = JsonDict.FromString(Properties.Settings.Default.FilePickers);
        }

        /// <summary>
        /// Happens on all files
        /// </summary>
        public event EventHandler _ValidFile;

        /// <summary>
        /// Happens on all directories
        /// </summary>
        public event EventHandler _ValidFolder;

        /// <summary>
        /// Happens on all files or directories
        /// </summary>
        public event EventHandler _ValidEntry;

        public enum FileType { File, Directory, Invalid };
        public delegate void FileTypeEventHandler(object sender, FileType ft);
        public event FileTypeEventHandler _Changed;


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

        public String _Filter
        {
            get
            {
                return this.openFileDialog1.Filter;
            }
            set
            {
                this.openFileDialog1.Filter = value;
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
                this._ValidFile?.Invoke(this, EventArgs.Empty);
                this._Changed?.Invoke(this, FileType.File);
            }
            else if (Directory.Exists(fn))
            {
                this.textBox1.ForeColor = Color.Blue;
                this._ValidFolder?.Invoke(this, EventArgs.Empty);
                this._Changed?.Invoke(this, FileType.Directory);
            }
            else
            {
                this.textBox1.ForeColor = Color.Red;
                this._Changed?.Invoke(this, FileType.Invalid);
                return;
            }

            this._ValidEntry?.Invoke(this, EventArgs.Empty);

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
