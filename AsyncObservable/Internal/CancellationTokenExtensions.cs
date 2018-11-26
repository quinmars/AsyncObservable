using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Quinmars.AsyncObservable
{
    static class CancellationTokenExtensions
    {
        public static CancellationTokenRegistration Register(this CancellationToken ca, IDisposable disposable)
            => ca.Register(o => ((IDisposable)o).Dispose(), disposable);
    }
}
