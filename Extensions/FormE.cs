using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class FormE
    {
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
