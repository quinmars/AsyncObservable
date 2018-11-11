using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class SkipTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;

            obs1.Invoking(o => o.Skip(-1))
                .Should().Throw<ArgumentOutOfRangeException>();
            obs2.Invoking(o => o.Skip(2))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Skip(-1))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Skip0()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Skip(0)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Skip5()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Skip(5)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("56789C");
        }

        [Fact]
        public async Task Skip11()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Skip(11)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }
    }
}
