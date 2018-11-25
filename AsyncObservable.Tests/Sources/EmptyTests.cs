using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class EmptyTests
    {
        [Fact]
        public async Task Empty1()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("C");
        }
    }
}
