using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;
using ImageToolbox.Tools;

namespace ImageToolbox
{
    public partial class NodeHandleOut : NodeHandle
    {
        internal static List<NodeHandle> allOuts = new List<NodeHandle>();

        internal List<NodeHandleIn> nhis = new List<NodeHandleIn>();

        internal NodeHandleOut(Tool parent)
            : base(parent)
        {
            InitializeComponent();
            this.IsInput = false;
            NodeHandleOut.allOuts.Add(this);
        }

        internal Sect getSpecialBitmap()
        {
            return this.getTool.SpecialBitmapGet(this);
        }
    }
}
