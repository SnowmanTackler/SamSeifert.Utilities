using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Concurrent
{
    public class ReadWriteWrapper<Immutable, Mutable> where Mutable : Immutable
    {
        private readonly Mutable Instance;
        private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        public ReadWriteWrapper(Mutable mutable)
        {
            this.Instance = mutable;
        }

        public void Read(Action<Immutable> action)
        {
            rwl.EnterReadLock();
            action(this.Instance);
            rwl.ExitReadLock();
        }

        public T Read<T>(Func<Immutable, T> action)
        {
            rwl.EnterReadLock();
            T ret = action(this.Instance);
            rwl.ExitReadLock();
            return ret;
        }

        public void Write(Action<Mutable> action)
        {
            rwl.EnterWriteLock();
            action(this.Instance);
            rwl.ExitWriteLock();
        }

        public T Write<T>(Func<Mutable, T> action)
        {
            rwl.EnterWriteLock();
            T ret = action(this.Instance);
            rwl.ExitWriteLock();
            return ret;
        }
    }
}
