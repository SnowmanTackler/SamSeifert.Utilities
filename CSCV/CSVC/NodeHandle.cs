
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CSCV_IDE
{
    interface NodeHandle
    {
        Point LocationCustom();
        Boolean Contains(Point p);
    }
}
