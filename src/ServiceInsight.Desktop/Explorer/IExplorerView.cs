namespace Particular.ServiceInsight.Desktop.Explorer
{
    using Caliburn.Micro;
    using Events;

    public interface IExplorerView :
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>
    {
        void Expand();

        void SelectRow(int rowHandle);

        void ExpandNode(ExplorerItem item);
    }
}