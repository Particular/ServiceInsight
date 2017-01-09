namespace ServiceInsight.MessageViewers
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using ExtensionMethods;
    using Framework;
    using HexViewer;
    using JsonViewer;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.MessageList;
    using ServiceInsight.ServiceControl;
    using XmlViewer;

    public class MessageBodyViewModel : Screen,
        IHandle<SelectedMessageChanged>,
        IHandle<BodyTabSelectionChanged>,
        IHandle<ServiceControlConnectionChanged>
    {
        readonly IServiceControl serviceControl;
        readonly IWorkNotifier workNotifier;
        readonly MessageSelectionContext selection;
        static Dictionary<string, MessageContentType> contentTypeMaps;

        static MessageBodyViewModel()
        {
            contentTypeMaps = new Dictionary<string, MessageContentType>
            {
                { "application/json", MessageContentType.Json },
                { "text/json", MessageContentType.Json },
                { "application/xml", MessageContentType.Xml },
                { "text/xml", MessageContentType.Xml },
                { "", MessageContentType.NotSpecified }
            };
        }

        public MessageBodyViewModel(
            HexContentViewModel hexViewer,
            JsonMessageViewModel jsonViewer,
            XmlMessageViewModel xmlViewer,
            IServiceControl serviceControl,
            IWorkNotifier workNotifier,
            MessageSelectionContext selectionContext)
        {
            this.serviceControl = serviceControl;
            this.workNotifier = workNotifier;
            selection = selectionContext;

            HexViewer = hexViewer;
            XmlViewer = xmlViewer;
            JsonViewer = jsonViewer;
        }

        public HexContentViewModel HexViewer { get; }

        public JsonMessageViewModel JsonViewer { get; }

        public XmlMessageViewModel XmlViewer { get; }

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

        public bool JsonViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json)
            && PresentationHint == PresentationHint.Standard;

        public bool XmlViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml)
            && PresentationHint == PresentationHint.Standard;

        public bool HexViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json || ContentType == MessageContentType.Xml)
            && PresentationHint == PresentationHint.Standard;

        public bool NoContentHelpNotVisible => PresentationHint != PresentationHint.NoContent;

        public bool NoContentHelpVisible => PresentationHint == PresentationHint.NoContent;

        public void Handle(SelectedMessageChanged @event)
        {
            LoadMessageBody();

            if (selection.SelectedMessage != null)
            {
                ContentType = contentTypeMaps.GetValueOrDefault(selection.SelectedMessage.ContentType, MessageContentType.NotSpecified);

                if (selection.SelectedMessage.Body != null)
                {
                    PresentationHint = selection.SelectedMessage.Body.Hint;
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
                messageDisplay.Display(selection.SelectedMessage);
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

        public void Handle(ServiceControlConnectionChanged message)
        {
            foreach (var messageDisplay in MessageDisplays)
            {
                messageDisplay.Clear();
            }
        }

        void LoadMessageBody()
        {
            if (selection.SelectedMessage == null || !ShouldLoadMessageBody || selection.SelectedMessage.BodyUrl.IsEmpty())
            {
                return;
            }

            using (workNotifier.NotifyOfWork("Loading message body..."))
            {
                serviceControl.LoadBody(selection.SelectedMessage);

                RefreshChildren();
            }
        }
    }
}