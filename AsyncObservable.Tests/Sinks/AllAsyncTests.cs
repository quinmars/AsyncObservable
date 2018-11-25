using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class AllAsyncTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<Task> action;

            action = async () => await default(IAsyncObservable<int>).AllAsync(i => true);
            action
                .Should().Throw<ArgumentNullException>();

            action = async () => await AsyncObservable.Empty<int>().AllAsync(null);
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task RangeTrue()
        {
            var value = await AsyncObservable.Range(0, 10).AllAsync(i => i < 20);

            value
                .Should().Be(true);
        }

        [Fact]
        public async Task RangeFalse()
        {
            var value = await AsyncObservable.Range(0, 10).AllAsync(i => i != 0);

            value
                .Should().Be(false);
        }

        [Fact]
        public async Task Empty()
        {
            var value = await AsyncObservable.Empty<int>().AllAsync(i => false);

            value
                .Should().Be(true);
        }

        [Fact]
        public void Throw()
        {
            Func<Task<bool>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).AllAsync(i => i < 0);

            func
                .Should().ThrowExactly<NotImplementedException>();
        }
    }
}
