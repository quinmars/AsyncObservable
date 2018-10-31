using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Empty<T> : IAsyncObservable<T>
    {
        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new ImmediateAsyncCancelable();

            await observer.OnSubscibeAsync(disposable);

            if (!disposable.IsDisposing)
                await observer.OnCompletedAsync();

        }
    }
}
