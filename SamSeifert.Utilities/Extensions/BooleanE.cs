using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class BooleanE
    {
        public static void AssertFalse(this bool b)
        {
            if (b)
            {
                throw new Exception("Not false!");
            }
        }

        public static void AssertTrue(this bool b)
        {
            if (!b)
            {
                throw new Exception("Not true!");
            }
        }

        public static void AssertEqual(this bool b, bool other)
        {
            if (b != other)
            {
                throw new Exception("Not equal!");
            }
        }
    }
}
