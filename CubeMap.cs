using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE
{
    public class CubeMap : DeleteableObject
    {
        private FrameBuffers _FrameBufferLeft;
        private FrameBuffers _FrameBufferFront;
        private FrameBuffers _FrameBufferRight;
        private FrameBuffers _FrameBufferBack;
        private FrameBuffers _FrameBufferTop;
        private FrameBuffers _FrameBufferBottom;

        public CubeMap(Size resolution, out bool sucess)
        {
            this._FrameBufferLeft = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferFront = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferRight = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferBack = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferTop = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferBottom = new FrameBuffers(resolution, out sucess);
        }

        public void GLDelete()
        {
            if (this._FrameBufferLeft != null) this._FrameBufferLeft.GLDelete();
            if (this._FrameBufferFront != null) this._FrameBufferFront.GLDelete();
            if (this._FrameBufferRight != null) this._FrameBufferRight.GLDelete();
            if (this._FrameBufferBack != null) this._FrameBufferBack.GLDelete();
            if (this._FrameBufferBack != null) this._FrameBufferBack.GLDelete();
            if (this._FrameBufferTop != null) this._FrameBufferTop.GLDelete();
            if (this._FrameBufferBottom != null) this._FrameBufferBottom.GLDelete();
        }
    }
}
