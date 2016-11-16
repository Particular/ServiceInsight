namespace ServiceInsight.Framework
{
    using System;
    using System.Reactive.Disposables;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;

    public interface IWorkNotifier
    {
        IDisposable NotifyOfWork();

        IDisposable NotifyOfWork(string startMessage);

        IDisposable NotifyOfWork(string startMessage, string finishMessage);
    }

    class WorkNotifier : IWorkNotifier
    {
        IEventAggregator eventAggregator;

        public WorkNotifier(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public IDisposable NotifyOfWork()
        {
            eventAggregator.PublishOnUIThread(new WorkStarted());

            return Disposable.Create(() => eventAggregator.PublishOnUIThread(new WorkFinished()));
        }

        public IDisposable NotifyOfWork(string startMessage)
        {
            eventAggregator.PublishOnUIThread(new WorkStarted(startMessage));

            return Disposable.Create(() => eventAggregator.PublishOnUIThread(new WorkFinished()));
        }

        public IDisposable NotifyOfWork(string startMessage, string finishMessage)
        {
            eventAggregator.PublishOnUIThread(new WorkStarted(startMessage));

            return Disposable.Create(() => eventAggregator.PublishOnUIThread(new WorkFinished(finishMessage)));
        }
    }
}