namespace ServiceInsight.Framework
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Pirac;

    public interface IRxEventAggregator : IDisposable
    {
        IObservable<TEvent> GetEvent<TEvent>();

        void Publish<TEvent>(TEvent sampleEvent);
    }

    class RxEventAggregator : IRxEventAggregator
    {
        Subject<object> subject = new Subject<object>();

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return subject.OfType<TEvent>().AsObservable().ObserveOnPiracBackground();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            subject.OnNext(sampleEvent);
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref subject, null)?.Dispose();
        }
    }
}