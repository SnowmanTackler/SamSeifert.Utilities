﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SamSeifert.Utilities
{
    public class GlobalEvents : IMessageFilter
    {
        private static GlobalEvents Instance = new GlobalEvents();

        private GlobalEvents()
        {
            Application.AddMessageFilter(this);
        }

        private bool clickedL = false;
        private Point pointL = new Point();

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
                    return Instance.pointL;
                }
            }
        }

        private MouseEventArgs getMouseEventArgs()
        {
            return new MouseEventArgs(MouseButtons.Left, 1, Cursor.Position.X, Cursor.Position.Y, 0);
        }

        public bool PreFilterMessage(ref Message m)
        {
            lock (this)
            {
                if (m.Msg == WM_LBUTTONDOWN)
                {
                    if (this._LMouseDown != null) this._LMouseDown(null, this.getMouseEventArgs());
                    this.pointL = Cursor.Position;
                    this.clickedL = true;
                }
                else if (m.Msg == WM_LBUTTONUP)
                {
                    if (this._LMouseUp != null) this._LMouseUp(null, this.getMouseEventArgs());
                    this.clickedL = false;
                }
                else if (m.Msg == WM_MOUSEMOVE)
                {
                    Point drag = new Point(Cursor.Position.X - this.pointL.X, Cursor.Position.Y - this.pointL.Y);

                    if (this.clickedL)
                    {
                        if (this._LMouseDrag != null)
                            this._LMouseDrag(this, new MouseEventArgs(MouseButtons.Left, 1, drag.X, drag.Y, 0));
                    }

                    if (this._MouseMove != null)
                        this._MouseMove(this, this.getMouseEventArgs());

                    this.pointL = Cursor.Position;
                }
                else if (m.Msg == WM_KEYDOWN)
                {
                    Keys ks = (Keys)(m.WParam);
                    GlobalEvents.keyTable[ks] = true;

                    //                if (!this.isKeyPressed(ks))
                    if (this._KeyDown != null)
                        this._KeyDown(null, EventArgs.Empty);
                }
                else if (m.Msg == WM_KEYUP)
                {
                    Keys ks = (Keys)(m.WParam);
                    GlobalEvents.keyTable[ks] = false;
                    if (this._KeyUp != null) this._KeyUp(null, EventArgs.Empty);
                }
                return false;
            }
        }


        private static Dictionary<Keys, bool> keyTable = new Dictionary<Keys, bool>();

        public static bool isKeyPressed(Keys k)
        {
            lock (Instance)
            {
                bool pressed = false;
                GlobalEvents.keyTable.TryGetValue(k, out pressed);
                return pressed;
            }
        }

        /// <summary>
        /// When we are crossing multiple forms the key presses can get lost on new forms.
        /// </summary>
        /// <param name="p"></param>
        public static void ForceDown(Keys p)
        {
            lock (Instance)
            {
                GlobalEvents.keyTable[p] = false;
            }
        }


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
