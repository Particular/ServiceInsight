namespace ServiceInsight.LogWindow
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interactivity;

    public class LoggingRichTextBoxBehavior : Behavior<RichTextBox>
    {
        public static readonly DependencyProperty LogDataProperty = DependencyProperty.Register(
            "LogData", typeof(object), typeof(LoggingRichTextBoxBehavior), new FrameworkPropertyMetadata(default(object), LogDataChanged));

        static void LogDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((LoggingRichTextBoxBehavior)dependencyObject).OnLogDataChanged(dependencyPropertyChangedEventArgs);
        }

        private Paragraph paragraph;
        IDisposable logSubscription;

        public object LogData
        {
            get { return GetValue(LogDataProperty); }
            set { SetValue(LogDataProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.Document == null)
            {
                AssociatedObject.Document = new FlowDocument();
            }

            paragraph = new Paragraph();

            AssociatedObject.Document.Blocks.Clear();
            AssociatedObject.Document.Blocks.Add(paragraph);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            paragraph = null;
        }

        void OnLogDataChanged(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var logs = dependencyPropertyChangedEventArgs.NewValue as ObservableCollection<LogMessage>;
            if (logs == null)
            {
                paragraph.Inlines.Clear();
                return;
            }

            Interlocked.Exchange(ref logSubscription, null)?.Dispose();

            paragraph.Inlines.Clear();
            foreach (var log in logs)
            {
                paragraph.Inlines.Add(new Run(log.Log) { Foreground = log.Brush, FontWeight = log.Weight });
            }

            logSubscription = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => logs.CollectionChanged += h,
                h => logs.CollectionChanged -= h)
                .Subscribe(ProcessChange);
        }

        void ProcessChange(EventPattern<NotifyCollectionChangedEventArgs> args)
        {
            if (args.EventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                var log = (LogMessage)args.EventArgs.NewItems[0];

                paragraph.Inlines.Add(new Run(log.Log) { Foreground = log.Brush, FontWeight = log.Weight });
            }

            if (args.EventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                paragraph.Inlines.Clear();
            }
        }
    }
}