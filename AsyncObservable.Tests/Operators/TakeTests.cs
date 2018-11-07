using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class TakeTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;

            obs1.Invoking(o => o.Take(-1))
                .Should().Throw<ArgumentOutOfRangeException>();
            obs2.Invoking(o => o.Take(2))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Take(-1))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Take0()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(0)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Take1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(2)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }
    }
}
