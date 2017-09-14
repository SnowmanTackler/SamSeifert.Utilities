using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.DataStructures
{
    public class Heap<T>
    {
        private int _Capacity = 10;
        public int _Size { get; private set; } = 0;

        T[] _Items;
        private Func<T, T, bool> _Func;

        private int getLeftChildIndex(int parentIndex) { return 2 * parentIndex + 1; }
        private int getRightChildIndex(int parentIndex) { return 2 * parentIndex + 2; }
        private int getParentIndex(int childIndex) { return (childIndex - 1) / 2; }

        private bool hasLeftChild(int index) { return getLeftChildIndex(index) < _Size; }
        private bool hasRightChild(int index) { return getRightChildIndex(index) < this._Size; }
        private bool hasParent(int index) { return getParentIndex(index) >= 0; }

        private T leftChild(int index) { return this._Items[getLeftChildIndex(index)]; }
        private T rightChild(int index) { return this._Items[getRightChildIndex(index)]; }
        private T parent(int index) { return this._Items[this.getParentIndex(index)]; }

        public Heap(Func<T, T, bool> true_if_left_entry_should_be_above_right_entry_in_heap)
        {
            this._Func = true_if_left_entry_should_be_above_right_entry_in_heap;
            this._Items = new T[_Capacity];
        }

        private void SwapIndices(int indexOne, int indexTwo)
        {
            MiscUtil.Swap(ref this._Items[indexOne], ref this._Items[indexTwo]);
        }

        private void EnsureExtraCapacity()
        {
            if (this._Size == _Capacity)
            {
                Array.Resize(ref this._Items, _Capacity * 2);
                _Capacity *= 2;
            }
        }

        public T Peek()
        {
            if (this._Size == 0) throw new NotSupportedException();
            return this._Items[0];
        }

        public T Remove()
        {
            if (this._Size == 0) throw new NotSupportedException();
            T item = this._Items[0];
            _Items[0] = _Items[this._Size - 1];
            _Items[this._Size - 1] = default(T);
            this._Size--;
            heapifyDown();
            return item;
        }

        public void Add(T item)
        {
            this.EnsureExtraCapacity();
            this._Items[_Size] = item;
            this._Size++;
            heapifyUp();
        }

        private void heapifyUp()
        {
            int index = this._Size - 1;
            while (hasParent(index) && this._Func(this._Items[index], parent(index)))
            {
                SwapIndices(index, getParentIndex(index));
                index = getParentIndex(index);
            }
        }

        private void heapifyDown()
        {
            int index = 0;
            while (hasLeftChild(index))
            {
                int smallerChildIndex = (hasRightChild(index) && this._Func(rightChild(index), leftChild(index))) ?
                    getRightChildIndex(index) :
                    getLeftChildIndex(index);

                if (this._Func(this._Items[index], this._Items[smallerChildIndex]))
                {
                    break;
                }
                else
                {
                    SwapIndices(index, smallerChildIndex);
                }
                index = smallerChildIndex;
            }
        }
    }
}
