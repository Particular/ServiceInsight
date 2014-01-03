using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.LogWindow;
using NServiceBus.Profiler.Desktop.MessageFlow;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.MessageViewers;
using NServiceBus.Profiler.Desktop.Saga;

namespace NServiceBus.Profiler.Desktop.Shell
{
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