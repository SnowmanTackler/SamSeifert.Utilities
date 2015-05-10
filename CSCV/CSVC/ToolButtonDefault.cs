using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSCV_IDE
{
    public partial class ToolButtonDefault : UserControl
    {
        public ToolButtonDefault()
        {
            InitializeComponent();
        }

        public void setText(String text)
        {
            this.label1.Text = text;
        }

        public void setImage(Image i)
        {
            this.pictureBox1.Image = i;
        }

        public void setClickHandler(MouseEventHandler e)
        {
            this.pictureBox1.MouseDown += e;
        }
    }
}
