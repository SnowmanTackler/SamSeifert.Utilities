using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

using SamSeifert.CSCV;

namespace CSCV_IDE
{
    public class NodeData
    {
        public readonly DataType _Type;
        
        public NodeData(DataType t)
        {
            this._Type = t;
        }

        public enum DataType
        {
            Sect,
            Size
        }
    }

    public class NodeDataSect : NodeData
    {
        public NodeDataSect(Sect s) : base(DataType.Sect) { this._Sect = s; }
        public Sect _Sect;
    }

    public class NodeDataSize : NodeData
    {
        public NodeDataSize(Size s) : base(DataType.Size) { this._Size = s; }
        public Size _Size;
    }
}
