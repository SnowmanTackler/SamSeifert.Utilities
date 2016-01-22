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





        public static void WireAllControls_MouseDoubleClick(this Control parent, MouseEventHandler eh)
        {
            parent.ToggleAll_MouseDoubleClick(eh, true);
        }

        public static void UnwireAllControls_MouseDoubleClick(this Control parent, MouseEventHandler eh)
        {
            parent.ToggleAll_MouseDoubleClick(eh, false);
        }

        private static void ToggleAll_MouseDoubleClick(this Control parent, MouseEventHandler eh, bool add)
        {
            if (add) parent.MouseDoubleClick += eh;
            else parent.MouseDoubleClick -= eh;
            foreach (Control child in parent.Controls) child.ToggleAll_MouseDoubleClick(eh, add);
        }











        public static void CascadeContextMenuStrip(this Control parent)
        {
            parent.CascadeContextMenuStrip(parent.ContextMenuStrip);
        }

        public static void DecascadeContextMenuStrip(this Control parent)
        {
            parent.DeascadeContextMenuStrip(parent.ContextMenuStrip);
        }

        private static void CascadeContextMenuStrip(this Control parent, ContextMenuStrip cms)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.ContextMenuStrip == cms) child.ContextMenuStrip = null;
                if (child.HasChildren) child.CascadeContextMenuStrip(cms);
            }
        }

        private static void DeascadeContextMenuStrip(this Control parent, ContextMenuStrip cms)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.ContextMenuStrip == null) child.ContextMenuStrip = cms;
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










        public static T GetParent<T>(this Control c) where T : class
        {
            if (c is T) return c as T;
            else if (c.Parent == null) return null;
            else return c.Parent.GetParent<T>();
        }

        public static void LoadFormState(this Form f)
        {
            if (Properties.Settings.Default.FormPositionSaved)
            {
                f.Location = Properties.Settings.Default.FormLocation;
                f.StartPosition = FormStartPosition.Manual;
                f.Size = Properties.Settings.Default.FormSize;

                if ((Properties.Settings.Default.FormWindowState != -1) &&
                    (((FormWindowState)(Properties.Settings.Default.FormWindowState)) == FormWindowState.Maximized))
                {
                    f.WindowState = FormWindowState.Maximized;
                }
            }
        }

        public static void SaveFormState(this Form f)
        {
            Properties.Settings.Default.FormPositionSaved = true;
            Properties.Settings.Default.FormLocation = f.Location;
            Properties.Settings.Default.FormSize = f.Size;

            if (f.WindowState == FormWindowState.Minimized) Properties.Settings.Default.FormWindowState = Convert.ToInt32(FormWindowState.Normal);
            else Properties.Settings.Default.FormWindowState = Convert.ToInt32(f.WindowState);

            Properties.Settings.Default.Save();

        }

    }
}
