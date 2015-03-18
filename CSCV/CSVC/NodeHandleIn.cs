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
    public partial class NodeHandleIn : NodeHandle
    {
        internal static List<NodeHandle> allIns = new List<NodeHandle>();

        internal NodeHandleOut nho;

        internal NodeHandleIn(Tool parent)
            : base(parent)
        {
            InitializeComponent();
            this.IsInput = true;
            NodeHandleIn.allIns.Add(this);
        }

        internal void setInputNodeNHO(NodeHandleOut n)
        {
            if (this.nho != null) this.nho.nhis.Remove(this);
            this.nho = n;
            this.nho.nhis.Add(this);
            this.getTool.StatusChanged();
        }

/*        internal Tool getPreviousTool()
        {
            if (this.nho != null)
                return this.nho.getTool;

            return null;
        }*/

        internal SamSeifert.ImageProcessing.Sect getSpecialBitmap()
        {
            if (this.nho != null) return this.nho.getSpecialBitmap();
            return null;
        }
    }
}
