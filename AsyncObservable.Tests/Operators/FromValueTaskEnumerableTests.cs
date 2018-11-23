using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class ValueTaskFromEnumerableTests
    {
        static async ValueTask<T> CreateTask<T>(T v)
        {
            await Task.Yield();
            return v;
        }

        [Fact]
        public void ArgumentExceptions()
        {
            Action action;

            action = () => AsyncObservable.ToAsyncObservable((IEnumerable<int>)null);
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Count10()
        {
            string result = "";
            var tenum = CreateTask(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            await tenum
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Count0()
        {
            string result = "";

            await Enumerable.Empty<int>()
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task CancelForever()
        {

            string result = "";
            var tenum = CreateTask(ForeverOne());

            await tenum
                .ToAsyncObservable()
                .Do(_ => result += "_")
                .Take(5)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("_1_1_1_1_1C");
        }

        static IEnumerable<int> ForeverOne()
        {
            while (true)
                yield return 1;
        }
    }
}
