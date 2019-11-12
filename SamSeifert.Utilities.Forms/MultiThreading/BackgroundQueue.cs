using SamSeifert.Utilities.Extensions;
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
        /*
         * Calling dispose on this item before it runs will cause it not to run.
         * It won't cancel it if it's already running
         */
        private class BackgroundQueueItem : IDisposable
        {
            public volatile bool _Cancel = false;
            public readonly BackgroundQueueMethod _Method;

            public void Dispose()
            {
                this._Cancel = true;
            }

            public BackgroundQueueItem(BackgroundQueueMethod meth)
            {
                this._Method = meth;
            }
        }

        private readonly Thread[] _Threads;
        private readonly Queue<BackgroundQueueItem> _Queue = new Queue<BackgroundQueueItem>();
        private bool _Queue_CurrentlyWorking = false;

        private volatile bool _ShouldContinue = true;

        private volatile int _Working;

        public BackgroundQueue(
            Form f, 
            String name, 
            ThreadPriority tp = ThreadPriority.Normal,
            int threads = 1)
        {
            if (f == null)
                throw new ArgumentNullException();

            f.FormClosing += this.FormClosing;

            this._Working = threads;
            this._Threads = new Thread[threads];
            this._Threads.Fill(i => new Thread(this._BackgroundThread));

            for (int i = 0; i < threads; i++)
            {
                this._Threads[i].Start(
                    new object[]
                    {
                        f,
                        (name ?? "Queue")  + " " + i,
                        tp
                    });

            }
        }

        public void Clear()
        {
            lock (this._Queue)
            {
                this._Queue.Clear();
            }
        }

        private IDisposable Enqueue(BackgroundQueueMethod meth)
        {
            var item = new BackgroundQueueItem(meth);
            lock (this._Queue)
            {
                this._Queue.Enqueue(item);
                Monitor.Pulse(this._Queue);
            }
            return item;
        }

        public IDisposable Enqueue(ParameterizedBackgroundThreadMethod meth, object param)
        {
            return this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                meth(continue_on, param);
            });
        }

        public IDisposable Enqueue(BackgroundThreadMethod meth)
        {
            return this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                meth(continue_on);
            });
        }

        public IDisposable Enqueue(ParameterizedBackgroundThreadMethodOut meth, object param, BackgroundThreadFinisher finish)
        {
            return this.Enqueue((ContinueCheck continue_on) => { return meth(continue_on, param); }, finish);
        }

        public IDisposable Enqueue(BackgroundThreadMethodOut meth, BackgroundThreadFinisher finish, bool beginInvoke = false)
        {
            return this.Enqueue((Form f, ContinueCheck continue_on) =>
            {
                var ob = meth(continue_on);
                if (beginInvoke)
                {
                    f.BeginInvoke(finish, ob);
                }
                else
                {
                    f.Invoke(finish, ob);
                }
            });
        }



        private void _BackgroundThread(object oargs)
        {
            var args = oargs as object[];

            var form = args[0] as Form;
            Thread.CurrentThread.Name = args[1] as String;
            Thread.CurrentThread.Priority = (ThreadPriority)args[2];

            while (this._ShouldContinueMethod())
            {
                BackgroundQueueItem btm = null;

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
                    if (!btm._Cancel)
                    {
                        btm._Method(form, this._ShouldContinueMethod);

                        lock (this._Queue)
                            this._Queue_CurrentlyWorking = false;
                    }
                }
            }

            bool lastThread = false;
            lock (this._Queue)
            {
                lastThread = this._Working == 1;
                this._Working--;
            }

            if (lastThread)
            {
                form.BeginInvoke(
                    (Action)(() => {
                        form.FormClosing -= this.FormClosing;
                    }));
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
                lock (this._Queue)
                {
                    return this._Working > 0;
                }
            }
        }

        /// <summary>
        /// Returns false if the queue is either currently working or has more items to do
        /// </summary>
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

        public int _Count
        {
            get
            {
                lock (this._Queue)
                {
                    return this._Queue.Count;
                }
            }
        }

        /// <summary>
        /// BEc
        /// </summary>
        public void _ThreadSafe_Join()
        {
            foreach (var thread in this._Threads)
            {
                thread.Join();
            }
        }
    }
}
