using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.XmlViewer
{
    public interface IXmlMessageViewModel : 
        IPlugin, 
        IViewAware,
        IHandle<MessageBodyLoadedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        MessageBody SelectedMessage { get; set; }
        void CopyMessageXml();
        bool CanCopyMessageXml();
    }
}