using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer
{
    public interface IXmlMessageViewModel : 
        IScreen,
        IViewAware,
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
        void CopyMessageXml();
        bool CanCopyMessageXml();
    }
}