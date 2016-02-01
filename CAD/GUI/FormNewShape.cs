using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;

namespace SamSeifert.GLE.CAD.GUI
{
    public partial class FormNewShape : Form
    {
        private enum SelectionType { XAML }
        private SelectionType _SelectionType = SelectionType.XAML;

        private bool file1 = false;
        private bool file2 = false;
        private bool file3 = false;

        private CadHandler _CadHandler = null;

        public FormNewShape() : base()
        {
            InitializeComponent();

            this.updateEnabled();
        }

        internal FormNewShape(CadHandler mommy) : this()
        {
            this._CadHandler = mommy;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;

            if (r != null)
            {
                if (r.Checked)
                {
                    if (r == this.radioButton4) this._SelectionType = SelectionType.XAML;

                    this.updateEnabled();
                }
            }
        }

        private void updateEnabled()
        {
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;

            this.button1.Text = "File 1";

            this.openFileDialog1.Filter = "";
            this.openFileDialog2.Filter = "";
            this.openFileDialog3.Filter = "";

            switch (this._SelectionType)
            {

                case SelectionType.XAML:
                    {
                        this.button1.Text = "XAML File";
                        this.openFileDialog1.Filter = "XAML File|*.XAML";
                        b1 = true;
                        break;
                    }
            }

            this.button1.Enabled = b1;
            this.textBox1.Enabled = b1;

            this.setGo();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
//            this.textBox2.Text = this.openFileDialog2.FileName;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
//            this.textBox3.Text = this.openFileDialog3.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.openFileDialog3.ShowDialog();
        }

        private Color ColorGood = Color.Green;
        private Color ColorBad = Color.Red;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.file1 = File.Exists(this.textBox1.Text);
            this.textBox1.ForeColor = this.file1 ?
                this.ColorGood :
                this.ColorBad;
            this.setGo();
        }

        private void setGo()
        {
            this.button0.Enabled =
                (this.file1 || !this.button1.Enabled) ;// &&
//                (this.file2 || !this.button2.Enabled) &&
//                (this.file3 || !this.button3.Enabled);
        }

        private void button0_Click(object sender, EventArgs e)
        {
            var list = new List<CadObject>();

            switch (this._SelectionType)
            {
                case SelectionType.XAML:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t = String.Join(" ", File.ReadAllLines(this.textBox1.Text));
                        var co = SamSeifert.GLE.CAD.Generator.FromXaml.Create(t, name);
                        if (co != null) list.Add(co);
                        break;
                    }
            }

            this._CadHandler.addParts(list.ToArray());

            this.Close();
        }
    }
}
