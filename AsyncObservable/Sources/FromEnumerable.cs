using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class FromEnumerable<T> : IAsyncObservable<T>
    {
        readonly IEnumerable<T> _enumerable;

        public FromEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new BooleanDisposable();

            await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);

            try
            {
                foreach (var item in _enumerable)
                {
                    if (disposable.IsDisposed)
                        break;

                    var t = observer.OnNextAsync(item);

                    try
                    {
                        await t.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await observer.OnErrorAsync(ex).ConfigureAwait(false);
                        break;
                    }
                }

                if (!disposable.IsDisposed)
                    await observer.OnCompletedAsync().ConfigureAwait(false);

            }
            finally
            {
                await observer.OnFinallyAsync().ConfigureAwait(false);
            }
        }
    }
}
