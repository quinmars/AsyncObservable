using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class FirstAsyncTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<Task> action;

            action = async () => await default(IAsyncObservable<int>).FirstAsync();
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Count10()
        {
            var value = await AsyncObservable.Range(1, 10).FirstAsync();

            value
                .Should().Be(1);
        }

        [Fact]
        public async Task Cancellation()
        {
            var result = "";
            var value = await AsyncObservable.Range(1, 10).Do(i => result += i).FirstAsync();

            result
                .Should().Be("1");
        }

        [Fact]
        public async Task Count1()
        {
            var value = await AsyncObservable.Return(1).FirstAsync();

            value
                .Should().Be(1);
        }

        [Fact]
        public void Count0()
        {
            Func<Task<int>> func = async () => await AsyncObservable.Empty<int>().FirstAsync();

            func
                .Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Throw()
        {
            Func<Task<int>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).FirstAsync();

            func
                .Should().ThrowExactly<NotImplementedException>();
        }
    }
}
