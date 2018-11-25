using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class AnyAsyncTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<Task> action;

            action = async () => await default(IAsyncObservable<int>).AnyAsync();
            action
                .Should().Throw<ArgumentNullException>();

            action = async () => await default(IAsyncObservable<int>).AnyAsync(i => true);
            action
                .Should().Throw<ArgumentNullException>();

            action = async () => await AsyncObservable.Empty<int>().AnyAsync(null);
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Count10()
        {
            var value = await AsyncObservable.Range(0, 10).AnyAsync();

            value
                .Should().Be(true);
        }

        [Fact]
        public async Task Empty()
        {
            var value = await AsyncObservable.Empty<int>().AnyAsync();

            value
                .Should().Be(false);
        }

        [Fact]
        public void Throw()
        {
            Func<Task<bool>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).AnyAsync();

            func
                .Should().ThrowExactly<NotImplementedException>();
        }

        [Fact]
        public async Task Count10WithPredicate()
        {
            var value = await AsyncObservable.Range(0, 10).AnyAsync(i => i > 4);

            value
                .Should().Be(true);
        }

        [Fact]
        public async Task Count10WithPredicate2()
        {
            var value = await AsyncObservable.Range(0, 10).AnyAsync(i => i > 20);

            value
                .Should().Be(false);
        }

        [Fact]
        public async Task EmptyWithPredicate()
        {
            var value = await AsyncObservable.Empty<int>().AnyAsync(i => i > 4);

            value
                .Should().Be(false);
        }

        [Fact]
        public void ThrowWithPredicate()
        {
            Func<Task<bool>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).AnyAsync(v => false);

            func
                .Should().ThrowExactly<NotImplementedException>();
        }
    }
}
