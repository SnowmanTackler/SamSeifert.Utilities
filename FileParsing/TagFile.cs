using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.Utilities.FileParsing
{
    public interface TagItem
    {
        void Display();
        void Display(String prec);
        IEnumerable<TagItem> Enumerate();
    }

    public class TagText : TagItem
    {
        public String text;

        public TagText(String input, out bool valid)
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
            this.text = input.Replace("\n", " ");;
            valid = true;
        }

        public void Display()
        {
            this.Display(" ");
        }

        public void Display(String prec)
        {
            Console.Write((prec.Length - 1).ToString("00"));
            Console.Write(prec);
            Console.Write("\"");
            Console.Write(this.text);
            Console.Write("\"");
            Console.Write(Environment.NewLine);
        }

        public IEnumerable<TagItem> Enumerate()
        {
            yield return this;
        }
    }

    public class TagFile : TagItem
    {
        public String _Name = "";
        public TagItem[] _Children = new TagItem[] { };
        public Dictionary<String, String> _Params = new Dictionary<String, String>();
        public List<String> _ParamsOrder = new List<String>();

        const char TB_OPEN = '<';
        const char TB_CLOSE = '>';
        const char QUOTE = '"';
        const char SPACE = ' ';
        const char EQUALS = '=';
        const char FSLASH = '/';
        const char QMARK = '?';

        private TagFile()
        {

        }

        public IEnumerable<TagItem> Enumerate()
        {
            yield return this;
            foreach (var child in this._Children)
                foreach (var en in child.Enumerate())
                    yield return en;
        }

        public override string ToString()
        {
            return this._Name;
        }

        public static TagFile ParseText(String input)
        {
            var i = input.ToCharArray();
            int s = 0;

            TagFile tbf = new TagFile();

            tbf.ParseText(ref i, ref s);

            return tbf;
        }

        private void ParseText(ref Char[] input, ref int start)
        {
            var cStart = start;

            var list = new List<TagFile>();
            int length = 0;

            char charCurrent = ' ';
            char charLast = ' ';

            bool inBracket = false;
            bool inBracketName = false;
            bool inQuote = false;

            string cKey = "";
            string cValue = "";
            TagFile cTBF = null;

            var chillis = new List<TagItem>();

            int text_start = -1;
            int text_length = 0;

            while (start < input.Length)
            {
                charLast = charCurrent;
                charCurrent = input[start++];

                length = start - cStart - 1;

                if (inQuote)
                {
                    if (charCurrent.Equals(QUOTE))
                    {
                        cValue = length > 0 ? new String(input, cStart, length).Trim() : "";
                        cStart = start;
                        inQuote = false;
                        cTBF._ParamsOrder.Add(cKey);
                        if (!cTBF._Params.ContainsKey(cKey)) cTBF._Params[cKey] = cValue;
                        //else Console.WriteLine("Replacing " + cTBF._Params[cKey] + " with " + cValue);
                    }
                }
                else if (inBracketName)
                {
                    if (charCurrent.Equals(SPACE))
                    {
                        cTBF._Name = length > 0 ? new String(input, cStart, length).Trim() : "";
                        cStart = start;
                        inBracketName = false;
                        inBracket = true;
                    }
                    if (charCurrent.Equals(TB_CLOSE))
                    {
                        if (charLast.Equals(FSLASH))
                        {
                            cTBF._Name = length > 0 ? new String(input, cStart, length - 1).Trim() : "";
                        }
                        else if (charLast.Equals(QMARK))
                        {
                            cTBF._Name = length > 0 ? new String(input, cStart, length - 1).Trim() : "";
                        }
                        else
                        {
                            cTBF._Name = length > 0 ? new String(input, cStart, length).Trim() : "";
                            cTBF.ParseText(ref input, ref start);
                        }

                        chillis.Add(cTBF);
                        cTBF = null;
                        inBracketName = false;
                        cStart = start;
                    }
                }
                else if (inBracket)
                {
                    if (charCurrent.Equals(EQUALS))
                    {
                        cKey = length > 0 ? new String(input, cStart, length).Trim() : "";
                        cStart = start;
                    }
                    else if (charCurrent.Equals(QUOTE))
                    {
                        cStart = start;
                        inQuote = true;
                    }
                    else if (charCurrent.Equals(TB_CLOSE))
                    {
#pragma warning disable CS0642 // Possible mistaken empty statement
                        if (charLast.Equals(FSLASH)) ;
                        else if (charLast.Equals(QMARK)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement
                        else cTBF.ParseText(ref input, ref start);

                        chillis.Add(cTBF);
                        cTBF = null;
                        inBracket = false;
                        cStart = start;
                    }
                }
                else
                {
                    bool open = charCurrent.Equals(TB_OPEN);
                    bool close_bracket = false;

                    if (start < input.Length)
                        close_bracket = input[start] == FSLASH;

                    if (open)
                    {
                        if (text_start > 0)
                        {
                            bool add;
                            String so = new String(input, text_start, text_length);
                            var tfa = new TagText(so, out add);
                            if (add) chillis.Add(tfa);
                            text_start = -1;
                        }

                        if (close_bracket)
                        {
                            while (start < input.Length && !charCurrent.Equals(TB_CLOSE))
                                charCurrent = input[start++];
                            break;
                        }
                        else
                        {
                            inBracketName = true;
                            cTBF = new TagFile();
                            cStart = start;
                        }
                    }
                    else
                    {
                        if (text_start < 0)
                        {
                            text_start = start - 1;
                            text_length = 1;
                        }
                        else text_length++;
                    }
                }
            }

            this._Children = chillis.ToArray();
        }


        public void Display()
        {
            this.Display(" ");
        }

        public void Display(String prec)
        {
            Console.Write((prec.Length - 1).ToString("00"));
            Console.Write(prec);
            Console.Write(this._Name);
            Console.Write(Environment.NewLine);

            for (int i = 0; i < this._ParamsOrder.Count; i++ )
            {
                var key = this._ParamsOrder[i];
                var val = this._Params[key];
                Console.Write(prec);
                Console.Write("   > ");
                Console.Write(key);
                Console.Write(" : ");
                Console.Write(val);
                Console.Write(Environment.NewLine);
            }

            prec += " ";

            if (this._Children.Length > 0)
                foreach (var f in this._Children)
                    f.Display(prec);
        }









        public List<TagFile> getMatches(String hit)
        {
            return this.getMatches(ref hit);
        }


        public List<TagFile> getMatches(ref String hit)
        {
            var ls = new List<TagFile>();

            if (this._Name.Equals(hit)) ls.Add(this);
            else 
                foreach (var c in this._Children)
                    if (c is TagFile)
                        ls.AddRange((c as TagFile).getMatches(ref hit));

            return ls;
        }

        public List<TagFile> getMatches(String hit1, String hit2)
        {
            return this.getMatches(ref hit1, ref hit2);            
        }

        public List<TagFile> getMatches(ref String hit1, ref String hit2)
        {
            var ls = new List<TagFile>();

            if (this._Name.Equals(hit1)) ls.AddRange(this.getMatches(ref hit2));
            else
                foreach (var c in this._Children) 
                    if (c is TagFile)
                        ls.AddRange((c as TagFile).getMatches(ref hit1, ref hit2));

            return ls;
        }

        public void getStringXML(out String front, out string back)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(this._Name);

            for (int i = 0; i < this._ParamsOrder.Count; i++)
            {
                var key = this._ParamsOrder[i];
                var val = this._Params[key];
                sb.Append(" ");
                sb.Append(key);
                sb.Append("=\"");
                sb.Append(val);
                sb.Append("\"");
            }

            sb.Append(">");

            front = sb.ToString();
            back = "</" + this._Name + ">";
        }

        public void getStringXML(out String middle)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(this._Name);

            for (int i = 0; i < this._ParamsOrder.Count; i++)
            {
                var key = this._ParamsOrder[i];
                var val = this._Params[key];
                sb.Append(" ");
                sb.Append(key);
                sb.Append("=\"");
                sb.Append(val);
                sb.Append("\"");
            }

            sb.Append("/>");
            middle = sb.ToString();
        }
    }
}
