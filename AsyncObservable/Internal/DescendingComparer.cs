using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    class DescendingComparer<T, TSelect> : IComparer<T>
    {
        readonly Func<T, TSelect> selector;
        readonly IComparer<TSelect> comparer;

        public DescendingComparer(Func<T, TSelect> selector, IComparer<TSelect> comparer = null)
        {
            this.selector = selector;
            this.comparer = comparer ?? Comparer<TSelect>.Default;
        }

        public int Compare(T x, T y)
        {
            var a = selector(x);
            var b = selector(y);
            return comparer.Compare(b, a);
        }
    }
}
