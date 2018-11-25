using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncScheduler
    {
        /*
         * Note: This is a preliminary design for the
         * scheduler interface. There remains the
         * question how it will be possible to observe
         * on a given thread/synchronisation context.
         */
        Task Delay(TimeSpan ts, CancellationToken ca);
        DateTimeOffset Now { get; }
    }
}
