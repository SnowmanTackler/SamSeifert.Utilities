using SamSeifert.Utilities.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class NumericUpDownE
    {
        public static int ValueI(this NumericUpDown nud)
        {
            return (int)Math.Round(nud.Value);
        }

        public static float ValueF(this NumericUpDown nud)
        {
            return (float)nud.Value;
        }

        public static double ValueD(this NumericUpDown nud)
        {
            return (double)nud.Value;
        }





        public static void SetValueMinMaxSafe(this NumericUpDown nud, int value)
        {
            nud.SetValueMinMaxSafe((Decimal)value);
        }

        public static void SetValueMinMaxSafe(this NumericUpDown nud, float value)
        {
            nud.SetValueMinMaxSafe((Decimal)value);
        }

        public static void SetValueMinMaxSafe(this NumericUpDown nud, double value)
        {
            nud.SetValueMinMaxSafe((Decimal)value);
        }

        public static void SetValueMinMaxSafe(this NumericUpDown nud, Decimal value)
        {
            nud.Value = value.Clampp(nud.Minimum, nud.Maximum);
        }




        public static void AddValueMinMaxSafe(this NumericUpDown nud, Decimal value)
        {
            nud.SetValueMinMaxSafe(nud.Value + value);
        }







        public static void Unpack(this NumericUpDown nud, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                nud.SetValueMinMaxSafe((decimal)(double)outo);
        }

        public static void Pack(this NumericUpDown nud, JsonDict dict, string key)
        {
            dict[key] = (double)nud.Value;
        }
    }
}
