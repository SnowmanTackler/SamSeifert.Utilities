using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities
{
    public static class TextSettings
    {
        public static String Folder
        {
            get
            {
                var pa = System.IO.Path.Combine(
                    Application.ExecutablePath,
                    "..",
                    "..",
                    "..",
                    "Settings");

                return Path.GetFullPath(pa);
            }
        }

        public static void Save(string file_name, string text)
        {
            Logger.WriteLine("File Saved: " + file_name);

            var path = Path.Combine(Folder, file_name);

            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, text);
        }

        public static String Read(string file_name)
        {
            string path = Path.Combine(Folder, file_name);
            if (File.Exists(path))
                return File.ReadAllText(path);
            else return null;
        }
    }
}
