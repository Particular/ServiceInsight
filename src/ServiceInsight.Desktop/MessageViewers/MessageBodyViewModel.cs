using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.MessageViewers.HexViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer;
using NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer;

namespace NServiceBus.Profiler.Desktop.MessageViewers
{
    public interface IMessageBodyViewModel : IScreen,
        IHandle<SelectedMessageChanged>
    {
    }

    public class MessageBodyViewModel : Screen, IMessageBodyViewModel
    {
        private static readonly IDictionary<string, MessageContentType> ContentTypeMaps;

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
        public MessageContentType ContentType { get; private set; }

        public bool JsonViewerVisibile
        {
            get { return ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json; }
        }

        public bool XmlViewerVisibile
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

    public enum MessageContentType
    {
        NotSpecified,
        Json,
        Xml,
    }
}