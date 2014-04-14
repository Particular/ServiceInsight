using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer
{
    public interface IXmlMessageViewModel : 
        IScreen,
        IViewAware,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
        void CopyMessageXml();
        bool CanCopyMessageXml();
    }
}