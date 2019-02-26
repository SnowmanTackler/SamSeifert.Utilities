﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.Utilities.FileParsing
{
    public class BracketFile
    {
        public String text;
        public int bracketType;
        public BracketFile[] _Children;

        public const char bracketType1Open = '{';
        public const char bracketType1Close = '}';
        public const char bracketType2Open = '[';
        public const char bracketType2Close = ']';

        public static BracketFile ParseText(String input)
        {
            try
            {
                Logger.WriteLine(input.Length);

                var ca = input.ToCharArray();
                int start = 0;

                BracketFile f = BracketFile.ParseText(ref ca, "", 0, ref start);

                if (f._Children.Length == 1) return f._Children[0];
                else return f;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static BracketFile ParseText(ref char[] input, string head, int btype, ref int start)
        {
            BracketFile f = new BracketFile();

            f.bracketType = btype;
            f.text = head;

            var contextstart = start;

            var list = new List<BracketFile>();
            int length = 0;
            char s = ' ';

            while (start < input.Length)
            {
                s = input[start++];

                length = start - contextstart - 1;

                if (s.Equals(bracketType1Open))
                {
                    list.Add(BracketFile.ParseText(
                        ref input,
                        length > 0 ? new String(input, contextstart, length) : "",
                        1,
                        ref start));
                    contextstart = start + 1;
                }
                else if (s.Equals(bracketType2Open))
                {
                    list.Add(BracketFile.ParseText(
                        ref input,
                        length > 0 ? new String(input, contextstart, length) : "",
                        2,
                        ref start));
                    contextstart = start + 1;
                }
                else if (btype == 1 && s.Equals(bracketType1Close))
                {
                    break;
                }
                else if (btype == 2 && s.Equals(bracketType2Close))
                {
                    break;
                }
            }

            if (start - contextstart > 0)
            {
                BracketFile f2 = new BracketFile();
                f2._Children = new BracketFile[] { };
                f2.text = length > 0 ? new String(input, contextstart, length) : "";
                f2.NiceStrings();
                list.Add(f2);
            }

            f.NiceStrings();
            f._Children = list.ToArray();

            return f;
        }

        private void NiceStrings()
        {
            this.text = this.text.Trim();
            this.text = this.text.Replace('\t', ' ');

            int oldLength = 0;
            while (oldLength != this.text.Length)
            {
                oldLength = this.text.Length;
                this.text = this.text.Replace("  ", " ");
            }
        }

        public void Save()
        {
            var x = new List<String>();

            this.Save(0, ref x);

            var sf = new System.Windows.Forms.SaveFileDialog();

            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                System.IO.File.WriteAllLines(sf.FileName, x);
        }

        private void Save(int level, ref List<string> str)
        {
            String s = level.ToString("00 ");
            for (int i = 0; i < level; i++) s += " ";

            if (this.text.Length < 150) s += this.text;
            else s += " XXXX " + this.text.Length;

            str.Add(s);

            if (this._Children.Length > 0)
            {
                foreach (var f in this._Children) f.Save(level + 1, ref str);
            }
        }

        public void Display(bool contents)
        {
            this.Display(0, contents);
        }

        private void Display(int level, bool contents)
        {
            String s = level.ToString("00 ");
            for (int i = 0; i < level; i++) s += " ";

            if ((contents) || (this.text.Length < 150)) s += this.text;
            else s += " XXXX " + this.text.Length;

            Logger.WriteLine(s);

            if (this._Children.Length > 0)
            {
                foreach (var f in this._Children) f.Display(level + 1, contents);
            }
        }














        public List<BracketFile> getMatches(ref String hit)
        {
            var ls = new List<BracketFile>();

            if (this.text.Equals(hit)) ls.Add(this);
            else foreach (var c in this._Children) ls.AddRange(c.getMatches(ref hit));

            return ls;
        }

        public List<BracketFile> getMatches(ref String hit1, ref String hit2)
        {
            var ls = new List<BracketFile>();

            if (this.text.Equals(hit1)) ls.AddRange(this.getMatches(ref hit2));
            else foreach (var c in this._Children) ls.AddRange(c.getMatches(ref hit1, ref hit2));

            return ls;
        }
    }
}