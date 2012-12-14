using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public interface IExplorerView : IHandle<WorkStartedEvent>, IHandle<WorkFinishedEvent>
    {
        void Expand();
        void SelectRow(int rowHandle);
        void ExpandNode(ExplorerItem item);
    }
}