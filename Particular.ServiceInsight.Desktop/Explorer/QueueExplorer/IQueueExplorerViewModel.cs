using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
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