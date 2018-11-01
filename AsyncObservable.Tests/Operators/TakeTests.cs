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
