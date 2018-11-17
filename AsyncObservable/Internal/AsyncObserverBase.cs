using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Quinmars.AsyncObservable
{
    abstract class AsyncObserverBase : IDisposable
    {
        private IDisposable _upstream;

        public bool IsCanceled { get; private set; }

        public virtual void Dispose()
        {
            Interlocked.Exchange(ref _upstream, null)?.Dispose();
            IsCanceled = true;
        }

        protected void SetUpstream(IDisposable upstream) => _upstream = upstream;
    }
}
