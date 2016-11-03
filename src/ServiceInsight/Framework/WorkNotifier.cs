namespace ServiceInsight.Framework
{
    using System;
    using System.Reactive.Disposables;
    using ServiceInsight.Framework.Events;

    public interface IWorkNotifier
    {
        IDisposable NotifyOfWork();

        IDisposable NotifyOfWork(string startMessage);

        IDisposable NotifyOfWork(string startMessage, string endMessage);
    }

    class WorkNotifier : IWorkNotifier
    {
        IRxEventAggregator eventAggregator;

        public WorkNotifier(IRxEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public IDisposable NotifyOfWork()
        {
            eventAggregator.Publish(new WorkStarted());

            return Disposable.Create(() => eventAggregator.Publish(new WorkFinished()));
        }

        public IDisposable NotifyOfWork(string startMessage)
        {
            eventAggregator.Publish(new WorkStarted(startMessage));

            return Disposable.Create(() => eventAggregator.Publish(new WorkFinished()));
        }

        public IDisposable NotifyOfWork(string startMessage, string endMessage)
        {
            eventAggregator.Publish(new WorkStarted(startMessage));

            return Disposable.Create(() => eventAggregator.Publish(new WorkFinished(endMessage)));
        }
    }
}