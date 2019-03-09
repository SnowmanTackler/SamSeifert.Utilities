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
                var exe_path = Application.ExecutablePath;

                const string pf = "Program Files";
                if (exe_path.Contains(pf)) // When Installed!  Use same directory structure as EXE in Program files, but in AppData (all users)
                {
                    var ls = new List<String>();

                    String dir = exe_path;
                    String last_part = null;

                    while (true)
                    {
                        dir = Path.GetDirectoryName(dir);
                        if (dir == null) break;
                        last_part = Path.GetFileName(dir);
                        if (last_part.Contains(pf)) break;
                        ls.Insert(0, last_part);
                    }

                    ls.Insert(0, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

                    return Path.Combine(ls.ToArray());
                }
                else return Path.GetFullPath(Path.Combine(
                        exe_path,
                        "..",
                        "..",
                        "..",
                        "Resources Generated"));
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
