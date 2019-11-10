using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class Holder<T>
    {
        public T t;

        public Holder(T t)
        {
            this.t = t;
        }

        public void set(T t)
        {
            this.t = t;
        }

        public T get()
        {
            return this.t;
        }
    }
}
