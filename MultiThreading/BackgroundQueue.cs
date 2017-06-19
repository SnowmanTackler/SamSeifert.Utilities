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
        private readonly Thread _Thread;
        private readonly Queue<BackgroundQueueMethod> _Queue = new Queue<BackgroundQueueMethod>();
        private bool _Queue_CurrentlyWorking = false;

        private volatile bool _ShouldContinue = true;
        private volatile bool _Working = true;

        public BackgroundQueue(Form f, String name, ThreadPriority tp = ThreadPriority.Normal)
        {
            if (f == null)
                throw new ArgumentNullException();

            f.FormClosing += this.FormClosing;

            this._Thread = new Thread(this._BackgroundThread);
            this._Thread.Start(
                new object[]
                {
                    f,
                    name,
                    tp
                });
        }

        private void Enqueue(BackgroundQueueMethod meth)
        {
            lock (this._Queue)
            {
                this._Queue.Enqueue(meth);
                Monitor.Pulse(this._Queue);
            }
        }

        public void Enqueue(ParameterizedBackgroundThreadMethod meth, object param)
        {
            this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                meth(continue_on, param);
            });
        }

        public void Enqueue(BackgroundThreadMethod meth)
        {
            this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                meth(continue_on);
            });
        }

        public void Enqueue(ParameterizedBackgroundThreadMethodOut meth, object param, BackgroundThreadFinisher finish)
        {
            this.Enqueue((ContinueCheck continue_on) => { return meth(continue_on, param); }, finish);
        }

        public void Enqueue(BackgroundThreadMethodOut meth, BackgroundThreadFinisher finish)
        {
            this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                var ob = meth(continue_on);
                f.Invoke(finish, ob);
            });
        }



        private void _BackgroundThread(object oargs)
        {
            var args = oargs as object[];

            var form = args[0] as Form;
            Thread.CurrentThread.Name = (args[1] == null) ? "Background Thread" : args[1] as String;
            Thread.CurrentThread.Priority = (ThreadPriority)args[2];

            while (this._ShouldContinueMethod())
            {
                BackgroundQueueMethod btm = null;

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
                        btm = this._Queue.Dequeue();
                    }
                }

                if (btm != null)
                {
                    btm(form, this._ShouldContinueMethod);

                    lock (this._Queue)
                        this._Queue_CurrentlyWorking = false;
                }
            }

            form.Invoke(
                (Action)(() => {
                    form.FormClosing -= this.FormClosing;
                }));

            this._Working = false;
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
                return this._Working;
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

        public void _ThreadSafe_Join()
        {
            this._Thread?.Join();
        }
    }
}
