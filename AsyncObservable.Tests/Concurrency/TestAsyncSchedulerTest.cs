using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;
using FluentAssertions.Extensions;
using System.Linq;

namespace Tests
{
    public class TestAsyncSchedulerTests
    {
        [Fact]
        public void EmptyRun()
        {
            var scheduler = new TestAsyncScheduler();
            var t1 = scheduler.Now;

            scheduler.Run();

            scheduler.EllapsedTime
                .Should().Be(TimeSpan.Zero);

            (scheduler.Now - t1)
                .Should().Be(TimeSpan.Zero);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task OneItem(int ms)
        {
            var delay = TimeSpan.FromMilliseconds(ms);

            var scheduler = new TestAsyncScheduler();
            var t1 = scheduler.Now;
            var t2 = default(DateTimeOffset);

            var task = scheduler.Delay(delay, default);
            var task2 = task.ContinueWith(t => t2 = scheduler.Now);

            scheduler.Run();

            await task2;

            scheduler.EllapsedTime
                .Should().Be(delay);

            (t2 - t1)
                .Should().Be(delay);

            (scheduler.Now - t1)
                .Should().Be(delay);
        }

        [Fact]
        public async Task Many()
        {
            var scheduler = new TestAsyncScheduler();

            async Task<List<TimeSpan>> Run(TimeSpan delay, int n)
            {
                var l = new List<TimeSpan>();
                for (int i = 0; i< n;  i++)
                {
                    await scheduler.Delay(delay, default);
                    l.Add(scheduler.EllapsedTime);
                }
                return l;
            }

            var t = Run(20.Seconds(), 10);
            scheduler.Run();
            var list = await t;

            var expected = Enumerable.Range(1, 10).Select(i => (i * 20).Seconds());

            list
                .Should().Equal(expected);
        }

        [Fact]
        public async Task ManyParallel()
        {
            var scheduler = new TestAsyncScheduler();

            async Task<List<TimeSpan>> Run(TimeSpan delay, int n)
            {
                var l = new List<TimeSpan>();
                for (int i = 0; i< n;  i++)
                {
                    await scheduler.Delay(delay, default);
                    l.Add(scheduler.EllapsedTime);
                }
                return l;
            }

            var t1 = Run(20.Seconds(), 10);
            var t2 = Run(20.Seconds(), 10);

            scheduler.Run();

            var list1 = await t1;
            var list2 = await t1;

            var expected = Enumerable.Range(1, 10).Select(i => (i * 20).Seconds());

            list1
                .Should().Equal(expected);
            list2
                .Should().Equal(expected);
        }
    }
}
