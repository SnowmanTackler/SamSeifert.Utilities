using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace SamSeifert.Utilities.DataStructures
{
    public class Queue<T> : IEnumerable<T> where T : struct
    {
        private int _Index = 0;
        public int _Length { get; private set; }
        private T[] _Data = new T[8];

        public Queue()
        {
            this._Length = 0;
        }

        public void Enqueue(T t)
        {
            if (this._Length == _Data.Length)
            {
                var rep = new T[this._Length * 2];
                int l1 = this._Length - _Index;
                Array.Copy(_Data, _Index, rep, 0, this._Length - _Index);
                Array.Copy(_Data, 0, rep, this._Length - _Index, _Index);
                _Data = rep;
                _Index = 0;
            }
            _Data[(_Index + this._Length) % _Data.Length] = t;
            this._Length++;
        }

        public T this[int dex]
        {
            get
            {
                if (dex < this._Length) return this._Data[(this._Index + dex) % this._Data.Length];
                else throw new IndexOutOfRangeException("In Queue");
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < this._Length; i++)
                yield return this._Data[(this._Index + i) % this._Data.Length];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // TODO        
            throw new NotImplementedException();
        }

        public T Newest
        {
            get
            {
                return this[this._Length - 1];
            }
        }

        internal T Oldest
        {
            get
            {
                return this[0];
            }
        }

        internal void Clear()
        {
            this._Length = 0;
        }
    }



}
