using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Json;

namespace SamSeifert.Utilities.Extensions
{
    public static class FormE
    {
        const string default_save_name = "default_form_state.local.json";

        public static void LoadFormState(this Form f, string file_name = default_save_name)
        {
            try
            {
                var jd = Json.JsonDict.FromString(TextSettings.Read(file_name));

                var location = jd.asGeneric<object[]>("Location");
                f.Location = new System.Drawing.Point(location.asInt(0), location.asInt(1));

                f.StartPosition = FormStartPosition.Manual;

                var size = jd.asGeneric<object[]>("Size");
                f.Size = new System.Drawing.Size(size.asInt(0), size.asInt(1));

                var fws = (FormWindowState)jd.asInt("FormWindowState");
                if (fws == FormWindowState.Maximized)
                    f.WindowState = FormWindowState.Maximized;
            }
            catch (Exception exc)
            {
                Logger.WriteException(f, "LoadFormState", exc);
            }
        }

        public static void SaveFormState(this Form f, string file_name = default_save_name)
        {
            var jd = new JsonDict();
            jd["Location"] = new object[] { f.Location.X, f.Location.Y };
            jd["Size"] = new object[] { f.Size.Width, f.Size.Height };
            jd["FormWindowState"] = Convert.ToInt32((f.WindowState == FormWindowState.Minimized) ? FormWindowState.Normal : f.WindowState);
            TextSettings.Save(file_name, jd.ToString());
        }
    }
}
