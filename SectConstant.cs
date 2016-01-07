using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.CSCV
{
    public class SectConstant : Sect
    {
        private readonly Single value;

        public SectConstant(Single v, SectType t) : base(t)
        {
            this.value = v;
        }

        public override Boolean isSquishy()
        {
            return true;
        }

        public override Size getPrefferedSize()
        {
            return new Size(1, 1);
        }

        public override Single this[int x, int y]
        {
            get
            {
                return this.value;
            }
        }

        public override Sect Clone()
        {
            return new SectConstant(this.value, this._Type);
        }

        public override Single min
        {
            get
            {
                return this.value;
            }
        }

        public override Single max
        {
            get
            {
                return this.value;
            }
        }

        public override Single avg
        {
            get
            {
                return this.value;
            }
        }

        public override void reset()
        {
        }
    }
}
