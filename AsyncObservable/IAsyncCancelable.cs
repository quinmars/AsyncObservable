using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncCancelable : IAsyncDisposable, IDisposable
    {
        bool IsDisposing { get; }
        bool IsDisposed { get; }
    }
}
