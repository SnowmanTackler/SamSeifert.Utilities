using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.Utilities.Files.Xml
{
    public class TagFile : TagItem
    {
        public String _Name = "";
        public TagItem[] _Children = new TagItem[] { };
        public readonly Dictionary<String, String> _Params = new Dictionary<String, String>();
        public readonly List<String> _ParamsOrder = new List<String>();

        const char TB_OPEN = '<';
        const char TB_CLOSE = '>';
        const char QUOTE = '"';
        const char SPACE = ' ';
        const char EQUALS = '=';
        const char FSLASH = '/';
        const char QMARK = '?';

        private TagFile(TagFile parent)
        {
            this._Parent.SetTarget(parent);
        }

        public TagFile(params TagFile[] children)
        {
            this._Children = children;
        }

        public override IEnumerable<TagItem> Enumerate()
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
                            var tfa = new TagText(this, so, out add);
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
                            cTBF = new TagFile(this);
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

        protected override bool MatchesName(string name)
        {
            return String.Equals(name, this._Name);
        }

        public override void Display(String prec)
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

                if (val.Length < 100)
                    Console.Write(val);
                else
                    Console.Write(val.Substring(0, 100));

                Console.Write(Environment.NewLine);
            }

            prec += " ";

            if (this._Children.Length > 0)
                foreach (var f in this._Children)
                    f.Display(prec);
        }








        /// <summary>
        /// Forces all children to be sequential depths, not neccesarily right after root.
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        public IEnumerable<TagFile> getMatchesAtAnyDepth(params String[] hits)
        {
            return this.getMatches(false, hits);
        }

        /// <summary>
        /// Forces all children to be sequential depths, and right after root.
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        public IEnumerable<TagFile> getMatchesAtZeroDepth(params String[] hits)
        {
            string[] nhits = new string[hits.Length + 1];
            Array.Copy(hits, 0, nhits, 1, hits.Length);
            nhits[0] = this._Name;
            return this.getMatches(true, nhits);
        }

        /// <summary>
        /// Doesn't force sequential at all!
        /// Can go hit1, deep1, deep2, hit2, deep3, hit3
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        public IEnumerable<TagFile> getGapMatches(params String[] hits)
        {
            bool irrelevant = false;
            return getMatches(irrelevant, hits, true);
        }

        private IEnumerable<TagFile> getMatches(bool force_next, String[] hits, bool gap_allowed = false)
        {
            if (hits == null) throw new Exception("TagFile getMatches null input");
            if (hits.Length == 0) throw new Exception("TagFile getMatches 0 length input");

            String hit = hits[0];

            if (this._Name.Equals(hit))
            {
                if (hits.Length == 1) yield return this;
                else
                {
                    var new_hits = hits.SubArray(1);
                    foreach (var c in this._Children)
                        if (c is TagFile)
                            foreach (var cc in (c as TagFile).getMatches(true, new_hits, gap_allowed))
                                yield return cc;
                }
            }
            else if ((!force_next) || gap_allowed) // has to be sequential once we find root, otherwise, go deeper
            {
                foreach (var c in this._Children)
                    if (c is TagFile)
                        foreach (var cc in (c as TagFile).getMatches(false, hits, gap_allowed))
                            yield return cc;
            }
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
