using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.MultiThreading
{
    public class BackgroundQueue
    {
        private readonly Queue<BackgroundThreadMethod> _Queue = new Queue<BackgroundThreadMethod>();
        private bool _Queue_CurrentlyWorking = false;

        private readonly String _Name;
        private Form _ClosingForm;

        private volatile bool _ShouldContinue = true;
        private volatile Thread _Thread = null;


        public BackgroundQueue(Form f, String name)
        {
            this._Name = name;

            this._ClosingForm = f;

            if (this._ClosingForm != null)
                this._ClosingForm.FormClosing += FormClosing;

            this._Thread = new Thread(this._BackgroundThread);
            this._Thread.Start();
        }

        public void Enqueue(BackgroundThreadMethodWithParam meth, object param)
        {
            this.Enqueue((ContinueCheck continue_on) =>
            {
                meth(continue_on, param);
            });
        }

        public void Enqueue(BackgroundThreadMethod meth)
        {
            lock (this._Queue)
            {
                this._Queue.Enqueue(meth);
                Monitor.Pulse(this._Queue);
            }
        }

        private void _BackgroundThread()
        {
            Thread.CurrentThread.Name = (this._Name == null) ? "Background Queue" : this._Name;

            while (this._ShouldContinueMethod())
            {
                BackgroundThreadMethod args = null;

                lock (this._Queue)
                {
                    if (this._Queue.Count == 0)
                    {
                        // Wait will release the lock until it is racquired!
                       Monitor.Wait(this._Queue, 250);
                    }
                    else
                    {
                        this._Queue_CurrentlyWorking = true;
                        args = this._Queue.Dequeue();
                    }
                }

                if (args != null)
                {
                    args(this._ShouldContinueMethod);

                    lock (this._Queue)
                        this._Queue_CurrentlyWorking = false;
                }
            }

            this._Thread = null;

            if (this._ClosingForm != null)
            {
                Action a = () =>
                {
                    this._ClosingForm.FormClosing -= this.FormClosing;
                    this._ClosingForm = null;
                };

                if (this._ClosingForm.IsDisposed) return;
                else if (this._ClosingForm.InvokeRequired) this._ClosingForm.BeginInvoke(a);
                else a();
            }
        }

        private bool _ShouldContinueMethod()
        {
            return this._ShouldContinue;
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            this._ShouldContinue = false;

            if (this._ThreadSafe_Running)
            {
                e.Cancel = true;
            }
        }

        public void _ThreadSafe_Stop()
        {
            this._ShouldContinue = false;
        }

        /// <summary>
        /// Returns true if the background thread is still looking for new tasks
        /// </summary>
        public bool _ThreadSafe_Running
        {
            get
            {
                return this._Thread != null;
            }
        }

        public bool _ThreadSafe_Waiting
        {
            get
            {
                lock (this._Queue)
                {
                    if (this._Queue_CurrentlyWorking) return false;
                    else return this._Queue.Count == 0;
                }
            }
        }
    }
}
