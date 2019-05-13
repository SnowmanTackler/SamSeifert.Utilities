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
using SamSeifert.GLE.CAD;
using SamSeifert.Utilities.Files.Vrml;

namespace SamSeifert.GLE.Forms
{
    public partial class FormNewShape : Form
    {
        private CadHandler _CadHandler = null;

        public FormNewShape() : base()
        {
            InitializeComponent();
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
                    if (r == this.rbXAML) this.openFileDialog1.Filter = "XAML File|*.XAML";
                    else if (r == this.rbVRML) this.openFileDialog1.Filter = "VRML File|*.WRL";
                    else if (r == this.rbSTL) this.openFileDialog1.Filter = "STL File|*.STL";

                    this.buttonBrowse.Enabled = true;
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }        

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }
        

        private Color ColorGood = Color.Green;
        private Color ColorBad = Color.Red;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var fileExists = File.Exists(this.textBox1.Text);
            this.buttonGo.Enabled = fileExists;
            this.textBox1.ForeColor = fileExists ?
                this.ColorGood :
                this.ColorBad;
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            CadObject co = null;

            var name = Path.GetFileName(this.textBox1.Text);

            switch (Path.GetExtension(name).ToLower())
            {
                case ".xaml":
                    {
                        var t = String.Join(" ", File.ReadAllLines(this.textBox1.Text));
                        co = CAD.Generator.FromXaml.Create(t, name);
                        break;
                    }
                case ".wrl":
                    {
                        co = CAD.Generator.FromVrml.Create(VrmlFile.FromFile(this.textBox1.Text))
                            .Center()
                            .ConsolidateMatrices()
                            .ConsolidateColors();
                        break;
                    }
                case ".stl":
                    {
                        co = CAD.Generator.FromStl.Create(File.ReadAllText(this.textBox1.Text))
                            .Center()
                            .ConsolidateMatrices()
                            .ConsolidateColors();
                        break;
                    }
            }

            if (co != null)                
                this._CadHandler.addParts(co);

            this.Close();
        }
        
    }
}
