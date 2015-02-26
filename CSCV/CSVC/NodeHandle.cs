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
    public partial class NodeHandle : UserControl
    {
        internal enum NodeHandleTypeEnum { input, output };

        protected internal Tool getTool;

        private NodeHandle() { InitializeComponent(); }
        internal NodeHandle(Tool parent)
        {
            InitializeComponent();
            this.getTool = parent;
        }        

        private bool DisposingCustom = false;
        internal bool IsInput;

        internal new void Dispose()
        {
            if (this.DisposingCustom) return;
            this.DisposingCustom = true;


            NodeHandleOut.allOuts.Remove(this);
            NodeHandleIn.allIns.Remove(this);

            foreach (NodeHandleOut nho in NodeHandleOut.allOuts)
                nho.nhis.Remove(this as NodeHandleIn);

            foreach (NodeHandleIn nhi in NodeHandleIn.allIns)
                if (nhi.nho == this) nhi.nho = null;

            base.Dispose();
        }

        internal Point LocationCustom
        {
            get
            {
                return new Point(this.Location.X + this.Width/2, this.Location.Y + this.Height /2);
            }
        }

        internal Boolean Contains(Point p)
        {
            return (this.Left < p.X && p.X < this.Left + this.Width &&
                    this.Top < p.Y && p.Y < this.Top + this.Height);
        }

        internal void setInputNode(NodeHandle n)
        {
            NodeHandleIn nho = this as NodeHandleIn;
            NodeHandleOut nhi = n as NodeHandleOut;

            if (nho != null && nhi != null)
                nho.setInputNodeNHO(nhi);
        }
    }
}
