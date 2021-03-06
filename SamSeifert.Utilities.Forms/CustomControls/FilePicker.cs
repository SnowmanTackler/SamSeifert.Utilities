﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SamSeifert.Utilities.Files.Json;
using SamSeifert.Utilities.Logging;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class FilePicker : TextBoxHint
    {
        const string default_save_name = "default_file_pickers.local.json";

        private static JsonDict _Values = null;

        static FilePicker()
        {
            var read = TextSettings.Read(default_save_name);
            if (read == null)
            {
                FilePicker._Values = new JsonDict();
            }
            else
            {
                FilePicker._Values = JsonDict.FromString(read);
            }
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

        private readonly OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private readonly Timer timer1 = new Timer();

        private String __SaveIdentifer = "Default";
        public String _SaveIdentifier
        {
            get
            {
                return this.__SaveIdentifer;
            }
            set
            {
                this.__SaveIdentifer = value;
                this.loadFromFileSystem();
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
        
        public FilePicker() : base()
        {
            if (this.DesignMode) return;

            this.openFileDialog1.FileOk += this.openFileDialog1_FileOk;
            this.timer1.Interval = 500;
            this.timer1.Tick += this.timer1_Tick;

            this.ApplyWatermark("C:\\Path\\To\\File");

            this.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            this.TextChanged += new System.EventHandler(this.textBox1_TextChanged);

            this.loadFromFileSystem();
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.ForeColor = Color.Gray;
            this.timer1.Stop();
            this.timer1.Enabled = true;
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;

            String fn = this.Text;

            if (this.WatermarkText == fn)
            {
                return;
            }

            if (File.Exists(fn))
            {
                Logger.Default.Debug("FilePicker file { " + fn + " }");
                this.ForeColor = Color.Green;
                this._ValidFile?.Invoke(this, EventArgs.Empty);
                this._Changed?.Invoke(this, FileType.File);
            }
            else if (Directory.Exists(fn))
            {
                Logger.Default.Debug("FilePicker directory { " + fn + " }");
                this.ForeColor = Color.Blue;
                this._ValidFolder?.Invoke(this, EventArgs.Empty);
                this._Changed?.Invoke(this, FileType.Directory);
            }
            else
            {
                Logger.Default.Debug("FilePicker invalid { " + fn + " }");
                this.ForeColor = Color.Red;
                this._Changed?.Invoke(this, FileType.Invalid);
                return;
            }

            this._ValidEntry?.Invoke(this, EventArgs.Empty);

            FilePicker._Values[this._SaveIdentifier] = this.Text;
            TextSettings.Save(default_save_name, FilePicker._Values.ToString());
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.Text = this.openFileDialog1.FileName;
        }

        private void loadFromFileSystem()
        {
            if (this.DesignMode)
            {
                return;
            }

            var identifier = this._SaveIdentifier;
            object outo;
            if (FilePicker._Values.TryGetValue(identifier, out outo))
                if (outo is String)
                {
                    Logger.Default.Debug("FilePicker loaded { " + outo + " } for { " + identifier + " }");
                    this.Text = outo as String;
                }
        }
    }
}
