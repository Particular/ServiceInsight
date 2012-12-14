using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Plugins;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.MessageList;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public interface IShellViewModel : 
        IConductor, 
        IHandle<WorkStartedEvent>,
        IHandle<WorkFinishedEvent>,
        IWorkTracker
    {
        IExplorerViewModel Explorer { get; }
        IMessageListViewModel Messages { get; }
        IStatusBarManager StatusBarManager { get; }
        IObservableCollection<IPlugin> Plugins { get; }
        IPlugin SelectedPlugin { get; }
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