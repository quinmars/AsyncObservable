using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Throw<T> : IAsyncObservable<T>
    {
        private readonly Exception _exception;

        public Throw(Exception ex)
        {
            _exception = ex;
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

            var t = observer.OnErrorAsync(_exception);
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
