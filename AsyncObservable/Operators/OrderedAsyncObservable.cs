using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class OrderedAsyncObservable<T> : IOrderedAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly OrderedAsyncObservable<T> _parent;
        readonly IComparer<T> _comparer;

        public OrderedAsyncObservable(IAsyncObservable<T> source, IComparer<T> comparer)
        {
            _source = source;
            _comparer = comparer;
        }

        OrderedAsyncObservable(IAsyncObservable<T> source, IComparer<T> comparer, OrderedAsyncObservable<T> parent)
            : this(source, comparer)
        {
            _parent = parent;
        }

        IOrderedAsyncObservable<T> IOrderedAsyncObservable<T>.CreateOrderedObservable<TKey>(Func<T, TKey> selector, IComparer<TKey> comparer, bool descending)
        {
            IComparer<T> comb;
            if (descending)
                comb = new DescendingComparer<T, TKey>(selector, comparer);
            else
                comb = new AsscendingComparer<T, TKey>(selector, comparer);

            return new OrderedAsyncObservable<T>(_source, comb, this);
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var comp = _parent == null ? _comparer : ComposeComparer();

            var o = new OrderedAsyncObserver(observer, comp);
            return _source.SubscribeAsync(o);
        }

        IComparer<T> ComposeComparer()
        {
            // first count the ancestors
            var count = 0;
            var cur = this;
            while (cur != null)
            {
                cur = cur._parent;
                count++;
            }

            // create and 
            var array = new IComparer<T>[count];

            // fill the array
            var i = count;
            cur = this;
            while (cur != null)
            {
                i--;
                array[i] = cur._comparer;
                cur = cur._parent;
            }

            return new CompositeComparer(array);
        }

        sealed class CompositeComparer : IComparer<T>
        {
            readonly IComparer<T>[] comparers;

            public CompositeComparer(IComparer<T>[] comparers)
            {
                this.comparers = comparers;
            }

            public int Compare(T x, T y)
            {
                foreach (var c in comparers)
                {
                    var r = c.Compare(x, y);
                    if (r != 0)
                    {
                        return r;
                    }
                }
                return 0;
            }
        }

        sealed class OrderedAsyncObserver : ForwardingAsyncObserver<T>
        {
            Heap<T> heap;

            internal OrderedAsyncObserver(IAsyncObserver<T> source, IComparer<T> comparer) 
                : base(source)
            {
                heap = new Heap<T>(comparer);
            }

            public async override ValueTask OnCompletedAsync()
            {
                if (IsCanceled)
                    return;

                try
                {
                    heap.Build();
                    while (heap.Count > 0 && !IsCanceled)
                        await ForwardNextAsync(heap.Pop()).ConfigureAwait(false);

                }
                catch (Exception error)
                {
                    await SignalErrorAsync(error).ConfigureAwait(false);
                    return;
                }

                await ForwardCompletedAsync().ConfigureAwait(false);
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (!IsCanceled)
                    heap.Append(value);

                return default;
            }

            public override ValueTask OnFinallyAsync()
            {
                heap = null;
                return base.OnFinallyAsync();
            }
        }
    }
}
