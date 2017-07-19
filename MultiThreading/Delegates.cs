using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.MultiThreading
{
    internal delegate void BackgroundQueueMethod(Form f, ContinueCheck continue_on);

    /// <summary>
    /// Returns true if thread should keep going.
    /// </summary>
    /// <returns></returns>
    public delegate bool ContinueCheck();

    public delegate void   BackgroundThreadMethod(ContinueCheck continue_on);
    public delegate object BackgroundThreadMethodOut(ContinueCheck continue_on);
    public delegate void   ParameterizedBackgroundThreadMethod(ContinueCheck continue_on, object x);
    public delegate object ParameterizedBackgroundThreadMethodOut(ContinueCheck continue_on, object x);

    public delegate void BackgroundThreadFinisher(object x);
}
