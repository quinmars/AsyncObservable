using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class ThrowTests
    {
        [Fact]
        public async Task Throw1()
        {
            string result = "";

            var d = await AsyncObservable.Throw<int>(new Exception())
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("E");
        }
    }
}
