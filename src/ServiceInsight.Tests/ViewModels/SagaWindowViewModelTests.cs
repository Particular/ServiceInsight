namespace ServiceInsight.Tests.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.MessageList;
    using ServiceInsight.Models;
    using ServiceInsight.Saga;
    using ServiceInsight.ServiceControl;

    [TestFixture]
    public class SagaWindowViewModelTests
    {
        private SagaWindowViewModel viewModel;
        private MessageSelectionContext messageSelectionContext;
        private IServiceControl serviceControl;

        [SetUp]
        public void TestInitialize()
        {
            var eventAggregator = Substitute.For<IEventAggregator>();
            var workNotifier = Substitute.For<IWorkNotifier>();
            var clipboard = Substitute.For<IClipboard>();
            var windowManager = Substitute.For<IWindowManagerEx>();

            serviceControl = Substitute.For<IServiceControl>();
            messageSelectionContext = new MessageSelectionContext(eventAggregator);
            viewModel = new SagaWindowViewModel(eventAggregator, workNotifier, serviceControl, clipboard, windowManager, messageSelectionContext);
        }

        [Test]
        public void When_initiating_message_can_not_be_found_doesnot_throw()
        {
            var sagaId = Guid.NewGuid();
            var timeouts = new List<SagaTimeoutMessage> { new SagaTimeoutMessage { DeliverAt = DateTime.Now, Timeout = TimeSpan.FromMinutes(1) } };
            var update = new SagaUpdate { InitiatingMessage = null, OutgoingMessages = timeouts };

            var sagaChanges = new List<SagaUpdate> { update };
            var sagaList = new List<SagaInfo> { new SagaInfo { SagaId = sagaId } };
            var sagaData = new SagaData { Changes = sagaChanges };

            messageSelectionContext.SelectedMessage = new StoredMessage { InvokedSagas = sagaList };
            serviceControl.GetSagaById(Arg.Is(sagaId)).Returns(sagaData);
            var selectedMessageChanged = new SelectedMessageChanged();

            Assert.DoesNotThrow(() => viewModel.Handle(selectedMessageChanged));
        }
    }
}