﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public class TestAsyncScheduler : IAsyncScheduler
    {
        readonly Heap<Item> _heap;
        readonly object _locker = new object();

        public DateTimeOffset StartTime { get; }
        public DateTimeOffset Now { get; private set; }
        public TimeSpan EllapsedTime => Now - StartTime;

        public TestAsyncScheduler(DateTimeOffset startTime)
        {
            StartTime = startTime;
            Now = StartTime;
            _heap = new Heap<Item>(ItemComparer.Instance);
        }

        public TestAsyncScheduler()
        {
            StartTime = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero);
            Now = StartTime;
            _heap = new Heap<Item>(ItemComparer.Instance);
        }

        public Task Delay(TimeSpan ts, CancellationToken ca)
        {
            var item = new Item(Now + ts);
            ca.Register(i => ((Item)i).Cancel(), item);
            
            lock (_locker)
            {
                _heap.Push(item);
            }

            return item.Task;
        }


        public async Task<T> RunAsync<T>(Func<Task<T>> func)
        {
            var t = func();
            Run();
            return await t;
        }

        public void Run()
        {
            while (true)
            {
                Item item;
                lock (_locker)
                {
                    if (!_heap.TryPop(out item))
                        return;
                }

                Now = item.DueTime;
                item.Finish();
            }
        }

        class Item
        {
            readonly TaskCompletionSource<Unit> _tcs;

            public DateTimeOffset DueTime { get; }
            public Task Task => _tcs.Task;

            public Item(DateTimeOffset due)
            {
                DueTime = due;
                _tcs = new TaskCompletionSource<Unit>();
            }

            public void Finish()
            {
                _tcs.TrySetResult(default);
            }

            public void Cancel()
            {
                _tcs.TrySetCanceled();
            }
        }

        class ItemComparer : IComparer<Item>
        {
            public static ItemComparer Instance { get; } = new ItemComparer();

            ItemComparer() { }
            public int Compare(Item x, Item y) => DateTimeOffset.Compare(x.DueTime, y.DueTime);
        }

    }
}