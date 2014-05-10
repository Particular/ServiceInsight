namespace Particular.ServiceInsight.Desktop.Shell
{
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;
    using Explorer;
    using Explorer.EndpointExplorer;
    using Explorer.QueueExplorer;
    using LogWindow;
    using MessageFlow;
    using MessageList;
    using MessageProperties;
    using MessageViewers;
    using Saga;

    public interface IShellViewModel : 
        IConductor, 
        IDeactivate,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<SwitchToMessageBody>,
        IHandle<SwitchToSagaWindow>,
        IHandle<SwitchToFlowWindow>,
        IWorkTracker
    {
        IQueueExplorerViewModel QueueExplorer { get; }
        IEndpointExplorerViewModel EndpointExplorer { get; }
        IMessageListViewModel Messages { get; }
        IStatusBarManager StatusBarManager { get; }
        ILogWindowViewModel LogWindow { get; }
        ISagaWindowViewModel SagaWindow { get; }
        IMessageFlowViewModel MessageFlow { get; }
        IMessageBodyViewModel MessageBody { get; }
        IMessagePropertiesViewModel MessageProperties { get; }
        IShellView View { get; }
        ExplorerItem SelectedExplorerItem { get; }
        bool AutoRefresh { get; }
        bool BodyTabSelected { get; set; }
        void ShutDown();
        void About();
        void Help();
        void DeleteCurrentQueue();
        void ConnectToMessageQueue();
        void ConnectToServiceControl();
        void DeleteSelectedMessages();
        void PurgeCurrentQueue();
        void RefreshAll();
        void ImportMessage();
        void ExportMessage();
        Task CreateQueue();
        void CreateMessage();
        void OnSelectedTabbedViewChanged(object view);
    }
}