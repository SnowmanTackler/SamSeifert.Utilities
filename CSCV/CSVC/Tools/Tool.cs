using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Tool : UserControl
    {
        public const int HandleGap = 4;

        public Tool()
        {
            InitializeComponent();
            this._Controls.Add(this);
        }

        public override bool Equals(object obj)
        {
            Tool t = obj as Tool;
            if (t == null) return false;
            return this == t;
        }





        private static object thread_lock = new object();
        private static volatile Thread currentThread;
        private static List<Tool> toUpdate = new List<Tool>();
        private static List<Tool> isUpdating = new List<Tool>();
/*        private class UpdatePair
        {
            public NodeHandleIn nhi;
            public Tool t;
            
            public UpdatePair(NodeHandleIn nhi, Tool t)
            {
                this.nhi = nhi; this.t = t;
            }
            
            public override bool Equals(object obj)
            {
                UpdatePair up = obj as UpdatePair;
                if (up == null) return false;
                return this.t == up.t && this.nhi == up.nhi;
            }
        };*/

        public void StatusChanged()
        {
            Tool.toUpdate.Add(this);
            Tool.ThreadMethodStart();
        }

        private static void ThreadMethodStart() { (new Thread(Tool.ThreadMethod)).Start(); }
        private static void ThreadMethod()
        {
            lock (thread_lock)
            {
                var t = Tool.currentThread;
                Tool.currentThread = Thread.CurrentThread;
                Tool.currentThread.Name = "Image Update Thread";
                if (t != null) t.Join();
            }

            Boolean broken = false;
            FormMain.get().Invoke((MethodInvoker)delegate
            {
                while (Tool.toUpdate.Count > 0)
                {
                    Tool up = Tool.toUpdate[0];
                    Tool.toUpdate.Remove(up);
                    Tool.isUpdating.Add(up);
                    if (up.ClearFutures(0, null))
                    {
                        FormMain.get().setError("INFINITE LOOP DETECTED");
                        broken = true;
                        break;
                    }
                }
            });

            if (!broken)
            {
                while (Tool.isUpdating.Count > 0)
                {
                    if (Tool.currentThread != Thread.CurrentThread)
                    {
                        broken = true;
                        break;
                    }

                    Tool up = Tool.isUpdating[0];

                    up.SpecialBitmapUpdate();

                    Tool.isUpdating.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// This method is always called on main thread
        /// A true return means it was an infinite loop
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool ClearFutures(int i, NodeHandleIn sender)
        {           
            if (i++ > 100) return true;

            if (this.ClearData(sender))
            {
                foreach (var nhi in this.getOutputs())
                {
                    Tool t = nhi.getTool;
                    Tool.isUpdating.Remove(t);
                    Tool.isUpdating.Add(t);

                    if (t.ClearFutures(i, nhi)) return true;
                }
            }
            return false;
        }

        internal static Boolean isWorking()
        {
            if (Tool.currentThread == null) return false;
            else return Tool.currentThread.IsAlive;
        }











        /// <summary>
        /// Always called on main thread
        /// </summary>
        private void MenuDelete()
        {
            if (Tool.isWorking())
            {
                var t = Tool.currentThread;
                Tool.currentThread = null;
                t.Join();
            }

            this.ClearFutures(0, null);
            this.Dispose();

            FormMain.InvalidateWorkspace();

            Tool.ThreadMethodStart();
        }

        private bool DisposingCustom = false;
        private new void Dispose()
        {
            if (this.DisposingCustom) return;
            this.DisposingCustom = true;
            foreach (Control c in this._Controls)
            {
                NodeHandle nh = c as NodeHandle;
                if (nh == null) c.Dispose();
                else nh.Dispose();
            }
        }















        private Point LocationCustom
        {
            get
            {
                return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
            }
        }

        protected internal List<Control> _Controls = new List<Control>();

        internal new void BringToFront()
        {
            foreach (Control c in this._Controls) c.BringToFront();
        }

        internal void addTo(ControlCollection cc)
        {
            foreach (Control c in this._Controls) cc.Add(c);
        }

        internal void removeFrom(ControlCollection cc)
        {
            foreach (Control c in this._Controls) cc.Remove(c);
        }

        internal void move(int x, int y)
        {
            foreach (Control c in this._Controls)
            {
                c.Left += x;
                c.Top += y;
            }
        }

        internal void moveTo(Point p)
        {
            this.moveTo(p.X, p.Y);
        }

        internal void moveTo(int x, int y)
        {
            this.move(x - this.LocationCustom.X, y - this.LocationCustom.Y);
        }









        private Boolean dragging = false;
        private Point DragStartPoint = new Point();

        protected internal void dragStart(object sender, MouseEventArgs e)
        {
            this.DragStartPoint = new Point(e.X, e.Y);
            this.BringToFront();
            this.dragging = true;
        }

        protected internal void dragAction(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.move(e.X - this.DragStartPoint.X, e.Y - this.DragStartPoint.Y);
                FormMain.InvalidateWorkspace();
            }
        }

        protected internal void dragEnd(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;
            }
        }

        private void ToolStrip_Edit_Click(object sender, EventArgs e)
        {
            this.MenuEdit();
        }

        private void ToolStrip_Delete_Click(object sender, EventArgs e)
        {
            this.MenuDelete();
        }






        public PictureBox pictureBoxThumb = null;

        protected internal void UpdateThumb()
        {
            if (this.pictureBoxThumb != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.UpdateThumbThreadSafe();
                    });
                }
                else
                {
                    this.UpdateThumbThreadSafe();
                }
            }
        }

        private void UpdateThumbThreadSafe()
        {
            if (this.pictureBoxThumb.Image != null)
            {
                this.pictureBoxThumb.Image.Dispose();
                this.pictureBoxThumb.Image = null;
            }

            ImageData id = this.SpecialBitmapGet(null);

            if (id != null)
            {
                var sz = Sizing.fitAinB(id.Size, this.pictureBoxThumb.Size).Size;
                this.pictureBoxThumb.Image = id.getImage(sz.Width, sz.Height, !FormMain._BoolAutoZeroColor);
            }
        }

























        public virtual void MenuEdit()
        {
        }

        /// <summary>
        /// Always called by main thread.  Returns true if the sender Node Handle In has any
        /// affect on the output of the tool.  If it does,the input has been changed, so clear
        /// any data associated with this tool's current state.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public virtual Boolean ClearData(NodeHandleIn sender)
        {
            return false;
        }

        /// <summary>
        /// Updates the Special Bitmaps
        /// </summary>
        public virtual void SpecialBitmapUpdate()
        {
            return;
        }

        /// <summary>
        /// Gets the bitmap for a specific output
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public virtual ImageData SpecialBitmapGet(NodeHandleOut sender)
        {
            return null;
        }

        public virtual List<NodeHandleIn> getOutputs()
        {
            return new List<NodeHandleIn>();
        }
    }
}
