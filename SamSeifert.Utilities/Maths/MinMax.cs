using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Maths
{
    public class MinMaxI
    {
        public int max { get; private set; } = int.MinValue;
        public int min { get; private set; } = int.MaxValue;
        public int range { get { return this.max - this.min; } }
        public int center { get { return (this.max + this.min) / 2; } }
        public void Update(int i)
        {
            this.max = Math.Max(i, this.max);
            this.min = Math.Min(i, this.min);
        }
    }

    public class MinMaxF
    {
        public float max { get; private set; } = float.MinValue;
        public float min { get; private set; } = float.MaxValue;
        public float range { get { return this.max - this.min; } }
        public float center { get { return (this.max + this.min) / 2; } }
        public void Update(float i)
        {
            this.max = Math.Max(i, this.max);
            this.min = Math.Min(i, this.min);
        }
    }

    public class MinMaxD
    {
        public double max { get; private set; } = double.MinValue;
        public double min { get; private set; } = double.MaxValue;
        public double range { get { return this.max - this.min; } }
        public double center { get { return (this.max + this.min) / 2; } }
        public void Update(double i)
        {
            this.max = Math.Max(i, this.max);
            this.min = Math.Min(i, this.min);
        }
    }
}
