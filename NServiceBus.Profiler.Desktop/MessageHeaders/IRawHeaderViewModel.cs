using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IRawHeaderViewModel : IScreen
    {
        string HeaderContent { get; }
    }
}