using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SamSeifert.Utilities.Concurrent {

    public abstract class BackgroundQueue : BackgroundToMainManager {
        private class IntHolder {
            public int Value = 0;
        }

        /*
         * Calling dispose on this item before it runs will cause it not to run.
         * It won't cancel it if it's already running.  Any queued tasks on main
         * thread will also be canceled.
         */
        private class BackgroundQueueItem : IDisposable, BackgroundTaskManager {
            private readonly BackgroundTask Method;
            public bool IsDisposed = false;
            private BackgroundToMainManager Manager;
            private readonly CancellationTokenSource TokenSource = new CancellationTokenSource();
            private readonly object Lock = new object();

            /// <summary>
            /// Will pulse when the count changes from 1 -> 0
            /// </summary>
            private readonly LockWrapper<IntHolder> ActiveMainThreadTasks = new LockWrapper<IntHolder>(new IntHolder());
            public BackgroundQueueItem(BackgroundTask meth) {
                this.Method = meth;
            }
            public void Dispose() {
                lock (this.Lock) {
                    if (this.IsDisposed) {
                        return;
                    }
                    this.IsDisposed = true;
                    this.TokenSource.Cancel();
                    this.TokenSource.Dispose();
                }
            }
            public void Run(BackgroundToMainManager manager) {
                lock (this.Lock) {
                    if (this.IsDisposed) {
                        return;
                    }
                    this.Manager = manager;
                }

                this.Method(this);
            }

            public bool RunOnMainThread(Action a) {
                lock (this.Lock) {
                    if (this.IsDisposed) {
                        return false;
                    }
                }

                this.ActiveMainThreadTasks.Access((data, locked) => {
                    data.Value++;
                });

                Action decrementCount = () => {
                    ActiveMainThreadTasks.Access((data, locked) => {
                        data.Value--;
                        if (data.Value == 0) {
                            locked.Pulse();
                        }
                    });
                };

                var taskAddedToMainThread = this.Manager.RunOnMainThread(() => {
                    if (!this.IsDisposed) {
                        a();
                    }
                    decrementCount();
                });

                if (taskAddedToMainThread) {
                    return true;
                } else {
                    decrementCount();
                    return false;
                }
            }

            public bool ShouldContinue() {
                lock (this.Lock) {
                    return !this.IsDisposed;
                }
            }

            public void WaitForMainThreadTasks() {
                this.ActiveMainThreadTasks.Access((data, locked) => {
                    while (data.Value != 0) {
                        locked.Wait();
                    }
                });
            }

            public CancellationToken GetCancellationToken() {
                lock (this.Lock) {
                    if (this.IsDisposed) {
                        return new CancellationToken(true);
                    } else {
                        return this.TokenSource.Token;
                    }
                }
            }
        }

        private class PostProcessingTracker {
            public readonly bool[] Entered;
            public readonly BackgroundQueueItem[] Items;
            public PostProcessingTracker(int threadCount) {
                this.Entered = new bool[threadCount];
                this.Items = new BackgroundQueueItem[threadCount];
            }
        }

        private class ThreadData {
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
        private readonly int ThreadCount;

        public BackgroundQueue(
            String name,
            ThreadPriority tp = ThreadPriority.Normal,
            int threads = 1) {
            this.ThreadCount = threads;

            this.LockedSharedData.Access((data, locked) => {
                data.ThreadData = new ThreadData[threads];
                data.ThreadData.Fill(() => new ThreadData());
            });


            for (int i = 0; i < threads; i++) {
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
        public void Stop(bool clearActiveTasks = true, bool clearQueuedTasks = true) {
            this.ClearAndMaybeStop(clearActiveTasks, clearQueuedTasks, true);
        }

        /// <summary>
        /// Will cancel current tasks, but will still accept future tasks.
        /// </summary>
        public void Clear(bool activeTasks = true, bool queuedTasks = true) {
            this.ClearAndMaybeStop(activeTasks, queuedTasks, false);
        }

        private void ClearAndMaybeStop(bool clearActiveTasks, bool clearQueuedTasks, bool stop = true) {
            this.LockedSharedData.Access((data, locked) => {
                if (clearActiveTasks) {
                    data.ThreadData.ForEach(td => {
                        td.ActiveTask?.Dispose();
                    });
                }
                if (clearQueuedTasks) {
                    data.Queue.ForEach(a => a.Dispose());
                    data.Queue.Clear();
                }
                if (stop) {
                    data.IsDisposed = true;
                }
            });
        }

        public IDisposable Enqueue(BackgroundTask meth) {
            return this.LockedSharedData.Access((data, locked) => {
                if (!data.IsDisposed) {
                    var item = new BackgroundQueueItem(meth);
                    data.Queue.Enqueue(item);
                    locked.Pulse();
                    return item;
                } else {
                    return null;
                }
            });
        }
        public IEnumerable<IDisposable> EnqueueAll(IEnumerable<BackgroundTask> meths) {
            return this.LockedSharedData.Access((data, locked) => {
                if (!data.IsDisposed) {
                    var items = meths.Select(it => new BackgroundQueueItem(it));
                    items.ForEach(item => { data.Queue.Enqueue(item); });
                    locked.PulseAll();
                    return items;
                } else {
                    return null;
                }
            });
        }

        /// <summary>
        /// Use this method to run something once all other currently enqueued tasks have finished.
        /// You can just use enqueue for this case, as when there is more than 3 worker threads 
        /// tasks might get completed out of order.
        /// </summary>
        /// <param name="task"></param>
        public IDisposable EnqueuePostProcessingTask(BackgroundTask task) {
            // Create a data structure that will be track progress of n tasks, where
            // n is the number of worker threads we have.
            var lockedObject = new LockWrapper<PostProcessingTracker>(new PostProcessingTracker(this.ThreadCount));

            // Function for creating a task that will block until all it's sisters have entered.
            Func<int, BackgroundQueueItem> blockTask = index => {
                return new BackgroundQueueItem(manager => {
                    // Make sure all old tasks are finished (and dead).
                    manager.WaitForMainThreadTasks();

                    // Tell other threads we made it here.
                    lockedObject.Access((data, locked) => { data.Entered[index] = true; });

                    // Block indefinitely.
                    while (true) {
                        // Check if all other threads either made it here, or died trying.
                        int enteredOrDisposed = lockedObject.Access((data, locked) => {
                            var count = 0;
                            for (int tc = 0; tc < this.ThreadCount; tc++) {
                                if (data.Entered[tc] || data.Items[tc].IsDisposed) {
                                    count++;
                                }
                            }
                            return count++;
                        });

                        if (enteredOrDisposed == this.ThreadCount) {
                            // If everyone is accounted for, stop the blocking.
                            break;
                        } else {
                            // This is a bit sloppy.  It would be pretty easy
                            // to tell when a task is entered using lock / pulse,
                            // but hard to tell when a task is disposed.
                            Thread.Sleep(100);
                        }
                    }
                });
            };

            // Create a blocking task for each thread, and store it in our data structure.
            var items = lockedObject.Access((data, locked) => {
                for (int i = 0; i < this.ThreadCount; i++) {
                    data.Entered[i] = false;
                    data.Items[i] = blockTask(i);
                }
                var its = new List<BackgroundQueueItem>();
                its.AddRange(data.Items);
                return its;
            });

            // The above tasks will each block a worker thread until all of them
            // have either been entered, or disposed.  After those tasks complete,
            // we complete the assigned task IF none of the prior tasks were disposed.
            items.Add(new BackgroundQueueItem(manager => {
                int entered = lockedObject.Access((data, locked) => {
                    var count = 0;
                    for (int tc = 0; tc < this.ThreadCount; tc++) {
                        if (data.Entered[tc]) {
                            count++;
                        }
                    }
                    return count++;
                });

                if (entered == this.ThreadCount) {
                    task(manager);
                }
            }));

            // Add all the background task items to the queue.
            return this.LockedSharedData.Access((data, locked) => {
                if (!data.IsDisposed) {
                    items.ForEach(item => { data.Queue.Enqueue(item); });
                    locked.PulseAll();
                    return items.Last();
                } else {
                    return null;
                }
            });
        }

        private void BackgroundThreadMethod(object oargs) {
            var args = oargs as object[];

            int threadIndex = (int) args[0];
            Thread.CurrentThread.Name = args[1] as String;
            Thread.CurrentThread.Priority = (ThreadPriority) args[2];

            while (true) {
                BackgroundQueueItem item = null;

                bool IsDisposed = this.LockedSharedData.Access((data, locked) => {
                    if (data.IsDisposed) {
                        data.ThreadData[threadIndex].ActiveTask = null;
                        return true;
                    }
                    if (data.Queue.Count == 0) {
                        data.ThreadData[threadIndex].ActiveTask = null;
                        // Wait will release the lock until it is racquired!
                        locked.Wait(500);
                        return false;
                    } else {
                        item = data.Queue.Dequeue();
                        data.ThreadData[threadIndex].ActiveTask = item;
                        return false;
                    }
                });

                if (IsDisposed) {
                    break;
                } else {
                    item?.Run(this);
                }
            }

            this.LockedSharedData.Access((data, locked) => {
                data.ThreadData[threadIndex].Alive = false;
            });
        }

        /// <summary>
        /// If this returns false, the queue is still running, and will accept new items.
        /// </summary>
        public bool IsDisposed {
            get {
                return this.LockedSharedData.Access((data, locked) => data.IsDisposed);
            }
        }

        /// <summary>
        /// Returns true if any thread is free, and will continue to be free for the forseeable future.
        /// (Until another item get's enqueued).
        /// </summary>
        public bool HasWaitingThreads {
            get {
                return this.LockedSharedData.Access((data, locked) => {
                    int waitingThreads = 0;
                    foreach (var td in data.ThreadData) {
                        if (td.Alive && (td.ActiveTask == null)) {
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
        public bool HasWorkingThreads {
            get {
                return this.ActiveTasks != 0;
            }
        }

        /// <summary>
        /// Total number of tasks in the queue and tasks being executed right now!
        /// </summary>
        public int ActiveTasks {
            get {
                return this.LockedSharedData.Access((data, locked) => {
                    int tasks = data.Queue.Count;
                    foreach (var td in data.ThreadData) {
                        if (td.ActiveTask != null) {
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
