using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Interpolate
{
    public class LinearInterpolator : Interpolator
    {
        private readonly double ValueStart;
        private double ValueEnd;
        /// <summary>
        /// Miliseconds
        /// </summary>
        private readonly int TimeDuration;
        /// <summary>
        /// Miliseconds
        /// </summary>
        private readonly int TimeStart;

        public LinearInterpolator(
            double valueStart,
            double valueEnd,
            int timeStart,
            int timeDuration)
        {
            this.ValueStart = valueStart;
            this.ValueEnd = valueEnd;
            this.TimeStart = timeStart;
            this.TimeDuration = timeDuration;
        }

        public double evaluate(int currentTime)
        {
            if (currentTime > this.TimeStart + this.TimeDuration)
            {
                return this.ValueEnd;
            }
            else
            {
                var progress01 = (currentTime - this.TimeStart) / (double)this.TimeDuration;
                return this.ValueStart + (this.ValueEnd - this.ValueStart) * progress01;
            } 
        }
    }
}
