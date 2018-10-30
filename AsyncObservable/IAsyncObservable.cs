using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncObservable<out T>
    {
        ValueTask SubscribeAsync(IAsyncObserver<T> observer);
    }
}
