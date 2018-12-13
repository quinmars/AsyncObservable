using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class DistinctUntilChangedTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, long> predicate = null;

            obs1.Invoking(o => o.DistinctUntilChanged(predicate))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.DistinctUntilChanged(_ => 5))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.DistinctUntilChanged(predicate))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.DistinctUntilChanged())
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Simple()
        {
            string result = "";

            await new [] {0, 1, 1, 1, 0, 0, 1, 1, 1, 0 }
                .ToAsyncObservable()
                .DistinctUntilChanged()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("01010C");
        }

        [Fact]
        public async Task All()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .DistinctUntilChanged()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .DistinctUntilChanged()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Throw()
        {
            string result = "";

            await AsyncObservable.Throw<int>(new NotImplementedException())
                .DistinctUntilChanged()
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }
    }
}
