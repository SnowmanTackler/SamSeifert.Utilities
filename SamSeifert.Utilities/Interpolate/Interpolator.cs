using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Interpolate
{
    public interface Interpolator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTime">Usually Environment.TickCount</param>
        /// <returns></returns>
        double evaluate(int currentTime);
    }
}
