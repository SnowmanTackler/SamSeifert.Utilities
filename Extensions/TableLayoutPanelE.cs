using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class TableLayoutPanelE
    {
        public static void RemoveColumn(this TableLayoutPanel p, int column)
        {
            p.SuspendLayout();

            { // Clear controls from current row
                int col = column;
                for (int i = 0; i < p.RowCount; i++)
                {
                    Control c = p.GetControlFromPosition(col, i);
                    if (c != null)
                    {
                        p.Controls.Remove(c);
                        c.Dispose();
                    }
                }
            }

            for (int col = column + 1; col < p.ColumnCount; col++)
            { // Move Controls
                for (int i = 0; i < p.RowCount; i++)
                {
                    Control c = p.GetControlFromPosition(col, i);
                    if (c != null)
                    {
                        p.SetColumn(c, col - 1);
                    }
                }
            }

            p.ColumnStyles.RemoveAt(column);
            p.ColumnCount--;

            p.ResumeLayout(false);
            p.PerformLayout();
        }

        public static void RemoveRow(this TableLayoutPanel p, bool adjust_height = true)
        {
            p.RemoveRow(p.RowCount - 1, adjust_height);
        }

        public static void RemoveRow(this TableLayoutPanel p, int rowww, bool adjust_height = true)
        {
            p.SuspendLayout();

            { // Clear controls from current row
                int row = rowww;
                for (int i = 0; i < p.ColumnCount; i++)
                {
                    Control c = p.GetControlFromPosition(i, row);
                    if (c != null)
                    {
                        p.Controls.Remove(c);
                        c.Dispose();
                    }
                }
            }

            for (int row = rowww + 1; row < p.ColumnCount; row++)
            { // Move Controls
                for (int i = 0; i < p.ColumnCount; i++)
                {
                    Control c = p.GetControlFromPosition(i, row);
                    if (c != null)
                    {
                        p.SetRow(c, row - 1);
                    }
                }
            }

            if (adjust_height) p.Height -= p.GetRowHeights()[rowww];

            p.RowStyles.RemoveAt(rowww);
            p.RowCount--;

            p.ResumeLayout(false);
            p.PerformLayout();
        }
    }
}
