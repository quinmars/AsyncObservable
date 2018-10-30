using System;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncObserver<in T>
    {
        ValueTask OnSubscibeAsync(IAsyncCancelable disposable);
        ValueTask OnNextAsync(T value);
        ValueTask OnCompletedAsync();
        ValueTask OnErrorAsync(Exception error);
    }
}
