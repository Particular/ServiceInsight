using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Startup;

namespace NServiceBus.Profiler.Desktop.LogWindow
{
    public class RichTextBoxAppender : AppenderSkeleton
    {
        private readonly RichTextBox _richtextBox;
        private readonly Paragraph _paragraph;
        private const int MaxTextLength = 100000;

        public RichTextBoxAppender(RichTextBox textbox)
        {
            _richtextBox = textbox;
            _paragraph = new Paragraph();
            CreateFilters();
            Layout = CreateLogLayout();
            Document.Blocks.Add(_paragraph);
        }

        private void CreateFilters()
        {
            LoggerMatchingTypes.ForEach(type => AddFilter(new LoggerMatchFilter {AcceptOnMatch = true, LoggerToMatch = type.FullName }));
            AddFilter(new DenyAllFilter());
        }

        private IEnumerable<Type> LoggerMatchingTypes
        {
            get
            {
                yield return typeof (IServiceControl);
                yield return typeof (IQueueOperationsAsync);
                yield return typeof (IQueueOperations);
            }
        }

        private ILayout CreateLogLayout()
        {
            return new PatternLayout(LoggingConfig.LogPattern);
        }

        protected FlowDocument Document
        {
            get { return _richtextBox.Document; }
        }

        protected InlineCollection Lines
        {
            get { return _paragraph.Inlines; }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_richtextBox.Dispatcher.CheckAccess())
            {
                UpdateLog(loggingEvent);
            }
            else
            {
                _richtextBox.Dispatcher.BeginInvoke((Action)(() => UpdateLog(loggingEvent)));
            }
        }

        public void Clear()
        {
            Lines.Clear();
        }

        private void UpdateLog(LoggingEvent loggingEvent)
        {
            if (IsTextLarge())
            {
                Clear();
            }

            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(stringWriter, loggingEvent);
            var log = stringWriter.ToString();

            switch (loggingEvent.Level.ToString())
            {
                case "INFO":
                    AppendText(log, Colors.Black, true);
                    break;
                case "WARN":
                    AppendText(log, Colors.DarkOrange);
                    break;
                case "ERROR":
                    AppendText(log, Colors.Red);
                    break;
                case "FATAL":
                    AppendText(log, Colors.DarkOrange);
                    break;
                case "DEBUG":
                    AppendText(log, Colors.Green);
                    break;
                default:
                    AppendText(log, Colors.Black);
                    break;
            }

            _richtextBox.ScrollToEnd();
        }

        private void AppendText(string log, Color color, bool bold = false)
        {
            Lines.Add(CreateInline(log, color, bold));
        }

        private static Inline CreateInline(string text, Color color, bool bold)
        {
            return new Run
            {
                Text = text,
                Foreground = new SolidColorBrush(color),
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal
            };
        }

        private bool IsTextLarge()
        {
            return Lines.Count > MaxTextLength;
        }
    }
}