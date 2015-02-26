using System;
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
    public partial class Sink : ToolDefault
    {
        public Sink()
            : base(false, true)
        {
            this.checkBoxName.Text = "Sink";
            this.checkBoxName.AutoCheck = false;

            this.checkBoxName.CheckedChanged -= base.checkBoxName_CheckedChanged;
            this.checkBoxName.Click += this.checkBoxName_Click;

            Sink.setNewToolImageSink(this);
        }

        public override Boolean ClearData(NodeHandleIn sender)
        {
            if (this.checkBoxName.Checked) FormMain.get().updateWithData(null);
            return base.ClearData(sender);
        }


        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            FormMain.get().updateWithData(d);
            return d;
        }

        private static Sink _Selected = null;

        private static void setNewToolImageSink(Sink s)
        {
            if (Sink._Selected != null)
                Sink._Selected.checkBoxName.Checked = false;
            
            s.checkBoxName.Checked = true;
            FormMain.get().updateWithData(s._SpecialBitmap);
            Sink._Selected = s;
        }

        private void checkBoxName_Click(object sender, EventArgs e)
        {
            Sink.setNewToolImageSink(this);
        }
    }
}
