using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.CustomControls
{
    public class ButtonDown : System.Windows.Forms.Button
    {
        public bool _Pressed { get; private set; } = false;

        public ButtonDown() : base()
        {
            this.MouseDown += ButtonDown_MouseDown;
            this.MouseUp += ButtonDown_MouseUp;
            this.MouseLeave += ButtonDown_MouseLeave;
        }

        private void ButtonDown_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this._Pressed = false;
        }

        private void ButtonDown_MouseLeave(object sender, EventArgs e)
        {
            this._Pressed = false;
        }

        private void ButtonDown_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this._Pressed = true;
        }
    }
}
