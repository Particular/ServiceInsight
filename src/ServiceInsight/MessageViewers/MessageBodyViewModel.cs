﻿using ServiceInsight.MessageViewers.CustomMessageViewer;

namespace ServiceInsight.MessageViewers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        IHandleWithTask<SelectedMessageChanged>,
        IHandleWithTask<BodyTabSelectionChanged>,
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
            ICustomMessageViewerResolver customMessageViewerResolver,
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
            CustomViewer = customMessageViewerResolver.GetCustomMessageBodyViewer();
        }

        public HexContentViewModel HexViewer { get; }

        public JsonMessageViewModel JsonViewer { get; }

        public XmlMessageViewModel XmlViewer { get; }

        public ICustomMessageBodyViewer CustomViewer { get; }

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
                yield return CustomViewer;
            }
        }

        public bool JsonViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json)
            && PresentationHint == PresentationHint.Standard;

        public bool XmlViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Xml)
            && PresentationHint == PresentationHint.Standard;

        public bool HexViewerVisible => (ContentType == MessageContentType.NotSpecified || ContentType == MessageContentType.Json || ContentType == MessageContentType.Xml)
            && PresentationHint == PresentationHint.Standard;

        public bool CustomViewerVisible => CustomViewer.IsVisible(selection.SelectedMessage, PresentationHint);

        public string CustomViewerName => (CustomViewer as IScreen)?.DisplayName;

        public bool NoContentHelpNotVisible => PresentationHint != PresentationHint.NoContent;

        public bool NoContentHelpVisible => PresentationHint == PresentationHint.NoContent;

        public async Task Handle(SelectedMessageChanged @event)
        {
            ClearMessageDisplays();
            await LoadMessageBody();

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

            NotifyOfPropertyChange(nameof(CustomViewerVisible));
            NotifyOfPropertyChange(nameof(HexViewerVisible));
            NotifyOfPropertyChange(nameof(JsonViewerVisible));
            NotifyOfPropertyChange(nameof(XmlViewerVisible));
        }

        void RefreshChildren()
        {
            foreach (var messageDisplay in MessageDisplays)
            {
                messageDisplay.Display(selection.SelectedMessage);
            }
        }

        public async Task Handle(BodyTabSelectionChanged @event)
        {
            ShouldLoadMessageBody = @event.IsSelected;
            if (ShouldLoadMessageBody)
            {
                await LoadMessageBody();
            }
        }

        public void Handle(ServiceControlConnectionChanged message)
        {
            ClearMessageDisplays();
        }

        void ClearMessageDisplays()
        {
            foreach (var messageDisplay in MessageDisplays)
            {
                messageDisplay.Clear();
            }
        }

        async Task LoadMessageBody()
        {
            if (selection.SelectedMessage == null || !ShouldLoadMessageBody || selection.SelectedMessage.BodyUrl.IsEmpty())
            {
                return;
            }

            using (workNotifier.NotifyOfWork("Loading message body..."))
            {
                await serviceControl.LoadBody(selection.SelectedMessage);

                RefreshChildren();
            }
        }
    }
}