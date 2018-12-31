using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Range : IAsyncObservable<int>
    {
        readonly int _start;
        readonly int _end;

        public Range(int start, int count)
        {
            _start = start;
            _end = start + count;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<int> observer)
        {
            var disposable = new BooleanDisposable();

            await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);

            try
            {
                for (int i = _start; i < _end; i++)
                {
                    if (disposable.IsDisposed)
                        break;

                    var t = observer.OnNextAsync(i);

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
