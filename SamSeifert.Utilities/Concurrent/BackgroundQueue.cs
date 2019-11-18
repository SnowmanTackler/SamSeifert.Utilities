using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Concurrent
{

    public abstract class BackgroundQueue : BackgroundToMainManager
    {
        private class IntHolder
        {
            public int Value = 0;
        }

        /*
         * Calling dispose on this item before it runs will cause it not to run.
         * It won't cancel it if it's already running.  Any queued tasks on main
         * thread will also be canceled.
         */
        private class BackgroundQueueItem : IDisposable, BackgroundTaskManager
        {
            private readonly BackgroundTask Method;
            private volatile bool IsDisposed = false;
            private BackgroundToMainManager Manager;
            
            /// <summary>
            /// Will pulse when the count changes from 1 -> 0
            /// </summary>
            private readonly LockWrapper<IntHolder> ActiveMainThreadTasks = new LockWrapper<IntHolder>(new IntHolder());

            public void Dispose()
            {
                this.IsDisposed = true;
            }

            public BackgroundQueueItem(BackgroundTask meth)
            {
                this.Method = meth;
            }

            public void Run(BackgroundToMainManager manager)
            {
                if (this.IsDisposed) return;
                this.Manager = manager;
                this.Method(this);
            }

            public bool RunOnMainThread(Action a)
            {
                if (this.IsDisposed)
                {
                    return false;
                }

                this.ActiveMainThreadTasks.Access((data, locked) =>
                {
                    data.Value++;
                });

                Action decrementCount = () =>
                {
                    ActiveMainThreadTasks.Access((data, locked) =>
                    {
                        data.Value--;
                        if (data.Value == 0)
                        {
                            locked.Pulse();
                        }
                    });
                };

                var taskAddedToMainThread = this.Manager.RunOnMainThread(() =>
                {
                    if (!this.IsDisposed)
                    {
                        a();
                    }
                    decrementCount();
                });

                if (taskAddedToMainThread)
                {
                    return true;
                }
                else 
                {
                    decrementCount();
                    return false;
                }
            }

            public bool ShouldContinue()
            {
                return !this.IsDisposed;
            }

            public void WaitForMainThreadTasks()
            {
                this.ActiveMainThreadTasks.Access((data, locked) =>
                {
                    while (data.Value != 0)
                    {
                        locked.Wait();
                    }
                });
            }
        }

        private class ThreadData
        {
            public BackgroundQueueItem ActiveTask = null;
            /// <summary>
            /// When the thread is ended (told to stop, and has finished all it's work), this 
            /// will get set to false.
            /// </summary>
            public bool Alive = true;
        }

        private class SharedData {
            public ThreadData[] ThreadData;
            public bool IsDisposed = false;
            public readonly Queue<BackgroundQueueItem> Queue = new Queue<BackgroundQueueItem>();
        }

        /// <summary>
        /// Will get pulsed whenever a thread terminates.
        /// </summary>
        private LockWrapper<SharedData> LockedSharedData = new LockWrapper<SharedData>(new SharedData());

        public BackgroundQueue(
            String name,
            ThreadPriority tp = ThreadPriority.Normal,
            int threads = 1)
        {
            this.LockedSharedData.Access((data, locked) =>
            {
                data.ThreadData = new ThreadData[threads];
                data.ThreadData.Fill(() => new ThreadData());
            });


            for (int i = 0; i < threads; i++)
            {
                new Thread(this.BackgroundThreadMethod).Start(
                    new object[]
                    {
                        i,
                        (name ?? "Queue")  + " " + i,
                        tp
                    });
            }
        }

        /// <summary>
        /// Will cancel current tasks, and will stop accepting future tasks.  Will
        /// terminate threads.
        /// </summary>
        public void Stop(bool clearActiveTasks = true, bool clearQueuedTasks = true)
        {
            this.ClearAndMaybeStop(clearActiveTasks, clearQueuedTasks, true);
        }

        /// <summary>
        /// Will cancel current tasks, but will still accept future tasks.
        /// </summary>
        public void Clear(bool activeTasks = true, bool queuedTasks = true)
        {
            this.ClearAndMaybeStop(activeTasks, queuedTasks, false);
        }

        private void ClearAndMaybeStop(bool clearActiveTasks, bool clearQueuedTasks, bool stop = true)
        {
            this.LockedSharedData.Access((data, locked) =>
            {
                if (clearActiveTasks)
                {
                    data.ThreadData.ForEach(td => { 
                        td.ActiveTask?.Dispose();
                    });
                }
                if (clearQueuedTasks)
                {
                    data.Queue.ForEach(a => a.Dispose());
                    data.Queue.Clear();
                }
                if (stop)
                {
                    data.IsDisposed = true;
                }
            });
        }

        public IDisposable Enqueue(BackgroundTask meth)
        {
            return this.LockedSharedData.Access((data, locked) =>
            {
                if (!data.IsDisposed)
                {
                    var item = new BackgroundQueueItem(meth);
                    data.Queue.Enqueue(item);
                    locked.Pulse();
                    return item;
                } 
                else
                {
                    return null;
                }
            });
        }

        private void BackgroundThreadMethod(object oargs)
        {
            var args = oargs as object[];

            int threadIndex = (int)args[0];
            Thread.CurrentThread.Name = args[1] as String;
            Thread.CurrentThread.Priority = (ThreadPriority)args[2];

            while (true)
            {
                BackgroundQueueItem item = null;

                bool IsDisposed = this.LockedSharedData.Access((data, locked) =>
                {
                    if (data.IsDisposed)
                    {
                        data.ThreadData[threadIndex].ActiveTask = null;
                        return true;
                    }
                    if (data.Queue.Count == 0)
                    {
                        data.ThreadData[threadIndex].ActiveTask = null;
                        // Wait will release the lock until it is racquired!
                        locked.Wait(500);
                        return false;
                    }
                    else
                    {
                        item = data.Queue.Dequeue();
                        data.ThreadData[threadIndex].ActiveTask = item;
                        return false;
                    }
                });

                if (IsDisposed)
                {
                    break;
                } 
                else
                {
                    item?.Run(this);
                }
            }

            this.LockedSharedData.Access((data, locked) =>
            {
                data.ThreadData[threadIndex].Alive = false;
            });
        }

        /// <summary>
        /// If this returns false, the queue is still running, and will accept new items.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return this.LockedSharedData.Access((data, locked) => data.IsDisposed);
            }
        }

        /// <summary>
        /// Returns true if any thread is free, and will continue to be free for the forseeable future.
        /// (Until another item get's enqueued).
        /// </summary>
        public bool HasWaitingThreads
        {
            get
            {
                return this.LockedSharedData.Access((data, locked) =>
                {
                    int waitingThreads = 0;
                    foreach (var td in data.ThreadData)
                    {
                        if (td.Alive && (td.ActiveTask == null))
                        {
                            waitingThreads++;
                        }
                    }
                    return waitingThreads > data.Queue.Count;
                });
            }
        }

        /// <summary>
        /// Returns true if any thread is working / will be working in the forseeable future.
        /// </summary>
        public bool HasWorkingThreads
        {
            get
            {
                return this.ActiveTasks != 0;
            }
        }

        /// <summary>
        /// Total number of tasks in the queue and tasks being executed right now!
        /// </summary>
        public int ActiveTasks
        {
            get
            {
                return this.LockedSharedData.Access((data, locked) =>
                {
                    int tasks = data.Queue.Count;
                    foreach (var td in data.ThreadData)
                    {
                        if (td.ActiveTask != null)
                        {
                            tasks++;
                        }
                    }
                    return tasks;
                });
            }
        }

        /*
        TODO(SAM) use different LOCK PULSE
        public void Join()
        {
            this.LockedSharedData.Access((data, locked) =>
            {
                while (true)
                {
                    int countAlive = 0;
                    foreach (var td in data.ThreadData)
                    {
                        if (td.Alive)
                        {
                            countAlive++;
                        }
                    }

                    if (countAlive == 0) break;

                    locked.Wait();
                }
            });
        }
        */

        public abstract bool RunOnMainThread(Action a);

    }
}
