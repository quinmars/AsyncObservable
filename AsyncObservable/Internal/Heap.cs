using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    internal class Heap<T>
    {
        private struct IndexedItem
        {
            public int Index;
            public T Value;
        }

        readonly List<IndexedItem> _list;
        readonly IComparer<T> _comparer;
        int _count = 0;
        int _nextIndex = 1;

        public Heap(IComparer<T> comparer)
        {
            this._list = new List<IndexedItem>();
            this._comparer = comparer;
        }

        public void Append(T value)
        {
            var item = new IndexedItem { Index = _nextIndex, Value = value };
            if (_count == _list.Count)
                _list.Add(item);
            else
                _list[_count] = item;

            _nextIndex++;
            _count++;
        }

        public void Build()
        { 
            for (int i = _count / 2 - 1; i >= 0; --i)
                Heapify(i);
        }

        public int Count => _count;

        public T Pop()
        {
            var v = _list[0];
            _list[0] = _list[_count - 1];
            _list[_count - 1] = default;
            _count--;

            if (_count > 0)
                Heapify(0);

            return v.Value;
        }

        public bool TryPop(out T value)
        {
            if (_count > 0)
            {
                value = Pop();
                return true;
            }
            value = default;
            return false;
        }

        public void Push(T value)
        {
            Append(value);
            Decrease(_count - 1);
        }

        void Heapify(int i)
        {
            while (true)
            {
                int min = i;
                if (Left(i) < _count && IsLesser(Left(i), min))
                    min = Left(i);
                if (Right(i) < _count && IsLesser(Right(i), min))
                    min = Right(i);
                if (min == i)
                    break;

                Swap(i, min);
                i = min;
            }
        }

        void Decrease(int i)
        {
            while (i > 0 && IsLesser(i, Parent(i)))
            {
                Swap(i, Parent(i));
                i = Parent(i);
            }
        }

        static int Left(int i) => 2 * i + 1;
        static int Right(int i) => 2 * i + 2;
        static int Parent(int i) => (i - 1) / 2;

        private bool IsLesser(int a, int b)
        {
            var v = _comparer.Compare(_list[a].Value, _list[b].Value);
            if (v == 0)
                return _list[a].Index < _list[b].Index;

            return v < 0;
        }

        private void Swap(int a, int b)
        {
            var tmp = _list[a];
            _list[a] = _list[b];
            _list[b] = tmp;
        }
    }
}
