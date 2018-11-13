using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;

namespace Tests
{
    public class AggregateTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 4);
            var obs2 = (IAsyncObservable<int>)null;
            var seed = (Func<int>)null;
            var agg = (Func<int, int, int>)null;

            obs1.Invoking(o => o.Aggregate(() => 0, agg))
                .Should().Throw<ArgumentNullException>();
            obs1.Invoking(o => o.Aggregate(seed, (a, c) => a + c))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Aggregate(() => 0, (a, c) => a + c))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Sum()
        {
            double result = 0.0;

            await new[] { 0.0, 4.0, 13.0, 3.0}.ToAsyncObservable()
                .Aggregate(() => 0.0, (a, c) => a + c)
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(20.0);
        }

        [Fact]
        public async Task List()
        {
            List<int> result = null;

            await new[] { 0, 1, 2, 3}.ToAsyncObservable()
                .Aggregate(
                    () => new List<int>(),
                    (a, c) => {
                        a.Add(c);
                        return a;
                    })
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Equal(0, 1, 2, 3);
        }

        [Fact]
        public async Task ThrowSeed()
        {
            List<int> result = null;
            Exception exp = null;

            await new[] { 0, 1, 2, 3}.ToAsyncObservable()
                .Aggregate(
                    () => throw new Exception("Seed"),
                    (List<int> a, int c) => {
                        a.Add(c);
                        return a;
                    })
                .SubscribeAsync(i => result = i, ex => exp = ex);

            result
                .Should().BeNull();
            exp
                .Should().BeOfType<Exception>();
        }

        [Fact]
        public async Task ThrowAggregator()
        {
            List<int> result = null;
            Exception exp = null;

            await new[] { 0, 1, 2, 3}.ToAsyncObservable()
                .Aggregate(
                    () => new List<int>(),
                    (a, c) => throw new Exception("Aggregator"))
                .SubscribeAsync(i => result = i, ex => exp = ex);

            result
                .Should().BeNull();
            exp
                .Should().BeOfType<Exception>();
        }
    }
}
