namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using ExtensionMethods;
    using HexViewer;
    using JsonViewer;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.ServiceControl;
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

        MessageContentType ContentType { get; set; }

        PresentationHint PresentationHint { get; set; }

        public bool JsonViewerVisible
        {
            get { 
                return (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json) 
                            && PresentationHint == PresentationHint.Standard; 
            }
        }
        
        public bool XmlViewerVisible
        {
            get { 
                return (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml) 
                            && PresentationHint == PresentationHint.Standard; 
            }
        }
        public bool HexViewerVisible
        {
            get {
                return (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json || ContentType == MessageContentType.Xml) 
                            && PresentationHint == PresentationHint.Standard; 
            }
        }

        public bool NoContentHelpNotVisible
        {
            get { return PresentationHint != PresentationHint.NoContent; }
        }

        public bool NoContentHelpVisible
        {
            get
            {
                return PresentationHint == PresentationHint.NoContent;
            }
        }

        public void Handle(SelectedMessageChanged @event)
        {
            var storedMessage = @event.Message;
            if (storedMessage != null)
            {
                ContentType = ContentTypeMaps.GetValueOrDefault(storedMessage.ContentType, MessageContentType.NotSpecified);

                if (storedMessage.Body != null)
                {
                    PresentationHint = storedMessage.Body.Hint;
                }

            }
            else
            {
                ContentType = MessageContentType.NotSpecified;
            }
        }
    }
}