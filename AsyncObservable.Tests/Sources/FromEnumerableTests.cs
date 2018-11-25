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
    public class FromEnumerableTests
    {
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

            await new [] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Count0()
        {
            string result = "";

            await new int[] {}
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task CancelForever()
        {

            string result = "";

            await ForeverOne()
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
