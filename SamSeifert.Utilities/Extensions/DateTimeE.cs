using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class DateTimeUtil
    {
        public static int isMonth(string test)
        {
            switch (test.ToLower())
            {
                case "january": return 01;
                case "february": return 02;
                case "march": return 03;
                case "april": return 04;
                case "may": return 05;
                case "june": return 06;
                case "july": return 07;
                case "august": return 08;
                case "september": return 09;
                case "october": return 10;
                case "november": return 11;
                case "december": return 12;
                default: return 0;
            }
        }

        public static int isDayOfWeek(string test)
        {
            switch (test.ToLower())
            {
                case "sunday": return 1;
                case "monday": return 2;
                case "tuesday": return 3;
                case "wednesday": return 4;
                case "thursday": return 5;
                case "friday": return 6;
                case "saturday": return 7;
                default: return 0;
            }
        }
    }
}
