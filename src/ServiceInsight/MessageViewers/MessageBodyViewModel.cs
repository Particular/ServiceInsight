namespace Particular.ServiceInsight.Desktop.MessageViewers
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using ExtensionMethods;
    using HexViewer;
    using JsonViewer;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;
    using XmlViewer;

    public class MessageBodyViewModel : Screen,
        IHandle<SelectedMessageChanged>,
        IHandle<BodyTabSelectionChanged>
    {
        readonly IServiceControl serviceControl;
        readonly IEventAggregator eventAggregator;
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
            XmlMessageViewModel xmlViewer,
            IServiceControl serviceControl,
            IEventAggregator eventAggregator)
        {
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            HexViewer = hexViewer;
            XmlViewer = xmlViewer;
            JsonViewer = jsonViewer;
        }

        public HexContentViewModel HexViewer { get; private set; }

        public JsonMessageViewModel JsonViewer { get; private set; }

        public XmlMessageViewModel XmlViewer { get; private set; }

        public StoredMessage SelectedMessage { get; private set; }

        bool ShouldLoadMessageBody { get; set; }

        MessageContentType ContentType { get; set; }

        PresentationHint PresentationHint { get; set; }

        IEnumerable<IDisplayMessageBody> MessageDisplays
        {
            get
            {
                yield return XmlViewer;
                yield return HexViewer;
                yield return JsonViewer;
            }
        }

        public bool JsonViewerVisible
        {
            get
            { 
                return (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json) 
                            && PresentationHint == PresentationHint.Standard; 
            }
        }
        
        public bool XmlViewerVisible
        {
            get
            { 
                return (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml) 
                            && PresentationHint == PresentationHint.Standard; 
            }
        }

        public bool HexViewerVisible
        {
            get
            {
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
            SelectedMessage = @event.Message;

            LoadMessageBody();

            if (SelectedMessage != null)
            {
                ContentType = ContentTypeMaps.GetValueOrDefault(SelectedMessage.ContentType, MessageContentType.NotSpecified);

                if (SelectedMessage.Body != null)
                {
                    PresentationHint = SelectedMessage.Body.Hint;
                }
            }
            else
            {
                ContentType = MessageContentType.NotSpecified;
            }
        }

        void RefreshChildren()
        {
            foreach (var messageDisplay in MessageDisplays)
            {
                messageDisplay.Display(SelectedMessage);
            }
        }

        public void Handle(BodyTabSelectionChanged @event)
        {
            ShouldLoadMessageBody = @event.IsSelected;
            if (ShouldLoadMessageBody)
            {
                LoadMessageBody();
            }
        }

        void LoadMessageBody()
        {
            if (SelectedMessage == null || !ShouldLoadMessageBody || SelectedMessage.BodyUrl.IsEmpty())
            {
                return;
            }

            eventAggregator.Publish(new WorkStarted("Loading message body..."));

            serviceControl.LoadBody(SelectedMessage);

            RefreshChildren();

            eventAggregator.Publish(new WorkFinished());
        }

    }
}