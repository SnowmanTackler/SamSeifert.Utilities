using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class ObjectE
    {
        public static void AssertNotNull(this object o)
        {
            if (o == null)
            {
                throw new Exception("Nonnull object was null");
            }
        }
    }
}
