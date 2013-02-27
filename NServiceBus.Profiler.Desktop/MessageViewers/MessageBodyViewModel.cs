using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.MessageViewers.HexViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer;

namespace NServiceBus.Profiler.Desktop.MessageViewers
{
    public interface IMessageBodyViewModel : 
        IScreen
    {
        
    }

    public class MessageBodyViewModel : Screen, IMessageBodyViewModel
    {
        public MessageBodyViewModel(
            IHexContentViewModel hexViewer, 
            IJsonMessageViewModel jsonViewer,
            IXmlMessageViewModel xmlViewer)
        {
            HexViewer = hexViewer;
            XmlViewer = xmlViewer;
            JsonViewer = jsonViewer;
        }

        public IHexContentViewModel HexViewer { get; private set; }
        public IJsonMessageViewModel JsonViewer { get; private set; }
        public IXmlMessageViewModel XmlViewer { get; private set; }
    }
}