using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using SamSeifert.Utilities.DataStructures;

namespace SamSeifert.Utilities
{
    public delegate void GlobalWheel(GlobalEvents.WheelArgs evnt);

    public class GlobalEvents : IMessageFilter
    {
        private static GlobalEvents Instance = new GlobalEvents();

        private GlobalEvents()
        {
            Application.AddMessageFilter(this);
        }

        private bool clickedL = false;
        private Point _LastCursorPosition = new Point();
        private static DefaultDict<Keys, bool> _KeysPressed = new DefaultDict<Keys, bool>(false);

        public class WheelArgs
        {
            /// <summary>
            /// Set to true if you want this event to not continue propogating
            /// </summary>
            public bool _Handled = false;
            public readonly int _Delta;
            public readonly Point _CursorPosition;
            public readonly IntPtr _Target;

            public WheelArgs(int delta, IntPtr target, Point cursor_position)
            {
//                this._Delta = delta;
//                this._Target = target;
//                this._CursorPosition = cursor_position;
            }
        }

        private event GlobalWheel _MouseWheel;
        public static event GlobalWheel MouseWheel
        {
            add
            {
                lock (Instance)
                {
                    Instance._MouseWheel += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._MouseWheel -= value;
                }
            }
        }


        private event MouseEventHandler _LMouseDown;
        public static event MouseEventHandler LMouseDown
        {
            add
            {
                lock (Instance)
                {
                    Instance._LMouseDown += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._LMouseDown -= value;
                }
            }
        }

        private event MouseEventHandler _LMouseUp;
        public static event MouseEventHandler LMouseUp
        {
            add
            {
                lock (Instance)
                {
                    Instance._LMouseUp += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._LMouseUp -= value;
                }
            }
        }

        private event MouseEventHandler _LMouseDrag;
        public static event MouseEventHandler LMouseDrag
        {
            add
            {
                lock (Instance)
                {
                    Instance._LMouseDrag += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._LMouseDrag -= value;
                }
            }
        }

        private event MouseEventHandler _MouseMove;
        /// <summary>
        /// Move includes movement with mouse down and mouse up!
        /// </summary>
        public static event MouseEventHandler MouseMove
        {
            add
            {
                lock (Instance)
                {
                    Instance._MouseMove += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._MouseMove -= value;
                }
            }
        }

        private event EventHandler _KeyDown;
        public static event EventHandler KeyDown
        {
            add
            {
                lock (Instance)
                {
                    Instance._KeyDown += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._KeyDown -= value;
                }
            }
        }

        private event EventHandler _KeyUp;
        public static event EventHandler KeyUp
        {
            add
            {
                lock (Instance)
                {
                    Instance._KeyUp += value;
                }
            }
            remove
            {
                lock (Instance)
                {
                    Instance._KeyUp -= value;
                }
            }
        }

        public static Point MouseLocation
        {
            get
            {
                lock (Instance)
                {
                    return Instance._LastCursorPosition;
                }
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            Keys ks;
            lock (this)
            {
                switch (m.Msg)
                {
                    case WM_LBUTTONDOWN:
                        this._LMouseDown?.Invoke(null, new MouseEventArgs(
                            MouseButtons.Left,
                            1,
                            this._LastCursorPosition.X,
                            this._LastCursorPosition.Y,
                            0));
                        this._LastCursorPosition = Cursor.Position;
                        this.clickedL = true;
                        break;
                    case WM_LBUTTONUP:
                        this._LMouseUp?.Invoke(null, new MouseEventArgs(
                            MouseButtons.Left,
                            1,
                            this._LastCursorPosition.X,
                            this._LastCursorPosition.Y,
                            0));
                        this.clickedL = false;
                        break;
                    case WM_MOUSEMOVE:
                        var new_pos = Cursor.Position;
                        Point drag = new Point(
                            new_pos.X - this._LastCursorPosition.X,
                            new_pos.Y - this._LastCursorPosition.Y);
                        this._LastCursorPosition = new_pos;

                        if (this.clickedL)
                            this._LMouseDrag?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, drag.X, drag.Y, 0));

                        this._MouseMove?.Invoke(this, new MouseEventArgs(
                            MouseButtons.None,
                            1,
                            this._LastCursorPosition.X,
                            this._LastCursorPosition.Y,
                            0));

                        break;
                    case WM_MOUSEWHEEL:
                        {
                            // m.HWnd corresponds to a control.
                            // m.WParam corresponds to wheel
                            
                            // Point mouseAbsolutePosition = new Point(m.LParam.ToInt32());
                            // Point mouseRelativePosition = mCtrl.PointToClient(mouseAbsolutePosition);

                            // IntPtr hControlUnderMouse = WindowFromPoint(mouseAbsolutePosition);
                            // Control controlUnderMouse = Control.FromHandle(hControlUnderMouse);

                            // MouseButtons buttons = GetMouseButtons(m.WParam.ToInt32());

                            /*
                            Logger.WriteLine(Convert.ToString(m.WParam.ToInt64(), 2));
                            var e = new WheelArgs(
                                (int)(m.WParam.ToInt64() >> 16), // delta
                                m.HWnd,
                                this._LastCursorPosition
                                );
                            this._MouseWheel?.Invoke(e);
                            if (e._Handled) return true;
                            */
                            break;
                        }
                    case WM_KEYDOWN:
                        ks = (Keys)(m.WParam);
                        GlobalEvents._KeysPressed[ks] = true;
                        // if (!this.isKeyPressed(ks))
                        this._KeyDown?.Invoke(null, EventArgs.Empty);
                        break;
                    case WM_KEYUP:
                        ks = (Keys)(m.WParam);
                        GlobalEvents._KeysPressed[ks] = false;
                        this._KeyUp?.Invoke(null, EventArgs.Empty);
                        break;
                }
            }
            return false;
        }

        private static MouseButtons GetMouseButtons(int wParam)
        {
            MouseButtons buttons = MouseButtons.None;

            if (HasFlag(wParam, 0x0001)) buttons |= MouseButtons.Left;
            if (HasFlag(wParam, 0x0010)) buttons |= MouseButtons.Middle;
            if (HasFlag(wParam, 0x0002)) buttons |= MouseButtons.Right;
            if (HasFlag(wParam, 0x0020)) buttons |= MouseButtons.XButton1;
            if (HasFlag(wParam, 0x0040)) buttons |= MouseButtons.XButton2;

            return buttons;
        }

        private static bool HasFlag(int input, int flag)
        {
            return (input & flag) == flag;
        }

        public static bool isKeyPressed(Keys k)
        {
            lock (Instance)
            {
                return GlobalEvents._KeysPressed[k];
            }
        }

        public static bool isNumberPressed(int i)
        {
            if (i < 0) return false;
            if (i > 9) return false;
            lock (Instance)
                return GlobalEvents._KeysPressed[_NumberKeys[i]];
        }

        /// <summary>
        /// When we are crossing multiple forms the key presses can get lost on new forms.
        /// </summary>
        /// <param name="p"></param>
        public static void ForceDown(Keys p)
        {
            lock (Instance)
            {
                GlobalEvents._KeysPressed[p] = false;
            }
        }


        private readonly static Keys[] _NumberKeys = new Keys[]
        {
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
        };

        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        private const int WH_MOUSE = 7;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        private const int WH_KEYBOARD = 2;

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        private const int WM_MOUSEMOVE = 0x200;

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        private const int WM_LBUTTONUP = 0x202;

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        private const int WM_RBUTTONUP = 0x205;

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONUP = 0x208;

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        private const int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        private const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        #endregion
    }
}
