using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    public interface IOrderedAsyncObservable<out T> : IAsyncObservable<T>
    {
        IOrderedAsyncObservable<T> CreateOrderedObservable<TSelect>(Func<T, TSelect> selector, IComparer<TSelect> comparer, bool descending);
    }
}
