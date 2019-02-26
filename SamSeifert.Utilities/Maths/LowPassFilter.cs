using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Maths
{
    public class LowPassFilter
    {
        private readonly float _Alpha;
        private bool _First = true;
        private float _Value = 0;

        /// <summary>
        /// New values multiplied by (alpha).  
        /// Old Values multiplied by (alpha - 1).
        /// </summary>
        /// <param name="alpha"></param>
        public LowPassFilter(float alpha = 0.1f)
        {
            this._Alpha = alpha;
        }

        public void Reset()
        {
            this._First = true;
        }

        public float Update(float input)
        {
            if (this._First)
            {
                this._First = false;
                this._Value = input;
            }
            else
            {
                this._Value *= (1 - this._Alpha);
                this._Value += this._Alpha * input;
            }
            return this._Value;
        }
    }
}
