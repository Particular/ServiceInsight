using System.Collections;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.MessageViewers.HexViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers
{
    public interface IMessageBodyViewModel : 
        IScreen,
        IHandle<MessageBodyLoaded>
    {
    }

    public class MessageBodyViewModel : Screen, IMessageBodyViewModel
    {
        private static IDictionary<string, MessageContentType> ContentTypeMaps;

        static MessageBodyViewModel()
        {
            ContentTypeMaps = new Dictionary<string, MessageContentType>
            {
                {"application/json", MessageContentType.Json},
                {"text/json", MessageContentType.Json},
                {"application/xml", MessageContentType.Xml},
                {"text/xml", MessageContentType.Xml}
            };
        }

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

        public MessageContentType ContentType
        {
            get; set;
        }

        public bool JsonViewerVisibile
        {
            get { return ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json; }
        }

        public bool XmlViewerVisibile
        {
            get { return ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml; }
        }

        public void Handle(MessageBodyLoaded @event)
        {
            var storedMessage = @event.Message as StoredMessage;
            if (storedMessage != null && ContentTypeMaps.ContainsKey(storedMessage.ContentType))
            {
                ContentType = ContentTypeMaps[storedMessage.ContentType];
            }
            else
            {
                ContentType = MessageContentType.NotSpecified;
            }
        }
    }

    public enum MessageContentType
    {
        NotSpecified,
        Json,
        Xml,
    }
}