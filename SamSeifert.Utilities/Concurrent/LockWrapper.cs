using System;
using System.Threading;

namespace SamSeifert.Utilities.Concurrent {
    /// <summary>
    /// Wraps a lock and gives it helpful methods like Pulse, PulseAll, and Wait.
    /// </summary>
    public class MonitorWrapper {
        private Object Lock;
        public MonitorWrapper(object Lock) {
            this.Lock = Lock;
        }
        public void Pulse() {
            Monitor.Pulse(this.Lock);
        }

        public void PulseAll() {
            Monitor.PulseAll(this.Lock);
        }

        public void Wait(int millisecondsTimeout = int.MaxValue) {
            Monitor.Wait(this.Lock, millisecondsTimeout);
        }
    }

    /// <summary>
    /// Wraps a variable and only gives you access to it through the thread safe
    /// "Access" methods.
    /// </summary>
    /// <typeparam name="Mutable"></typeparam>
    public class LockWrapper<Mutable> {
        private readonly Mutable Instance;
        private readonly MonitorWrapper MonitorLock;
        private readonly object Lock = new Object();

        public LockWrapper(Mutable mutable) {
            this.Instance = mutable;
            this.MonitorLock = new MonitorWrapper(Lock);
        }

        public void Access(Action<Mutable, MonitorWrapper> action) {
            lock (this.Lock) {
                action(this.Instance, this.MonitorLock);
            }
        }

        public T Access<T>(Func<Mutable, MonitorWrapper, T> action) {
            lock (this.Lock) {
                T ret = action(this.Instance, this.MonitorLock);
                return ret;
            }
        }
    }

    public class LockWrapperStruct<T> where T : struct {
        private T Instance;
        private readonly object Lock = new Object();

        public LockWrapperStruct(T m) {
            this.Instance = m;
        }

        public void Set(T m) {
            lock (this.Lock) {
                this.Instance = m;
            }
        }

        public T Get() {
            lock (this.Lock) {
                return this.Instance;
            }
        }
    }
}
