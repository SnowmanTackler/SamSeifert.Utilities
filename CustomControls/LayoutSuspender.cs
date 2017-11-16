using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public class LayoutSuspender : IDisposable
    {
        private Control[] ccs;

        public LayoutSuspender(params Control[] controls)
        {
            this.ccs = controls;

            foreach (var c in this.ccs) c.SuspendLayout();
        }

        public void Dispose()
        {
            foreach (var c in this.ccs) c.ResumeLayout();
            this.ccs = null;
        }
    }
}
