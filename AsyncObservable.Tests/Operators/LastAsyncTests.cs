using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class LastAsyncTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<Task> action;

            action = async () => await default(IAsyncObservable<int>).LastAsync();
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Count10()
        {
            var value = await AsyncObservable.Range(0, 10).LastAsync();

            value
                .Should().Be(9);
        }

        [Fact]
        public async Task Count1()
        {
            var value = await AsyncObservable.Return(1).LastAsync();

            value
                .Should().Be(1);
        }

        [Fact]
        public void Count0()
        {
            Func<Task<int>> func = async () => await AsyncObservable.Empty<int>().LastAsync();

            func
                .Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Throw()
        {
            Func<Task<int>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).LastAsync();

            func
                .Should().ThrowExactly<NotImplementedException>();
        }
    }
}
