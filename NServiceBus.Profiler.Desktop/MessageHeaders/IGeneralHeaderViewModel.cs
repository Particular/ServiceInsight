using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IGatewayHeaderViewModel : IScreen
    {
    }

    public interface IGeneralHeaderViewModel : IScreen
    {
        bool CanCopyHeaderInfo();
        void CopyHeaderInfo();
    }
}