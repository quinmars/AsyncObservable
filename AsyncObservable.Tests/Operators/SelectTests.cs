using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class SelectTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, int> selector = null;

            obs1.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(i => i))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Select1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(i => 9 - i)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("9876543210C");
        }
    }
}
