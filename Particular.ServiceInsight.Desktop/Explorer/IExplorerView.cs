using Caliburn.PresentationFramework.ApplicationModel;
using Particular.ServiceInsight.Desktop.Events;

namespace Particular.ServiceInsight.Desktop.Explorer
{
    public interface IExplorerView : 
        IHandle<WorkStarted>, 
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailedEvent>
    {
        void Expand();
        void SelectRow(int rowHandle);
        void ExpandNode(ExplorerItem item);
    }
}