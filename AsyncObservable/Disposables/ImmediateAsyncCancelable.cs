using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class ImmediateAsyncCancelable : IAsyncCancelable
    {
        public bool IsDisposing { get; private set; }

        public ValueTask DisposeAsync()
        {
            IsDisposing = true;
            return new ValueTask();
        }
    }
}
