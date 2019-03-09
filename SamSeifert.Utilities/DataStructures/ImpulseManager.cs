using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace SamSeifert.Utilities.DataStructures
{
    public class ImpulseManager<T> : IEnumerable<T>
    {
        private class Impulse
        {
            public readonly float _Time;
            public T _T;
            public Impulse(T t, float current_time_seconds)
            {
                this._T = t;
                this._Time = current_time_seconds;
            }
        }

        public float _ExpireTime { get; set; }

        private int _Index = 0;
        public int _Length { get; private set; }
        private Impulse[] _Data = new Impulse[8];

        public ImpulseManager(float expire_time = 0.0f)
        {
            this._Length = 0;
            this._ExpireTime = expire_time;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < this._Length; i++)
                yield return this._Data[(this._Index + i) % this._Data.Length]._T;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // TODO        
            throw new NotImplementedException();
        }

        public void Enqueue(T t, float current_time_seconds)
        {
            if (this._Length == _Data.Length)
            {
                var rep = new Impulse[this._Length * 2];
                int l1 = this._Length - _Index;
                Array.Copy(_Data, _Index, rep, 0, this._Length - _Index);
                Array.Copy(_Data, 0, rep, this._Length - _Index, _Index);
                _Data = rep;
                _Index = 0;
            }
            _Data[(_Index + this._Length) % _Data.Length] = new Impulse(t, current_time_seconds);
            this._Length++;
        }

        public T this[int dex]
        {
            get
            {
                if (dex < this._Length) return this._Data[(this._Index + dex) % this._Data.Length]._T;
                else throw new IndexOutOfRangeException("In ImpulseManager");
            }
        }

        private Impulse At(int dex)
        {
            if (dex < this._Length) return this._Data[(this._Index + dex) % this._Data.Length];
            else throw new IndexOutOfRangeException("In ImpulseManager");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current_time_seconds"></param>
        /// <param name="leave_this_many">This is the minimum number that will be left (if it starts with enough)</param>
        public void ClearOld(float current_time_seconds, int leave_this_many = 0)
        {
            this.ClearBefore(current_time_seconds - this._ExpireTime, leave_this_many);
        }

        public void ClearBefore(float expire_time_seconds, int leave_this_many = 0)
        {
            while (this._Length > leave_this_many)
            {
                if (this._Data[this._Index % this._Data.Length]._Time <= expire_time_seconds)
                {
                    this._Index = (this._Index + 1) % _Data.Length;
                    this._Length--;
                }
                else break;
            }
        }

        public float TimeForIndex(int dex)
        {
            if (dex < this._Length) return this._Data[(this._Index + dex) % this._Data.Length]._Time;
            else throw new IndexOutOfRangeException("In ImpulseManager");
        }

        public T Oldest
        {
            get
            {
                return this[0];
            }
        }

        public Tuple<float, T> OldestImpulse
        {
            get
            {
                var impulse = this.At(0);
                return new Tuple<float, T>(impulse._Time, impulse._T);
            }
        }

        public T Newest
        {
            get
            {
                return this[this._Length - 1];
            }
        }

        public Tuple<float, T> NewestImpulse
        {
            get
            {
                var impulse = this.At(this._Length - 1);
                return new Tuple<float, T>(impulse._Time, impulse._T);
            }
        }

        public void Clear()
        {
            this._Length = 0;
        }

        public IEnumerable<Tuple<float, T>> EnumerateTimes()
        {
            for (int i = 0; i < this._Length; i++)
            {
                var impulse = this._Data[(this._Index + i) % this._Data.Length];
                yield return new Tuple<float, T>(impulse._Time, impulse._T);
            }
        }
        /*
        public bool En(float time, out T t)
        {
            // Should implement quick search;
            for (int i = 0; i < this._Length; i++)
            {
                var impulse = this._Data[(this._Index + i) % this._Data.Length];

                if (impulse._Time == time)
                {
                    t = impulse._T;
                    return true;
                }
            }

            t = default(T);
            return false;
        }*/
    }
}
