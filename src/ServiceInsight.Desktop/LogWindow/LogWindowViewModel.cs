namespace Particular.ServiceInsight.Desktop.LogWindow
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Framework;
    using Framework.Rx;
    using ReactiveUI;
    using Serilog.Events;
    using Serilog.Formatting;
    using Serilog.Formatting.Display;

    public class LogWindowViewModel : Screen
    {
        public static Subject<LogEvent> LogObserver = new Subject<LogEvent>();

        readonly IClipboard clipboard;
        ITextFormatter textFormatter;
        const int MaxTextLength = 5000;

        public LogWindowViewModel(IClipboard clipboard)
        {
            this.clipboard = clipboard;
            Logs = new ReactiveList<LogMessage>();
            textFormatter = new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}", CultureInfo.InvariantCulture);
            LogObserver.ObserveOn(RxApp.MainThreadScheduler).Subscribe(UpdateLog);

            ClearCommand = this.CreateCommand(Clear);
            CopyCommand = this.CreateCommand(Copy);
        }

        public ReactiveList<LogMessage> Logs { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }

        void Clear()
        {
            Logs.Clear();
        }

        void Copy()
        {
            clipboard.CopyTo(Logs.Aggregate("", (s, l) => s + l.Log));
        }

        void UpdateLog(LogEvent loggingEvent)
        {
            if (Logs.Count > MaxTextLength)
                Clear();

            var sr = new StringWriter();
            textFormatter.Format(loggingEvent, sr);
            var log = sr.ToString();

            switch (loggingEvent.Level)
            {
                case LogEventLevel.Information:
                    Logs.Add(new LogMessage(log, Colors.Black, true));
                    break;

                case LogEventLevel.Warning:
                    Logs.Add(new LogMessage(log, Colors.DarkOrange));
                    break;

                case LogEventLevel.Error:
                    Logs.Add(new LogMessage(log, Colors.Red));
                    break;

                case LogEventLevel.Fatal:
                    Logs.Add(new LogMessage(log, Colors.DarkOrange));
                    break;

                case LogEventLevel.Debug:
                    Logs.Add(new LogMessage(log, Colors.Green));
                    break;

                default:
                    Logs.Add(new LogMessage(log, Colors.Black));
                    break;
            }
        }
    }
}