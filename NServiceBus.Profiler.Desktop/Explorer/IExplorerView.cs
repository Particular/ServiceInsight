using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Explorer
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