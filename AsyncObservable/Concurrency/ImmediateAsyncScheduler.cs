using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable.Concurrency
{
    public class ImmediateAsyncScheduler : IAsyncScheduler
    {
        public Task Delay(TimeSpan ts, CancellationToken ca)
        {
            return Task.Delay(ts, ca);
        }

        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
