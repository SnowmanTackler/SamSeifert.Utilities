using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using SamSeifert.Utilities;

namespace SamSeifert.Utilities.Json
{
    public interface JsonPackable
    {
        JsonDict Pack();
        void Unpack(JsonDict dict);
    }
}
