namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Anotar.Serilog;
    using Diagram;
    using Framework.Rx;
    using Microsoft.Win32;
    using Pirac;
    using ServiceInsight.DiagramLegend;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.MessageList;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Settings;

    public class SequenceDiagramViewModel : RxScreen, IMessageCommandContainer
    {
        readonly IServiceControl serviceControl;
        readonly ISettingsProvider settingsProvider;
        string loadedConversationId;
        SequenceDiagramSettings settings;
        SequenceDiagramView view;

        const string SequenceDiagramDocumentationUrl = "http://docs.particular.net/serviceinsight/no-data-available";

        public SequenceDiagramViewModel(
            IRxEventAggregator eventAggregator,
            IServiceControl serviceControl,
            ISettingsProvider settingsProvider,
            MessageSelectionContext selectionContext,
            DiagramLegendViewModel diagramLegend,
            CopyConversationIDCommand copyConversationIDCommand,
            CopyMessageURICommand copyMessageURICommand,
            RetryMessageCommand retryMessageCommand,
            SearchByMessageIDCommand searchByMessageIDCommand,
            ChangeSelectedMessageCommand changeSelectedMessageCommand,
            ShowExceptionCommand showExceptionCommand,
            ReportMessageCommand reportMessageCommand,
            NetworkOperations networkOperations,
            SequenceDiagramView view)
        {
            this.serviceControl = serviceControl;
            this.settingsProvider = settingsProvider;

            Selection = selectionContext;
            CopyConversationIDCommand = copyConversationIDCommand;
            CopyMessageURICommand = copyMessageURICommand;
            RetryMessageCommand = retryMessageCommand;
            SearchByMessageIDCommand = searchByMessageIDCommand;
            ChangeSelectedMessageCommand = changeSelectedMessageCommand;
            ShowExceptionCommand = showExceptionCommand;
            ReportMessageCommand = reportMessageCommand;
            OpenLink = Command.Create(() => networkOperations.Browse(SequenceDiagramDocumentationUrl));
            ExportDiagramCommand = Command.Create(() => ExportToPng(view), () => HasItems);
            DiagramLegend = diagramLegend;
            DiagramItems = new DiagramItemCollection();
            HeaderItems = new DiagramItemCollection();

            AddChildren(diagramLegend);

            settings = settingsProvider.GetSettings<SequenceDiagramSettings>();

            ShowLegend = settings.ShowLegend;

            eventAggregator.GetEvent<SelectedMessageChanged>().ObserveOnPiracMain().Subscribe(Handle);
            eventAggregator.GetEvent<ScrollDiagramItemIntoView>().ObserveOnPiracMain().Subscribe(Handle);
        }

        protected override void OnViewLoaded(FrameworkElement view)
        {
            this.view = (SequenceDiagramView)view;
        }

        void ExportToPng(SequenceDiagramView viewToExport)
        {
            var bodyElement = (UIElement)viewToExport.ScrollViewer_Body.Content;
            var headerElement = (UIElement)viewToExport.ScrollViewer_Header.Content;

            var actualHeight = bodyElement.RenderSize.Height + headerElement.RenderSize.Height;
            var actualWidth = bodyElement.RenderSize.Width;

            var renderTarget = new RenderTargetBitmap((int)actualWidth, (int)actualHeight, 96, 96, PixelFormats.Default);
            var bodySourceBrush = new VisualBrush(bodyElement);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
                drawingContext.DrawRectangle(bodySourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }

            renderTarget.Render(drawingVisual);

            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                FileName = "sequencediagram.png",
                Filter = "Portable Network Graphics (.png)|*.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveAsPNG(renderTarget, saveFileDialog.FileName);
            }
        }

        static void SaveAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        public ICommand ExportDiagramCommand { get; }

        public ICommand OpenLink { get; }

        public ICommand CopyConversationIDCommand { get; }

        public ICommand CopyMessageURICommand { get; }

        public ICommand RetryMessageCommand { get; }

        public ICommand SearchByMessageIDCommand { get; }

        public ICommand ChangeSelectedMessageCommand { get; }

        public ICommand ShowExceptionCommand { get; }

        public ICommand ReportMessageCommand { get; }

        public DiagramLegendViewModel DiagramLegend { get; }

        public DiagramItemCollection DiagramItems { get; }

        public string ErrorMessage { get; set; }

        public ReportMessageCommand.ReportMessagePackage ReportPackage { get; set; }

        public bool ShowLegend { get; set; }

        void OnShowLegendChanged()
        {
            settings.ShowLegend = ShowLegend;
            settingsProvider.SaveSettings(settings);
        }

        public bool HasItems => DiagramItems != null && DiagramItems.Count > 0;

        public DiagramItemCollection HeaderItems { get; }

        public MessageSelectionContext Selection { get; }

        void Handle(SelectedMessageChanged message)
        {
            try
            {
                var conversationId = Selection?.SelectedMessage?.ConversationId;
                if (string.IsNullOrEmpty(conversationId))
                {
                    ClearState();
                    return;
                }

                if (loadedConversationId == conversationId && DiagramItems.Any()) //If we've already displayed this diagram
                {
                    RefreshSelection();
                    return;
                }

                var messages = serviceControl.GetConversationById(conversationId).ToList();
                if (messages.Count == 0)
                {
                    LogTo.Warning("No messages found for conversation id {0}", conversationId);
                    ClearState();
                    return;
                }

                CreateElements(messages);
                loadedConversationId = conversationId;
                RefreshSelection();
            }
            catch (Exception ex)
            {
                ClearState();
                ErrorMessage = $"There was an error processing the message data.";
                ReportPackage = new ReportMessageCommand.ReportMessagePackage(ex, Selection?.SelectedMessage);
            }
        }

        void RefreshSelection()
        {
            foreach (var item in DiagramItems.OfType<Handler>())
            {
                item.IsFocused = false;
            }

            foreach (var item in DiagramItems.OfType<Arrow>())
            {
                if (string.Equals(item.SelectedMessage.Id, Selection.SelectedMessage.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.IsFocused = true;
                    continue;
                }

                item.IsFocused = false;
            }
        }

        void CreateElements(List<StoredMessage> messages)
        {
            var modelCreator = new ModelCreator(messages, this);
            var endpoints = modelCreator.Endpoints;
            var handlers = modelCreator.Handlers;
            var routes = modelCreator.Routes;

            ClearState();

            DiagramItems.AddRange(endpoints);
            DiagramItems.AddRange(endpoints.Select(e => e.Timeline));
            DiagramItems.AddRange(handlers);
            DiagramItems.AddRange(handlers.SelectMany(h => h.Out));
            DiagramItems.AddRange(routes);

            HeaderItems.AddRange(endpoints);

            NotifyOfPropertyChange(nameof(HasItems));
        }

        void ClearState()
        {
            ErrorMessage = "";
            DiagramItems.Clear();
            HeaderItems.Clear();
            NotifyOfPropertyChange(nameof(HasItems));
        }

        void Handle(ScrollDiagramItemIntoView @event)
        {
            var diagramItem = DiagramItems.OfType<Arrow>()
                .FirstOrDefault(a => a.SelectedMessage.Id == @event.Message.Id);

            if (diagramItem != null)
            {
                view?.diagram.BringIntoView(diagramItem);
            }
        }
    }

    public interface IMessageCommandContainer
    {
        ICommand CopyConversationIDCommand { get; }

        ICommand CopyMessageURICommand { get; }

        ICommand RetryMessageCommand { get; }

        ICommand SearchByMessageIDCommand { get; }

        ICommand ChangeSelectedMessageCommand { get; }

        ICommand ShowExceptionCommand { get; }
    }
}