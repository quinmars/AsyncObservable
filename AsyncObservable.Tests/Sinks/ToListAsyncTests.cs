using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;

namespace Tests
{
    public class ToListAsyncTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<Task> action;

            action = async () => await default(IAsyncObservable<int>).ToListAsync();
            action
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Range()
        {
            var value = await AsyncObservable.Range(0, 10).ToListAsync();

            value
                .Should().Equal(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        }

        [Fact]
        public async Task Empty()
        {
            var value = await AsyncObservable.Empty<int>().ToListAsync();

            value
                .Should().HaveCount(0);
        }

        [Fact]
        public void Throw()
        {
            Func<Task<List<int>>> func = async () => await AsyncObservable.Throw<int>(new NotImplementedException()).ToListAsync();

            func
                .Should().ThrowExactly<NotImplementedException>();
        }
    }
}
