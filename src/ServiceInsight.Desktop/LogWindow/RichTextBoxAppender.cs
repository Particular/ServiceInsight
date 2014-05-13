namespace Particular.ServiceInsight.Desktop.LogWindow
{
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
    using ServiceControl;
    using Startup;

    public class RichTextBoxAppender : AppenderSkeleton
    {
        RichTextBox richtextBox;
        Paragraph paragraph;
        const int MaxTextLength = 5000;

        public RichTextBoxAppender(RichTextBox textbox)
        {
            richtextBox = textbox;
            paragraph = new Paragraph();
            CreateFilters();
            Layout = CreateLogLayout();
            Document.Blocks.Add(paragraph);
        }

        void CreateFilters()
        {
            LoggerMatchingTypes.ForEach(type => AddFilter(new LoggerMatchFilter {AcceptOnMatch = true, LoggerToMatch = type.FullName }));
            AddFilter(new DenyAllFilter());
        }

        IEnumerable<Type> LoggerMatchingTypes
        {
            get
            {
                yield return typeof (IServiceControl);
            }
        }

        ILayout CreateLogLayout()
        {
            return new PatternLayout(LoggingConfig.LogPattern);
        }

        protected FlowDocument Document
        {
            get { return richtextBox.Document; }
        }

        protected InlineCollection Lines
        {
            get { return paragraph.Inlines; }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (richtextBox.Dispatcher.CheckAccess())
            {
                UpdateLog(loggingEvent);
            }
            else
            {
                richtextBox.Dispatcher.BeginInvoke((Action)(() => UpdateLog(loggingEvent)));
            }
        }

        public void Clear()
        {
            Lines.Clear();
        }

        void UpdateLog(LoggingEvent loggingEvent)
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

            richtextBox.ScrollToEnd();
        }

        void AppendText(string log, Color color, bool bold = false)
        {
            Lines.Add(CreateInline(log, color, bold));
        }

        static Inline CreateInline(string text, Color color, bool bold)
        {
            return new Run
            {
                Text = text,
                Foreground = new SolidColorBrush(color),
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal
            };
        }

        bool IsTextLarge()
        {
            return Lines.Count > MaxTextLength;
        }
    }
}