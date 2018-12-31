using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    // This class is is inspired by Stephen Toub's article:
    // https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/
    //
    internal class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async ValueTask<Releaser> LockAsync()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            return new Releaser(this);
        }

        public struct Releaser : IDisposable
        {
            readonly AsyncLock _asyncLock;

            public Releaser(AsyncLock asyncLock)
                => _asyncLock = asyncLock;

            public void Dispose()
                => _asyncLock?._semaphore.Release();
        }
    }
}
