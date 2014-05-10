namespace Particular.ServiceInsight.Desktop.Shell.Menu
{
    using Caliburn.PresentationFramework;

    public interface IHaveContextMenu
    {
        IObservableCollection<IMenuItem> ContextMenuItems { get; }
        void OnContextMenuOpening();
    }
}