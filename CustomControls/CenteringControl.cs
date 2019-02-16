using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class CenteringControl : Panel
    {
        public event EventHandler _Centering;

        public CenteringControl()
        {
            InitializeComponent();
        }

        private void CenteringControl_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Resize += this.Recenter;
            this.Recenter(sender, e);
        }

        private void CenteringControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            e.Control.Resize -= this.Recenter;
        }

        public void Recenter(object sender, EventArgs e)
        {
            if (this.DesignMode) return;

            if (this.Controls.Count != 1) return;

            var c = this.Controls[0];

            c.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            c.Top = (this.Height - c.Height) / 2;
            c.Left = (this.Width - c.Width) / 2;

            this._Centering?.Invoke(this, EventArgs.Empty);
        }
    }
}
