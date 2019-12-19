using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class Assert
    {
        public static void IsTrue(bool b)
        {
            if (b != true)
            {
                throw new Exception("Wasn't true");
            }
        }

        public static void IsFalse(bool b)
        {
            if (b != false)
            {
                throw new Exception("Wasn't false");
            }
        }

        public static void IsNull(Object o)
        {
            if (o != null)
            {
                throw new Exception("Nonnull object was null");
            }
        }
        public static void IsNotNull(Object o)
        {
            if (o == null)
            {
                throw new Exception("Nonnull object was null");
            }
        }

        public static void Equals(Object a, Object b)
        {
            if (!a.Equals(b))
            {
                throw new Exception("Not Equal");
            }
        }

        public static void NotEqual(Object a, Object b)
        {
            if (a.Equals(b))
            {
                throw new Exception("Not Equal");
            }
        }
    }
}
