using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Desktop.Conversations;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.MessageHeaders;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.MessageViewers;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IShellViewModel : 
        IConductor, 
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IWorkTracker
    {
        IQueueExplorerViewModel QueueExplorer { get; }
        IEndpointExplorerViewModel EndpointExplorer { get; }
        IMessageListViewModel Messages { get; }
        IStatusBarManager StatusBarManager { get; }
        IConversationViewModel Conversation { get; }
        IMessageBodyViewModel MessageBody { get; }
        IEnumerable<IHeaderInfoViewModel> Headers { get; }
        IShellView View { get; }
        void ExitApp();
        void ShowAbout();
        void ShowHelp();
        void DeleteCurrentQueue();
        void ConnectToMachine();
        void DeleteSelectedMessages();
        void PurgeCurrentQueue();
        void RefreshQueues();
        void ImportMessage();
        void CreateQueue();
        void CreateMessage();
    }
}