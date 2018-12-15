using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    public class OrderedByTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, bool> predicate = null;

            obs1.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(_ => true))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Where(predicate))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Already_Ordered()
        {
            var l = await AsyncObservable.Range(1, 5)
                .OrderBy(v => v)
                .ToListAsync();
            l
                .Should().Equal(1, 2, 3, 4, 5);
        }

        [Fact]
        public async Task Inverse()
        {
            var l = await AsyncObservable.Range(1, 5)
                .OrderBy(v => -v)
                .ToListAsync();
            l
                .Should().Equal(5, 4, 3, 2, 1);
        }

        /* First we need a proper SelectMany
        [Fact]
        public async Task Linq()
        {
            var enu = from x in Enumerable.Range(0, 5)
                      from y in Enumerable.Range(0, 5)
                      from z in Enumerable.Range(0, 5)
                      orderby x descending, y ascending, z descending
                      select Tuple.Create(x, y);

            var l = await from x in AsyncObservable.Range(0, 5)
                      from y in AsyncObservable.Range(0, 5)
                      from z in AsyncObservable.Range(0, 5)
                      orderby x descending, y ascending, z descending
                      select Tuple.Create(x, y);

            l
                .Should()
                .Equal(enu);
        }
        */

        [Fact]
        public async Task StableSort()
        {
            var rnd = new Random(0);
            var enu = Enumerable
                .Range(0, 100)
                .Select(x => Tuple.Create(rnd.Next(0, 10), x))
                .ToList();

            var l = await enu
                .ToAsyncObservable()
                .OrderBy(x => x.Item1)
                .ToListAsync();

            l
                .Should()
                .Equal(enu.OrderBy(x => x.Item1));
        }

        [Fact]
        public void ThrowingSelector()
        {
            int Selector(int x) => throw new Exception("test");

            Func<Task> func = async () =>
                await AsyncObservable.Range(0, 4)
                .OrderBy(Selector)
                .ToListAsync();

            func
                .Should().Throw<Exception>()
                .WithMessage("test");
        }

        class IntThrowingComparer : IComparer<int>
        {
            public int Compare(int x, int y) => throw new NotImplementedException("test");
        }

        [Fact]
        public void ThrowingComparer()
        {
            Func<Task> func = async () =>
                await AsyncObservable.Range(0, 4)
                .OrderBy(x => x, new IntThrowingComparer())
                .ToListAsync();

            func
                .Should().Throw<Exception>()
                .WithMessage("test");
        }
    }
}
