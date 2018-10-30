using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Return<T> : IAsyncObservable<T>
    {
        private readonly T _value;

        public Return(T value)
        {
            _value = value;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new AnonymousAsyncCancelable();

            await observer.OnSubscibeAsync(disposable);

            if (disposable.IsDisposing)
            {
                disposable.SetDisposed();
                return;
            }

            var t = observer.OnNextAsync(_value);

            try
            {
                await t;
            }
            catch (Exception ex)
            {
                await observer.OnErrorAsync(ex);
                disposable.SetDisposed();
                return;
            }

            if (!disposable.IsDisposing)
                await observer.OnCompletedAsync();

            disposable.SetDisposed();
        }
    }
}
