using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Core.Settings;
using NServiceBus.Profiler.Desktop.About;
using NServiceBus.Profiler.Desktop.Conversations;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ManagementService;
using NServiceBus.Profiler.Desktop.MessageHeaders;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShellViewModel
    {
        private readonly IScreenFactory _screenFactory;
        private readonly IWindowManagerEx _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsProvider _settingsProvider;
        private int _workCounter;
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
            ISettingsProvider settingsProvider,
            IMessagePropertiesViewModel messageProperties)
        {
            _screenFactory = screenFactory;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _settingsProvider = settingsProvider;
            MessageProperties = messageProperties;
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

        internal void OnAutoRefreshing()
        {
            if(!AutoRefresh || WorkInProgress) 
                return;

            _eventAggregator.Publish(new AutoRefreshBeat());
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            View = (IShellView)view;

            DisplayName = GetProductName();
            StatusBarManager.Status = "Done";
            RestoreLayout();
        }

        public virtual void Deactivate(bool close)
        {
            base.OnDeactivate(close);
            _timer.Stop();
            SaveLayout();
        }

        private void SaveLayout()
        {
            View.SaveLayout(_settingsProvider);
        }

        private void RestoreLayout()
        {
            View.RestoreLayout(_settingsProvider);
        }

        public virtual bool AutoRefresh { get; set; }

        public IEnumerable<IHeaderInfoViewModel> Headers { get; private set; }

        public virtual IShellView View { get; private set; }

        public virtual IMessagePropertiesViewModel MessageProperties { get; private set; }

        public virtual IQueueExplorerViewModel QueueExplorer { get; private set; }

        public virtual IEndpointExplorerViewModel EndpointExplorer { get; private set; }

        public virtual IMessageListViewModel Messages { get; private set; }

        public virtual IConversationViewModel Conversation { get; private set; }

        public virtual IMessageBodyViewModel MessageBody { get; private set; }

        public virtual IStatusBarManager StatusBarManager { get; private set; }

        public virtual ExplorerItem SelectedExplorerItem { get; private set; }

        public virtual bool WorkInProgress
        {
            get { return _workCounter > 0; }
        }

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
        public virtual async void DeleteSelectedMessages()
        {
            await Messages.DeleteSelectedMessages();
        }

        [AutoCheckAvailability]
        public virtual async void PurgeCurrentQueue()
        {
            await Messages.PurgeQueue();
        }

        [AutoCheckAvailability]
        public virtual void DeleteCurrentQueue()
        {
            QueueExplorer.DeleteSelectedQueue();
        }

        [AutoCheckAvailability]
        public virtual async void RefreshQueues()
        {
            await Messages.RefreshMessages();
            await RefreshExplorer();
        }

        private async Task RefreshExplorer()
        {
            var endpointNode = SelectedExplorerItem.As<AuditEndpointExplorerItem>();
            if (endpointNode != null)
            {
                await EndpointExplorer.PartialRefresh();
            }

            var queueNode = SelectedExplorerItem.As<QueueExplorerItem>();
            if (queueNode != null)
            {
                await QueueExplorer.PartialRefresh();
            }
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

        public void OnAutoRefreshQueuesChanged()
        {
            
        }

        public virtual bool CanCreateMessage
        {
            get { return QueueExplorer.SelectedQueue != null && !WorkInProgress; }
        }

        public virtual bool CanRefreshQueues
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanPurgeCurrentQueue
        {
            get
            {
                return Messages.SelectedQueue != null &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanDeleteCurrentQueue
        {
            get
            {
                return Messages.SelectedQueue != null &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanDeleteSelectedMessages
        {
            get
            {
                return !WorkInProgress &&
                       Messages.FocusedMessage != null &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanCreateQueue
        {
            get
            {
                return !QueueExplorer.ConnectedToAddress.IsEmpty() &&
                       !WorkInProgress &&
                       SelectedExplorerItem.IsQueueExplorerSelected();
            }
        }

        public virtual bool CanConnectToMachine
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanConnectToManagementService
        {
            get { return !WorkInProgress; }
        }

        public virtual bool CanExportMessage
        {
            get { return !WorkInProgress && Messages.SelectedMessages.Count > 0 && false; } //TODO: Implement message export
        }

        public virtual bool CanImportMessage
        {
            get { return !WorkInProgress && false; } //TODO: Implement message import
        }

        private void InitializeAutoRefreshTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AutoRefreshInterval) };
            _timer.Tick += (s, e) => OnAutoRefreshing();
            _timer.Start();
        }

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => WorkInProgress);
            NotifyOfPropertyChange(() => CanConnectToMachine);
            NotifyOfPropertyChange(() => CanConnectToManagementService);
            NotifyOfPropertyChange(() => CanCreateMessage);
            NotifyOfPropertyChange(() => CanCreateQueue);
            NotifyOfPropertyChange(() => CanDeleteCurrentQueue);
            NotifyOfPropertyChange(() => CanDeleteSelectedMessages);
            NotifyOfPropertyChange(() => CanExportMessage);
            NotifyOfPropertyChange(() => CanImportMessage);
            NotifyOfPropertyChange(() => CanPurgeCurrentQueue);
            NotifyOfPropertyChange(() => CanRefreshQueues);
        }

        private string GetProductName()
        {
            var productAttribute = GetType().Assembly.GetAttribute<AssemblyProductAttribute>();
            return productAttribute.Product;
        }

        public virtual void Handle(WorkStarted @event)
        {
            _workCounter++;
            NotifyPropertiesChanged();
        }

        public virtual void Handle(WorkFinished @event)
        {
            if (_workCounter <= 0) 
                return;

            _workCounter--;
            NotifyPropertiesChanged();
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }
    }
}