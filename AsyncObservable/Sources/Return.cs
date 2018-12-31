using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Return<T> : IAsyncObservable<T>
    {
        readonly T _value;

        public Return(T value)
        {
            _value = value;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new BooleanDisposable();

            await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);

            try
            {
                if (disposable.IsDisposed)
                    return;

                var t = observer.OnNextAsync(_value);

                try
                {
                    await t.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (!disposable.IsDisposed)
                    {
                        await observer.OnErrorAsync(ex).ConfigureAwait(false);
                    }
                    return;
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
