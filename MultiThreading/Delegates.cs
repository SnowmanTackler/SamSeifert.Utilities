using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.MultiThreading
{
    public delegate bool ContinueCheck();
    public delegate void BackgroundThreadMethod(ContinueCheck continue_on);
    public delegate void BackgroundThreadMethodWithParam(ContinueCheck continue_on, object x);
}
