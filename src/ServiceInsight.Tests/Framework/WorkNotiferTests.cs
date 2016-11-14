namespace ServiceInsight.Tests.Framework
{
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;

    public class WorkNotiferTests
    {
        const string StartMessage = "StartMessage";
        const string FinishMessage = "FinishMessage";

        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;

        [SetUp]
        public void TestInitialize()
        {
            eventAggregator = Substitute.For<IEventAggregator>();
            workNotifier = new WorkNotifier(eventAggregator);
        }

        [Test]
        public void WorkNotifierCallsEventsInPairs_DefaultMessagesMatch()
        {
            var disposable = workNotifier.NotifyOfWork();

            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkStarted>(s => s.Message == new WorkStarted().Message));
            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkFinished>());

            eventAggregator.ClearReceivedCalls();

            disposable.Dispose();

            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkStarted>());
            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkFinished>(s => s.Message == new WorkFinished().Message));
        }

        [Test]
        public void WorkNotifierCallsEventsInPairs_StartMessagesMatch()
        {
            var disposable = workNotifier.NotifyOfWork(StartMessage);

            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkStarted>(s => s.Message == StartMessage));
            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkFinished>());

            eventAggregator.ClearReceivedCalls();

            disposable.Dispose();

            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkStarted>());
            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkFinished>(s => s.Message == new WorkFinished().Message));
        }

        [Test]
        public void WorkNotifierCallsEventsInPairs_StartMessagesAndFinishMessagesMatch()
        {
            var disposable = workNotifier.NotifyOfWork(StartMessage, FinishMessage);

            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkStarted>(s => s.Message == StartMessage));
            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkFinished>());

            eventAggregator.ClearReceivedCalls();

            disposable.Dispose();

            eventAggregator.DidNotReceive().PublishOnUIThread(Arg.Any<WorkStarted>());
            eventAggregator.Received(1).PublishOnUIThread(Arg.Is<WorkFinished>(s => s.Message == FinishMessage));
        }
    }
}