namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;

    public interface ILogWindowView
    {
        void Initialize();

        void Clear();

        void Copy();
    }

    public partial class LogWindowView : ILogWindowView
    {
        public static Subject<LogEvent> LogObserver = new Subject<LogEvent>();

        Paragraph paragraph;
        ITextFormatter textFormatter;
        const int MaxTextLength = 5000;

        public LogWindowView()
        {
            InitializeComponent();

            paragraph = new Paragraph();
            textFormatter = new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", CultureInfo.InvariantCulture);
            LogContainer.Document.Blocks.Add(paragraph);
        }

        public void Initialize()
        {
            LogObserver.ObserveOnDispatcher().Subscribe(UpdateLog);
        }

        public void Clear()
        {
            if (paragraph != null)
                paragraph.Inlines.Clear();
        }

        public void Copy()
        {
            LogContainer.Copy();
        }

        void UpdateLog(LogEvent loggingEvent)
        {
            if (IsTextLarge())
            {
                Clear();
            }

            var sr = new StringWriter();
            textFormatter.Format(loggingEvent, sr);
            var log = sr.ToString();

            switch (loggingEvent.Level)
            {
                case LogEventLevel.Information:
                    AppendText(log, Colors.Black, true);
                    break;

                case LogEventLevel.Warning:
                    AppendText(log, Colors.DarkOrange);
                    break;

                case LogEventLevel.Error:
                    AppendText(log, Colors.Red);
                    break;

                case LogEventLevel.Fatal:
                    AppendText(log, Colors.DarkOrange);
                    break;

                case LogEventLevel.Debug:
                    AppendText(log, Colors.Green);
                    break;

                default:
                    AppendText(log, Colors.Black);
                    break;
            }

            LogContainer.ScrollToEnd();
        }

        void AppendText(string log, Color color, bool bold = false)
        {
            paragraph.Inlines.Add(CreateInline(log, color, bold));
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
            return paragraph.Inlines.Count > MaxTextLength;
        }
    }
}