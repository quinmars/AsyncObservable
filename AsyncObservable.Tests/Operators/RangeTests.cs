using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class RangeTests
    {
        [Fact]
        public async Task Count10()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Count0()
        {
            string result = "";

            await AsyncObservable.Range(0, 0)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task NegativStartPoint()
        {
            string result = "";

            await AsyncObservable.Range(-3, 5)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("-3-2-101C");
        }
    }
}
