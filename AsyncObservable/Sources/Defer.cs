using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    static class Defer<T>
    {
        public class Sync : IAsyncObservable<T>
        {
            readonly Func<IAsyncObservable<T>> _factory;

            public Sync(Func<IAsyncObservable<T>> factory)
            {
                _factory = factory;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
            {
                var obs = default(IAsyncObservable<T>);

                try
                {
                    obs = _factory();
                }
                catch (Exception error)
                {
                    obs = AsyncObservable.Throw<T>(error);
                }

                return obs.SubscribeAsync(observer);
            }
        }

        public class Async : IAsyncObservable<T>
        {
            readonly Func<ValueTask<IAsyncObservable<T>>> _factory;

            public Async(Func<ValueTask<IAsyncObservable<T>>> factory)
            {
                _factory = factory;
            }

            public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
            {
                var obs = default(IAsyncObservable<T>);

                try
                {
                    obs = await _factory().ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    obs = AsyncObservable.Throw<T>(error);
                }

                await obs.SubscribeAsync(observer).ConfigureAwait(false);
            }
        }
    }
}
