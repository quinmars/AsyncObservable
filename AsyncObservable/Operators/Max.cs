using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class MaxInt32 : IAsyncObservable<int>
    {
        readonly IAsyncObservable<int> _source;

        public MaxInt32(IAsyncObservable<int> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<int> observer)
        {
            var o = new Observer(observer);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<int>
        {
            bool _hasValue;
            int _max;

            public Observer(IAsyncObserver<int> observer)
                : base(observer)
            {
            }

            public override ValueTask OnNextAsync(int value)
            {
                if (!IsCanceled)
                {
                    if (_hasValue)
                    {
                        if (value > _max)
                        {
                            _max = value;
                        }
                    }
                    else
                    {
                        _hasValue = true;
                        _max = value;
                    }
                }
                return default;
            }

            public override async ValueTask OnCompletedAsync()
            {
                if (IsCanceled)
                    return;

                if (!_hasValue)
                {
                    await ForwardErrorAsync(new InvalidOperationException("Sequence contains no elements!")).ConfigureAwait(false);
                    return;
                }

                await ForwardNextAsync(_max).ConfigureAwait(false);
                await ForwardCompletedAsync().ConfigureAwait(false);
            }
        }
    }

    class MaxDouble : IAsyncObservable<double>
    {
        readonly IAsyncObservable<double> _source;

        public MaxDouble(IAsyncObservable<double> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<double> observer)
        {
            var o = new Observer(observer);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<double>
        {
            bool _hasValue;
            double _max;

            public Observer(IAsyncObserver<double> observer)
                : base(observer)
            {
            }

            public override ValueTask OnNextAsync(double value)
            {
                if (!IsCanceled)
                {
                    if (_hasValue)
                    {
                        // if value is NaN _max should also become NaN
                        if (value > _max || double.IsNaN(value))
                        {
                            _max = value;
                        }
                    }
                    else
                    {
                        _hasValue = true;
                        _max = value;
                    }
                }
                return default;
            }

            public override async ValueTask OnCompletedAsync()
            {
                if (IsCanceled)
                    return;

                if (!_hasValue)
                {
                    await ForwardErrorAsync(new InvalidOperationException("Sequence contains no elements!")).ConfigureAwait(false);
                    return;
                }

                await ForwardNextAsync(_max).ConfigureAwait(false);
                await ForwardCompletedAsync().ConfigureAwait(false);
            }
        }
    }
}
