using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class RichTextBoxE
    {
        /// <summary>
        /// Appends to end
        /// </summary>
        /// <param name="box"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void AppendText(this RichTextBox box, char text, Color color)
        {
            box.AppendText(text.ToString(), color);
        }

        /// <summary>
        /// Appends to end
        /// </summary>
        /// <param name="box"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;

            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        /// <summary>
        /// Inserts at caret
        /// </summary>
        /// <param name="box"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void InsertText(this RichTextBox box, char text, Color color)
        {
            box.InsertText(text.ToString(), color);
        }

        /// <summary>
        /// Inserts at caret
        /// </summary>
        /// <param name="box"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void InsertText(this RichTextBox box, string text, Color color)
        {
            box.SelectionColor = color;
            box.SelectedText = text;
        }
    }
}
