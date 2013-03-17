using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Desktop.About;
using NServiceBus.Profiler.Desktop.Conversations;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ManagementService;
using NServiceBus.Profiler.Desktop.MessageHeaders;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.ScreenManager;
using System.Linq;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShellViewModel
    {
        private readonly IScreenFactory _screenFactory;
        private readonly IWindowManagerEx _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private DispatcherTimer _timer;
        
        public const int AutoRefreshInterval = 15000; //TODO: Wire to configuration/settings

        public ShellViewModel(
            IScreenFactory screenFactory,
            IWindowManagerEx windowManager,
            IQueueExplorerViewModel queueExplorer, 
            IEndpointExplorerViewModel endpointExplorer,
            IMessageListViewModel messages,
            IStatusBarManager statusBarManager,
            IEventAggregator eventAggregator,
            IConversationViewModel conversation,
            IMessageBodyViewModel messageBodyViewer,
            IEnumerable<IHeaderInfoViewModel> headers)
        {
            _screenFactory = screenFactory;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            Headers = headers.OrderBy(x => x.Order);
            Conversation = conversation;
            StatusBarManager = statusBarManager;
            QueueExplorer = queueExplorer;
            EndpointExplorer = endpointExplorer;
            MessageBody = messageBodyViewer;
            Messages = messages;

            Items.Add(messageBodyViewer);
            Items.Add(conversation);
            Items.Add(queueExplorer);
            Items.Add(messages);

            InitializeAutoRefreshTimer();
        }

        private void InitializeAutoRefreshTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AutoRefreshInterval) };
            _timer.Tick += (s, e) => OnAutoRefreshing();
            _timer.Start();
        }

        internal void OnAutoRefreshing()
        {
            if(!AutoRefreshQueues || WorkInProgress) return;
            _eventAggregator.Publish(new AutoRefreshBeat());
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            View = (IShellView)view;

            DisplayName = "NServiceBus Profiler";
            StatusBarManager.Status = "Done";
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _timer.Stop();
        }

        public IEnumerable<IHeaderInfoViewModel> Headers { get; private set; }

        public virtual IShellView View { get; private set; }

        public virtual IQueueExplorerViewModel QueueExplorer { get; private set; }

        public virtual IEndpointExplorerViewModel EndpointExplorer { get; private set; }

        public virtual IMessageListViewModel Messages { get; private set; }

        public virtual IConversationViewModel Conversation { get; private set; }

        public virtual IMessageBodyViewModel MessageBody { get; private set; }

        public virtual IStatusBarManager StatusBarManager { get; private set; }

        public virtual bool WorkInProgress { get; set; }

        public virtual void ExitApp()
        {
            Application.Current.Shutdown(0);
        }

        public virtual void ShowAbout()
        {
            var aboutViewModel = _screenFactory.CreateScreen<AboutViewModel>();
            _windowManager.ShowDialog(aboutViewModel);
        }

        public virtual void ShowHelp()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public virtual void ConnectToMachine()
        {
            var machineViewModel = _screenFactory.CreateScreen<ConnectToMachineViewModel>();
            var result = _windowManager.ShowDialog(machineViewModel);

            if(result.GetValueOrDefault(false))
            {
                QueueExplorer.ConnectToQueue(machineViewModel.ComputerName);
            }
        }

        public virtual bool AutoRefreshQueues { get; set; }

        public void OnAutoRefreshQueuesChanged()
        {
            
        }

        [AutoCheckAvailability]
        public virtual void ConnectToManagementService()
        {
            var connectionViewModel = _screenFactory.CreateScreen<ManagementConnectionViewModel>();
            var result = _windowManager.ShowDialog(connectionViewModel);

            if (result.GetValueOrDefault(false))
            {
                EndpointExplorer.ConnectToService(connectionViewModel.ServiceUrl);
            }
        }

        [AutoCheckAvailability]
        public virtual void DeleteSelectedMessages()
        {
            Messages.DeleteSelectedMessages();
        }

        [AutoCheckAvailability]
        public virtual void PurgeCurrentQueue()
        {
            Messages.PurgeQueue();
        }

        [AutoCheckAvailability]
        public virtual void DeleteCurrentQueue()
        {
            QueueExplorer.DeleteSelectedQueue();
        }

        [AutoCheckAvailability]
        public virtual void RefreshQueues()
        {
            QueueExplorer.FullRefresh();
        }

        [AutoCheckAvailability]
        public virtual void ImportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public virtual void ExportMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        [AutoCheckAvailability]
        public virtual void CreateQueue()
        {
            var screen = _screenFactory.CreateScreen<QueueCreationViewModel>();
            var result = _windowManager.ShowDialog(screen);

            if(result.GetValueOrDefault(false))
            {
                QueueExplorer.FullRefresh();
            }
        }

        [AutoCheckAvailability]
        public virtual void CreateMessage()
        {
            throw new NotImplementedException("This feature is not yet implemented.");
        }

        public virtual bool CanCreateMessage()
        {
            return QueueExplorer.SelectedQueue != null && !WorkInProgress;
        }

        public virtual bool CanRefreshQueues()
        {
            return !WorkInProgress;
        }

        public virtual bool CanPurgeCurrentQueue()
        {
            return Messages.SelectedQueue != null && !WorkInProgress;
        }

        public virtual bool CanDeleteCurrentQueue()
        {
            return Messages.SelectedQueue != null && !WorkInProgress;
        }

        public virtual bool CanDeleteSelectedMessages()
        {
            return Messages.FocusedMessage != null && !WorkInProgress;
        }

        public virtual bool CanCreateQueue()
        {
            return !QueueExplorer.ConnectedToAddress.IsEmpty() && !WorkInProgress;
        }

        public virtual bool CanConnectToMachine()
        {
            return !WorkInProgress;
        }

        public virtual bool CanConnectToManagementService()
        {
            return !WorkInProgress;
        }

        public virtual bool CanExportMessage()
        {
            return !WorkInProgress && Messages.SelectedMessages.Count > 0;
        }

        public virtual bool CanImportMessage()
        {
            return !WorkInProgress;
        }

        public virtual void Handle(WorkStarted @event)
        {
            WorkInProgress = true;
        }

        public virtual void Handle(WorkFinished @event)
        {
            WorkInProgress = false;
        }
    }
}