namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using System.Collections.Generic;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;
    using ExtensionMethods;
    using HexViewer;
    using JsonViewer;
    using XmlViewer;
    
    public class MessageBodyViewModel : Screen,
        IHandle<SelectedMessageChanged>
    {
        static Dictionary<string, MessageContentType> ContentTypeMaps;

        static MessageBodyViewModel()
        {
            ContentTypeMaps = new Dictionary<string, MessageContentType>
            {
                {"application/json", MessageContentType.Json},
                {"text/json", MessageContentType.Json},
                {"application/xml", MessageContentType.Xml},
                {"text/xml", MessageContentType.Xml},
                {"", MessageContentType.NotSpecified}
            };
        }

        public MessageBodyViewModel(
            HexContentViewModel hexViewer,
            JsonMessageViewModel jsonViewer,
            XmlMessageViewModel xmlViewer)
        {
            HexViewer = hexViewer;
            XmlViewer = xmlViewer;
            JsonViewer = jsonViewer;
        }

        public HexContentViewModel HexViewer { get; private set; }
        public JsonMessageViewModel JsonViewer { get; private set; }
        public XmlMessageViewModel XmlViewer { get; private set; }
        public MessageContentType ContentType { get; private set; }

        public bool JsonViewerVisible
        {
            get { return ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json; }
        }

        public bool XmlViewerVisible
        {
            get { return ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml; }
        }

        public void Handle(SelectedMessageChanged @event)
        {
            var storedMessage = @event.Message;
            if (storedMessage != null)
            {
                ContentType = ContentTypeMaps.GetValueOrDefault(storedMessage.ContentType, MessageContentType.NotSpecified);
            }
            else
            {
                ContentType = MessageContentType.NotSpecified;
            }
        }
    }
}