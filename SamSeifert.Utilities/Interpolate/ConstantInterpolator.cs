using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Interpolate
{
    public class ConstantInterpolator : Interpolator
    {
        private readonly double Value;

        public ConstantInterpolator(double value)
        {
            this.Value = value;
        }

        public double evaluate(int currentTime)
        {
            return this.Value;
        }
    }
}
