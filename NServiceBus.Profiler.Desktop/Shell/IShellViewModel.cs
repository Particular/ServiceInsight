using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Conversations;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Explorer;
using Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer;
using Particular.ServiceInsight.Desktop.Explorer.QueueExplorer;
using Particular.ServiceInsight.Desktop.LogWindow;
using Particular.ServiceInsight.Desktop.MessageHeaders;
using Particular.ServiceInsight.Desktop.MessageList;
using Particular.ServiceInsight.Desktop.MessageViewers;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public interface IShellViewModel : 
        IConductor, 
        IDeactivate,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<AsyncOperationFailedEvent>,
        IWorkTracker
    {
        IQueueExplorerViewModel QueueExplorer { get; }
        IEndpointExplorerViewModel EndpointExplorer { get; }
        IMessageListViewModel Messages { get; }
        IStatusBarManager StatusBarManager { get; }
        ILogWindowViewModel LogWindow { get; }
        IConversationViewModel Conversation { get; }
        IMessageBodyViewModel MessageBody { get; }
        IMessagePropertiesViewModel MessageProperties { get; }
        IShellView View { get; }
        ExplorerItem SelectedExplorerItem { get; }
        bool AutoRefresh { get; }
        void ShutDown();
        void About();
        void Help();
        void DeleteCurrentQueue();
        void ConnectToMachine();
        void ConnectToManagementService();
        void DeleteSelectedMessages();
        void PurgeCurrentQueue();
        void RefreshAll();
        void ImportMessage();
        void ExportMessage();
        void CreateQueue();
        void CreateMessage();
    }
}