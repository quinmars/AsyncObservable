using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }
}