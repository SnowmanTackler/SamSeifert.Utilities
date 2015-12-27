using SamSeifert.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities
{
    public static class ControlUtil
    {
        public static void WireAllControls_MouseDown(this Control parent, MouseEventHandler eh)
        {
            parent.ToggleAll_MouseDown(eh, true);
        }

        public static void UnwireAllControls_MouseDown(this Control parent, MouseEventHandler eh)
        {
            parent.ToggleAll_MouseDown(eh, false);
        }

        private static void ToggleAll_MouseDown(this Control parent, MouseEventHandler eh, bool add)
        {
            if (add) parent.MouseDown += eh;
            else parent.MouseDown -= eh;
            foreach (Control child in parent.Controls) child.ToggleAll_MouseDown(eh, add);
        }

        public static void CascadeContextMenuStrip(this Control parent)
        {
            parent.CascadeContextMenuStrip(parent.ContextMenuStrip);
        }

        public static void DecascadeContextMenuStrip(this Control parent)
        {
            parent.CascadeContextMenuStrip(null);
        }

        private static void CascadeContextMenuStrip(this Control parent, ContextMenuStrip cms)
        {
            foreach (Control child in parent.Controls)
            {
                child.ContextMenuStrip = cms;
                if (child.HasChildren) child.CascadeContextMenuStrip(cms);
            }
        }

        public static bool ContainsScreenPoint(this Control c, Point p)
        {
            p = c.PointToClient(p);
            if (p.X < 0) return false;
            if (p.Y < 0) return false;
            if (p.X >= c.Width) return false;
            if (p.Y >= c.Height) return false;
            return true;
        }

        public static void RemoveFromParent(this Control c)
        {
            if (c.Parent == null) return;
            c.Parent.Controls.Remove(c);
        }





    }
}
