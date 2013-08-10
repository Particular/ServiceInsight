namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IMessageListView : IViewWithGrid
    {
        void SetupContextMenu();
    }
}