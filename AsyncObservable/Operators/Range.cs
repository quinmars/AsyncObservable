using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Range : IAsyncObservable<int>
    {
        private readonly int _start;
        private readonly int _end;

        public Range(int start, int count)
        {
            _start = start;
            _end = start + count;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<int> observer)
        {
            var disposable = new ImmediateAsyncCancelable();

            await observer.OnSubscibeAsync(disposable);

            for (int i = _start; i < _end; i++)
            {
                if (disposable.IsDisposing)
                {
                    return;
                }

                var t = observer.OnNextAsync(i);

                try
                {
                    await t;
                }
                catch (Exception ex)
                {
                    await observer.OnErrorAsync(ex);
                    return;
                }
            }

            if (!disposable.IsDisposing)
                await observer.OnCompletedAsync();

        }
    }
}
