using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class ReturnTests
    {
        [Fact]
        public async Task Return1()
        {
            string result = "";

            await AsyncObservable.Return(1)
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("1C");
        }
    }
}
