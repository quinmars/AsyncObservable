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

            await observer.OnSubscibeAsync(disposable);

            if (disposable.IsDisposed)
            {
                await observer.DisposeAsync();
                return;
            }

            var t = observer.OnNextAsync(_value);

            try
            {
                await t;
            }
            catch (Exception ex)
            {
                if (!disposable.IsDisposed)
                {
                    await observer.OnErrorAsync(ex);
                }
                await observer.DisposeAsync();
                return;
            }

            if (!disposable.IsDisposed)
                await observer.OnCompletedAsync();

            await observer.DisposeAsync();
        }
    }
}
