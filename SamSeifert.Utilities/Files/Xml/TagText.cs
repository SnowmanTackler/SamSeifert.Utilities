using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Files.Xml
{
    public class TagText : TagItem
    {
        public String _Text;

        public TagText(TagFile parent, String input, out bool valid)
        {
            valid = false;
            input = input.Trim();
            if (input.Length == 0) return;
            int last_lens = -1;
            while (last_lens != input.Length)
            {
                last_lens = input.Length;
                input = input.Replace("\n\n", "\n");
            }
            this._Text = input.Replace("\n", " "); ;
            this._Parent.SetTarget(parent);
            valid = true;
        }

        public override void Display(String prec)
        {
            Console.Write((prec.Length - 1).ToString("00"));
            Console.Write(prec);
            Console.Write("\"");
            Console.Write(this._Text);
            Console.Write("\"");
            Console.Write(Environment.NewLine);
        }

        protected override bool MatchesName(string name)
        {
            return null == name;
        }

        public override IEnumerable<TagItem> Enumerate()
        {
            yield return this;
        }
    }
}
