using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public interface IQueueExplorerViewModel : 
        IExplorerViewModel,
        IScreen, 
        IViewAware,
        IQueueConnectionProvider,
        IHandle<QueueMessageCountChanged>,
        IHandle<AutoRefreshBeat>
    {
        IObservableCollection<ExplorerItem> Items { get; }
        ExplorerItem MachineRoot { get; }
        ExplorerItem FolderRoot { get; }
        Queue SelectedQueue { get; }

        Task ConnectToQueue(string computerName);
        void ExpandNodes();
        void DeleteSelectedQueue();
    }
}