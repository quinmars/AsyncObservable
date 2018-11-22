using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class AsyncSelectTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, ValueTask<int>> selector = null;

            obs1.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(i => new ValueTask<int>(i)))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
        }

        static async ValueTask<T> CreateTask<T>(T v)
        {
            await Task.Yield();
            return v;
        }

        [Fact]
        public async Task Select1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(i => CreateTask(9 - i))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("9876543210C");
        }

        [Fact]
        public async Task Throw()
        {
            string result = "";
            Func<int, ValueTask<int>> selector = i => throw new NotImplementedException();

            await AsyncObservable.Range(0, 10)
                .Select(selector)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }
    }
}
