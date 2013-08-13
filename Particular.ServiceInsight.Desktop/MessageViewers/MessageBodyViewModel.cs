using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.MessageViewers.HexViewer;
using Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer;
using Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer;

namespace Particular.ServiceInsight.Desktop.MessageViewers
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