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
    public class UnicastSubjectTests
    {
        IDisposable disposable = Disposable.Empty;

        [Fact]
        public async Task Count10()
        {
            string result = "";
            var subject = new UnicastAsyncSubject<int>();

            var s = subject.SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            await subject.OnSubscribeAsync(Disposable.Empty);

            await subject.OnNextAsync(0);
            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.OnNextAsync(3);
            await subject.OnNextAsync(4);
            await subject.OnNextAsync(5);
            await subject.OnNextAsync(6);
            await subject.OnNextAsync(7);
            await subject.OnNextAsync(8);
            await subject.OnNextAsync(9);

            await subject.OnCompletedAsync();
            await subject.OnFinallyAsync();

            await s;

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Count0()
        {
            string result = "";
            var subject = new UnicastAsyncSubject<int>();

            var s = subject.SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            await subject.OnSubscribeAsync(Disposable.Empty);

            await subject.OnCompletedAsync();
            await subject.OnFinallyAsync();

            await s;

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Throwing()
        {
            string result = "";
            var subject = new UnicastAsyncSubject<int>();

            var s = subject.SubscribeAsync(i => result += i, onCompleted: () => result += "C", onError: ex => result += "E");

            await subject.OnSubscribeAsync(Disposable.Empty);

            await subject.OnNextAsync(0);
            await subject.OnErrorAsync(new Exception());

            await subject.OnFinallyAsync();

            await s;

            result
                .Should().Be("0E");
        }

        [Fact]
        public async Task LateSubscribe()
        {
            string result = "";
            var subject = new UnicastAsyncSubject<int>();


            var t = subject.OnSubscribeAsync(Disposable.Empty);
            var s = subject.SubscribeAsync(i => result += i, onCompleted: () => result += "C", onError: ex => result += "E");

            await t;
            await subject.OnNextAsync(0);
            await subject.OnCompletedAsync();

            await subject.OnFinallyAsync();

            await s;

            result
                .Should().Be("0C");
        }

        [Fact]
        public async Task SubscriptionTask()
        {
            string result = "";
            var subject = new UnicastAsyncSubject<int>();

            var s = subject.SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            await subject.OnSubscribeAsync(Disposable.Empty);

            await subject.OnNextAsync(0);

            await subject.OnCompletedAsync();

            s.IsCompleted
                .Should().BeFalse();

            await subject.OnFinallyAsync();

            await s;
        }
    }
}
