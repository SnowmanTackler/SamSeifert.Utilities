using SamSeifert.Utilities; using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class ControlHolder<T>
    {
        private String _String;
        public T _Held;

        public override string ToString()
        {
            return this._String;
        }

        public ControlHolder(T ai, String s = null)
        {
            this._Held = ai;

            if (s == null) this._String = this._Held.ToString();
            else this._String = s;
        }
    }    
}
