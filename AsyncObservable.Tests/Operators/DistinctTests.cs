using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class DistinctTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, bool> predicate = null;

            obs1.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(_ => true))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Odd()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(i => i % 2)
                .Distinct()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task KeepOrder()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Distinct()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task TakeFirst()
        {
            string result = "";

            await new [] {0, 1, 2, 3, 3, 2, 2, 1, 1, 2, 2, 1, 3, 1, 3, 1, 4}
                .ToAsyncObservable()
                .Distinct()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("01234C");
        }

        [Fact]
        public async Task DistinctByMod()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .DistinctBy(v => v % 3)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("012C");
        }
    }
}
