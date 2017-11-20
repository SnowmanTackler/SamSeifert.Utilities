using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public class LayoutSuspender : IDisposable
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private Control[] ccs;

        public LayoutSuspender(params Control[] controls)
        {
            this.ccs = controls;

            foreach (var c in this.ccs)
            {
                SendMessage(c.Handle, WM_SETREDRAW, false, 0); // Prevent Redrawing
                c.SuspendLayout();
            }
        }

        public void Dispose()
        {
            foreach (var c in this.ccs)
            {
                c.ResumeLayout();
                SendMessage(c.Handle, WM_SETREDRAW, true, 0); // Resume Redrawing
                c.Refresh();
            }
            this.ccs = null;
        }

        private const int WM_SETREDRAW = 11;       
    }
}
