using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class AsyncWhereTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, ValueTask<bool>> predicate = null;

            obs1.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(_ => new ValueTask<bool>(true)))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
        }

        static async ValueTask<T> CreateTask<T>(T v)
        {
            await Task.Yield();
            return v;
        }

        [Fact]
        public async Task WhereOdd()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Where(i => CreateTask(i % 2 != 0))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("13579C");
        }

        [Fact]
        public async Task WhereNone()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Where(_ => CreateTask(false))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task WhereAll()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Where(_ => CreateTask(true))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }
    }
}
