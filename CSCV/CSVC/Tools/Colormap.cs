﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Colormap : ToolDefault
    {
        private volatile ColorMapType t = ColorMapType.Cold_Hot;

        private static string name = "Color Map";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(Colormap.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellColorMap);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Colormap());
        }
        
        public Colormap()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.Colormap(d, t, ref o);
            return o;
        }
    } 
}
