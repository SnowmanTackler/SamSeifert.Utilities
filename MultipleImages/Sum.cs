using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class MultipleImages
    {
        /// <summary>
        /// Postive signs are added, negative signs are subtracted
        /// </summary>
        /// <param name="inpt"></param>
        /// <param name="signs"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Sum(Sect[] inpt, Boolean[] signs, ref Sect outp)
        {
            var ret = MatchSectTypes(inpt, ref outp);
            if (ret != ToolboxReturn.Good) return ret;

            if (signs == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt.Length != signs.Length)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }

            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            Action<Sect, Sect, Boolean> act = (Sect anon_inpt, Sect anon_outp, bool sign) =>
            {
                float mult = sign ? 1 : -1;
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        anon_outp[y, x] += mult * anon_inpt[y, x];
            };

            if (outp._Type == SectType.Holder)
            {
                foreach (var sect_out in (outp as SectHolder).Sects.Values)
                {
                    (sect_out as SectArray).SetValue(0);
                    for (int i = 0; i < inpt.Length; i++)
                    {                        
                        Sect sect_in = inpt[i];                    
                        if (sect_in._Type == SectType.Holder) sect_in = (sect_in as SectHolder).Sects[sect_out._Type];
                        act(sect_in, sect_out, signs[i]);
                    }
                }
            }
            else
            {
                (outp as SectArray).SetValue(0);
                for (int i = 0; i < inpt.Length; i++)
                {
                    Sect sect_in = inpt[i];
                    act(sect_in, outp, signs[i]);
                }
            }

            return ToolboxReturn.Good;
        }
    }
}
