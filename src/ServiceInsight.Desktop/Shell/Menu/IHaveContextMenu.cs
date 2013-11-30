using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Desktop.Shell.Menu
{
    public interface IHaveContextMenu
    {
        IObservableCollection<IMenuItem> ContextMenuItems { get; }
        void OnContextMenuOpening();
    }
}