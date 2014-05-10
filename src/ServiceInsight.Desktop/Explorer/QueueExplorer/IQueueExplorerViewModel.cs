namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
{
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Events;
    using Models;

    public interface IQueueExplorerViewModel : 
        IExplorerViewModel,
        IScreen, 
        IViewAware,
        IQueueConnectionProvider,
        IHandle<QueueMessageCountChanged>
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