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
    using Caliburn.Micro;
    using Diagram;
    using Microsoft.Win32;
    using Particular.ServiceInsight.Desktop.ExtensionMethods;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Framework.Settings;
    using Particular.ServiceInsight.Desktop.MessageList;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;
    using Particular.ServiceInsight.Desktop.Settings;
    using ServiceInsight.DiagramLegend;

    public class SequenceDiagramViewModel : Screen,
        IHandle<SelectedMessageChanged>,
        IMessageCommandContainer
    {
        readonly IServiceControl serviceControl;
        private readonly ISettingsProvider settingsProvider;
        string loadedConversationId;
        private SequenceDiagramSettings settings;

        private const string SequenceDiagramDocumentationUrl = "http://docs.particular.net/serviceinsight/no-data-available";

        public SequenceDiagramViewModel(
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
            OpenLink = this.CreateCommand(arg => new NetworkOperations().Browse(SequenceDiagramDocumentationUrl));
            ExportDiagramCommand = this.CreateCommand(() => ExportToPng(view), m => m.HasItems);
            DiagramLegend = diagramLegend;
            DiagramItems = new DiagramItemCollection();
            HeaderItems = new DiagramItemCollection();

            settings = settingsProvider.GetSettings<SequenceDiagramSettings>();

            ShowLegend = settings.ShowLegend;
        }

        void ExportToPng(SequenceDiagramView view)
        {
            var bodyElement = (UIElement)view.ScrollViewer_Body.Content;
            var headerElement = (UIElement)view.ScrollViewer_Header.Content;

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

        private static void SaveAsPNG(RenderTargetBitmap bmp, string filename)
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

        public DiagramLegendViewModel DiagramLegend { get; }

        public DiagramItemCollection DiagramItems { get; set; }

        public bool ShowLegend { get; set; }

        private void OnShowLegendChanged()
        {
            settings.ShowLegend = ShowLegend;
            settingsProvider.SaveSettings(settings);
        }

        public bool HasItems => DiagramItems != null && DiagramItems.Count > 0;

        public DiagramItemCollection HeaderItems { get; set; }

        public MessageSelectionContext Selection { get; }


        protected override void OnActivate()
        {
            base.OnActivate();
            DiagramLegend.ActivateWith(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DiagramLegend.DeactivateWith(this);
        }

        public void Handle(SelectedMessageChanged message)
        {
            var storedMessage = message.Message;
            if (storedMessage == null)
            {
                ClearState();
                return;
            }

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
            {
                ClearState();
                return;
            }

            if (loadedConversationId == conversationId && DiagramItems.Any()) //If we've already displayed this diagram
            {
                RefreshSelection(storedMessage.Id);
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
            Selection.SelectedMessage = storedMessage;
        }

        void RefreshSelection(string selectedId)
        {
            foreach (var item in DiagramItems.OfType<Handler>())
            {
                item.IsFocused = false;
            }

            foreach (var item in DiagramItems.OfType<Arrow>())
            {
                if (string.Equals(item.SelectedMessage.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.IsFocused = true;
                    Selection.SelectedMessage = item.SelectedMessage;
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
            DiagramItems.Clear();
            HeaderItems.Clear();
            NotifyOfPropertyChange(nameof(HasItems));
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