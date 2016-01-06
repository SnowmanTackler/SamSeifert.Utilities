using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.CSCV
{
    public class SectMask : Sect
    {
        private readonly Sect _Sect;

        public SectMask(SectType t, Sect s)
            : base(t)
        {
            if (s == null) throw new Exception("Null Input");
            if (s._Type == SectType.Holder) throw new Exception("SectMask can't hold a SectHolder");
            if (t == SectType.Holder) throw new Exception("SectMask can't hold a SectHolder");
            this._Sect = s;
        }

        public override Boolean isSquishy()
        {
            return this._Sect.isSquishy();
        }

        public override Size getPrefferedSize()
        {
            return this._Sect.getPrefferedSize();
        }

        public override Single this[int x, int y]
        {
            get
            {
                return this._Sect[x, y];
            }
            set
            {
                this._Sect[x, y] = value;
            }
        }

        public override Sect Clone()
        {
            return new SectMask(this._Type, this._Sect.Clone());
        }

        public override Single min
        {
            get
            {
                return this._Sect.min;
            }
        }

        public override Single max
        {
            get
            {
                return this._Sect.max;
            }
        }

        public override Single avg
        {
            get
            {
                return this._Sect.avg;
            }
        }

        public override void reset()
        {
            this._Sect.reset();
        }
    }
}
