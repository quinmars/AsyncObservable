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
        public async Task Range1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }
    }
}
