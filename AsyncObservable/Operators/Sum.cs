using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class SumInt32 : IAsyncObservable<int>
    {
        readonly IAsyncObservable<int> _source;

        public SumInt32(IAsyncObservable<int> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<int> observer)
        {
            var o = new Observer(observer);
            return _source.SubscribeAsync(o);
        }

        class Observer : BaseAsyncObserver<int>
        {
            int _sum;

            public Observer(IAsyncObserver<int> observer)
                : base(observer)
            {
            }

            public override ValueTask OnNextAsync(int value)
            {
                if (!IsDisposed)
                {
                    try
                    {
                        checked
                        {
                            _sum += value;
                        }
                    }
                    catch (Exception error)
                    {
                        return ForwardErrorAsync(error);
                    }
                }
                return default;
            }

            public override async ValueTask OnCompletedAsync()
            {
                if (IsDisposed)
                    return;
                await ForwardNextAsync(_sum);
                await ForwardCompletedAsync();
            }
        }
    }

    class SumDouble : IAsyncObservable<double>
    {
        readonly IAsyncObservable<double> _source;

        public SumDouble(IAsyncObservable<double> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<double> observer)
        {
            var o = new Observer(observer);
            return _source.SubscribeAsync(o);
        }

        class Observer : BaseAsyncObserver<double>
        {
            double _sum;

            public Observer(IAsyncObserver<double> observer)
                : base(observer)
            {
            }

            public override ValueTask OnNextAsync(double value)
            {
                if (!IsDisposed)
                    _sum += value;
                return default;
            }

            public override async ValueTask OnCompletedAsync()
            {
                if (IsDisposed)
                    return;
                await ForwardNextAsync(_sum);
                await ForwardCompletedAsync();
            }
        }
    }
}
